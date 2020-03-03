using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bearded.UI.Rendering;

namespace Bearded.UI.Controls
{
    public class CompositeControl : Control, IControlParent, IEnumerable<Control>
    {
        private readonly List<Control> children = new List<Control>();

        public ReadOnlyCollection<Control> Children { get; }

        public bool HasFocusedDescendant => FocusState == FocusState.DescendantFocused;

        public CompositeControl()
        {
            Children = children.AsReadOnly();
        }

        public void Add(Control child)
        {
            child.AddTo(this);
            children.Add(child);
        }

        public void AddOnTopOf(Control reference, Control child)
        {
            child.AddTo(this);
            var i = children.IndexOf(reference);
            children.Insert(i, child);
        }

        public void Remove(Control child)
        {
            child.RemoveFrom(this);
            children.Remove(child);
        }

        public void RemoveAllChildren()
        {
            while (children.Count > 0)
            {
                var lastIndex = children.Count - 1;
                children[lastIndex].RemoveFrom(this);
                children.RemoveAt(lastIndex);
            }
        }

        public bool FocusDescendant(Control control)
        {
            var isChildFocused = Parent.FocusDescendant(control);
            if (isChildFocused)
                FocusState = FocusState.DescendantFocused;
            return isChildFocused;
        }

        public override void Unfocus()
        {
            switch (FocusState)
            {
                case FocusState.Unfocused:
                    break;
                case FocusState.DescendantFocused:
                    Parent.Unfocus();
                    break;
                case FocusState.Focused:
                    base.Unfocus();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void UnfocusDescendant()
        {
            Parent.UnfocusDescendant();
            FocusState = FocusState.Unfocused;
        }

        public override void SetFrameNeedsUpdate()
        {
            base.SetFrameNeedsUpdate();

            foreach (var child in children)
            {
                child.SetFrameNeedsUpdateIfNeeded();
            }
        }

        public override void Render(IRendererRouter r)
        {
            base.Render(r);

            foreach (var child in children.Where(c => c.IsVisible))
            {
                child.Render(r);
            }
        }

        protected override void RenderStronglyTyped(IRendererRouter r) => r.Render(this);

        public IEnumerator<Control> GetEnumerator() => children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
