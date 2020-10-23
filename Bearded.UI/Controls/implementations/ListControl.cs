using System;
using System.Collections.Generic;
using System.Linq;
using Bearded.UI.EventArgs;
using Bearded.UI.Rendering;
using Bearded.Utilities;

namespace Bearded.UI.Controls
{
    // TODO: fix scrolloffset - validateScolloffset recursion
    // TODO: extract scroll controls
    // TODO: make scoll bar
    // TODO: allow insert/removal/update of ranges
    // TODO: refactor all control operations to only happen on frame updates to prevent crashes when calling methods early

    public interface IListItemSource
    {
        int ItemCount { get; }
        double HeightOfItemAt(int index);

        Control CreateItemControlFor(int index);
        void DestroyItemControlAt(int index, Control control);
    }

    public class ListControl : CompositeControl
    {
        private readonly CompositeControl listContainer;
        private readonly CompositeControl contentContainer;

        private bool needsReload = true;

        private IListItemSource? itemSource;

        private readonly LinkedList<(Control Control, int Index, double Offset, double Height)> cells
            = new LinkedList<(Control, int, double, double)>();
        private double totalContentHeight;

        private int itemCount;

        public bool StickToBottom { get; set; } = true;
        public bool CurrentlyStuckToBottom { get; private set; }

        private double contentTopLimit;
        private double contentBottomLimit;

        public double ScrollOffset
        {
            get => contentTopLimit;
            private set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (contentTopLimit == value)
                    return;

                contentTopLimit = value;

                validateScrollPosition();
                updateContentContainerAnchors();
            }
        }

        public IListItemSource? ItemSource
        {
            get => itemSource;
            set
            {
                itemSource = value ?? throw new ArgumentNullException();
                needsReload = true;
                SetFrameNeedsUpdateIfNeeded();
            }
        }

        public ListControl(CompositeControl? listContainer = null, bool startStuckToBottom = false)
        {
            this.listContainer = listContainer ?? new CompositeControl();
            CurrentlyStuckToBottom = startStuckToBottom;

            Add(this.listContainer);

            contentContainer = new CompositeControl();
            this.listContainer.Add(contentContainer);
        }

        public override void MouseScrolled(MouseScrollEventArgs eventArgs)
        {
            if (itemSource == null)
                return;

            var delta = eventArgs.DeltaScrollF * -30;
            var offsetBefore = ScrollOffset;

            ScrollOffset = offsetBefore + delta;

            var offsetAfter = ScrollOffset;

            if (offsetAfter > offsetBefore)
            {
                onScrollDown(itemSource);
            }
            else if (offsetAfter < offsetBefore)
            {
                onScrollUp(itemSource);
            }

            if (offsetAfter != offsetBefore)
            {
                eventArgs.Handled = true;
            }
        }

        public void ScrollToTop()
        {
            if (itemSource == null)
                throw new InvalidOperationException("The ItemSource is not set.");

            ScrollOffset = 0;
            onScrollUp(itemSource);
        }

        public void ScrollToBottom()
        {
            if (itemSource == null)
                throw new InvalidOperationException("The ItemSource is not set.");

            ScrollOffset = totalContentHeight;
            onScrollDown(itemSource);
        }

        private void onScrollUp(IListItemSource itemSource)
        {
            addCellsUpwards(itemSource);
            removeCellsUpwards(itemSource);
        }

        private void onScrollDown(IListItemSource itemSource)
        {
            addCellsDownwards(itemSource);
            removeCellsDownwards(itemSource);
        }
        
        public void OnAppendItems(int addedCount, IListItemSource itemSource)
        {
            if (needsReload)
                return;

            var oldCount = itemCount;
            itemCount = itemSource.ItemCount;

            if (itemCount != oldCount + addedCount)
                throw new ArgumentException("Added count must equal the difference between previous and current ItemCount.", nameof(addedCount));

            var heightOfNewItems = Enumerable.Range(oldCount, addedCount).Sum(i => itemSource.HeightOfItemAt(i));

            totalContentHeight += heightOfNewItems;

            if (CurrentlyStuckToBottom)
                ScrollOffset = totalContentHeight;

            addCellsDownwards(itemSource);
            removeCellsDownwards(itemSource);
        }

        public void OnInsertedRange(int index, int count)
        {
            throw new NotImplementedException();
            // sum height of added items
            // update total height
            // if stuck to bottom, scroll
            // see if cells need to be inserted (yuk)
            // else add cells down/up
        }

        public void OnRemovingRange(int index, int count)
        {
            throw new NotImplementedException();
            // sum height of removing items
            // if any visible
            //  remove them
            //  update all following (index, offset)
            // add down/up (with offset, yuk)
        }

        private void onRemoveHead(int i)
        {
            // get height of removed items
            // offset scroll accordingly
            // validate scroll position

            // add cells downwards (order?)
            // remove cells downwards
        }

        protected override void FrameChanged()
        {
            base.FrameChanged();

            if (itemSource == null)
                return;

            if (needsReload)
            {
                Reload();
                needsReload = false;
            }
            else
            {
                if (CurrentlyStuckToBottom)
                    ScrollOffset = totalContentHeight;

                validateScrollPosition();
                addCellsDownwards(itemSource);
                removeCellsUpwards(itemSource);
                addCellsUpwards(itemSource);
            }
        }
        
        public void Reload()
        {
            if (itemSource == null)
                throw new InvalidOperationException("The ItemSource is not set.");

            itemCount = itemSource.ItemCount;

            ensureNoCells(itemSource);

            calculateTotalHeight(itemSource);

            if (CurrentlyStuckToBottom)
                ScrollOffset = totalContentHeight;

            validateScrollPosition();

            if (CurrentlyStuckToBottom)
                addCellsUpwards(itemSource);
            else
                addCellsDownwards(itemSource);
        }

        private void calculateTotalHeight(IListItemSource itemSource)
        {
            totalContentHeight = Enumerable
                .Range(0, itemCount)
                .Sum(i => itemSource.HeightOfItemAt(i));
        }

        private void updateContentContainerAnchors()
        {
            contentContainer.SetAnchors(
                Anchors.Default.H,
                new Anchors(new Anchor(0, -ScrollOffset), new Anchor(0, totalContentHeight - ScrollOffset)).V
                );
        }

        private void validateScrollPosition()
        {
            const double minimum = 0;
            var maximum = getMaximumScrollOffset();

            ScrollOffset = ScrollOffset.Clamped(minimum, maximum);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            CurrentlyStuckToBottom = StickToBottom && ScrollOffset == maximum;

            updateContentLimits();
        }

        private double getMaximumScrollOffset()
            => Math.Max(0, totalContentHeight - listContainer.Frame.Size.Y);

        private void updateContentLimits()
        {
            contentBottomLimit = contentTopLimit + listContainer.Frame.Size.Y;
        }

        private void removeCellsUpwards(IListItemSource itemSource)
        {
            while (cells.Count > 0)
            {
                var lastCell = cells.Last.Value;
                
                if (lastCell.Offset < contentBottomLimit)
                    break;
                
                itemSource.DestroyItemControlAt(lastCell.Index, lastCell.Control);

                contentContainer.Remove(lastCell.Control);
                cells.RemoveLast();
            }
        }

        private void removeCellsDownwards(IListItemSource itemSource)
        {
            while (cells.Count > 0)
            {
                var firstCell = cells.First.Value;

                if (bottomOf(firstCell) > contentTopLimit)
                    break;

                itemSource.DestroyItemControlAt(firstCell.Index, firstCell.Control);

                contentContainer.Remove(firstCell.Control);
                cells.RemoveFirst();
            }
        }

        private void addCellsUpwards(IListItemSource itemSource)
        {
            var firstCell = cells.Count == 0
                ? (null, itemCount, totalContentHeight, 0)
                : cells.First.Value;

            while (firstCell.Index > 0)
            {
                if (firstCell.Offset < contentTopLimit)
                    break;

                firstCell = addCellAbove(firstCell.Index - 1, firstCell.Offset, itemSource);
            }
        }

        private void addCellsDownwards(IListItemSource itemSource)
        {
            var lastCell = cells.Count == 0
                ? (null, -1, 0, 0)
                : cells.Last.Value;

            while (lastCell.Index + 1 < itemCount)
            {
                var lastCellBottom = bottomOf(lastCell);

                if (lastCellBottom > contentBottomLimit)
                    break;

                lastCell = addCellBelow(lastCell.Index + 1, lastCellBottom, itemSource);
            }
        }

        private (Control? Control, int Index, double Offset, double Height)
            addCellBelow(int index, double top, IListItemSource itemSource)
        {
            var height = itemSource.HeightOfItemAt(index);
            var bottom = top + height;

            var cell = createCellIfVisible(index, bottom, top, height, itemSource);

            if (cell.Control != null)
                cells.AddLast((cell.Control, cell.Index, cell.Offset, cell.Height));

            return cell;
        }

        private (Control? Control, int Index, double Offset, double Height)
            addCellAbove(int index, double bottom, IListItemSource itemSource)
        {
            var height = itemSource.HeightOfItemAt(index);
            var top = bottom - height;

            var cell = createCellIfVisible(index, bottom, top, height, itemSource);

            if (cell.Control != null)
                cells.AddFirst((cell.Control, cell.Index, cell.Offset, cell.Height));

            return cell;
        }

        private (Control? Control, int Index, double Offset, double Height)
            createCellIfVisible(int index, double bottom, double top, double height, IListItemSource itemSource)
        {
            var isVisible = bottom >= contentTopLimit && top <= contentBottomLimit;
            
            return isVisible
                ? (createCellControl(index, top, bottom, itemSource), index, top, height)
                : (null, index, top, height);
        }

        private Control createCellControl(int index, double top, double bottom, IListItemSource itemSource)
        {
            var control = itemSource.CreateItemControlFor(index);

            anchorCell(control, top, bottom);
            contentContainer.Add(control);

            return control;
        }

        private static void anchorCell(Control control, double cellTop, double cellBottom)
        {
            control.SetAnchors(
                Anchors.Default.H,
                new Anchors(new Anchor(0, cellTop), new Anchor(0, cellBottom)).V
            );
        }

        private double bottomOf((Control? Control, int Index, double Offset, double Height) cell)
        {
            return cell.Offset + cell.Height;
        }

        private void ensureNoCells(IListItemSource itemSource)
        {
            if (contentContainer.Children.Count > 0)
                clearChildren(itemSource);
        }

        private void clearChildren(IListItemSource itemSource)
        {
            foreach (var (control, index, _, _) in cells)
            {
                itemSource.DestroyItemControlAt(index, control);
            }
            contentContainer.RemoveAllChildren();
            cells.Clear();
        }

        public override void Render(IRendererRouter r)
        {
            // hack to make list populate even if the list itself isn't rendered
            _ = Frame;
            base.Render(r);
        }

        protected override void RenderStronglyTyped(IRendererRouter r) => r.Render(this);
    }
}
