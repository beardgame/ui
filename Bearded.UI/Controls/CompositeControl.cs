using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bearded.UI.Rendering;
using Bearded.Utilities;

namespace Bearded.UI.Controls
{
    public class CompositeControl : Control, IControlParent, IFocusParent, IEnumerable<Control>
    {
        private readonly List<Control> children = new List<Control>();

        public ReadOnlyCollection<Control> Children { get; }

        public bool HasFocusedDescendant => FocusState == FocusState.DescendantFocused;
        private Maybe<Control> focusedDescendant = Maybe.Nothing;

        public CompositeControl()
        {
            Children = children.AsReadOnly();
        }

        public static CompositeControl CreateClickThrough() => new CompositeControl {IsClickThrough = true};

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

        bool IFocusParent.PropagateFocus(Control control)
        {
            var isChildFocused = FocusParent.PropagateFocus(control);
            if (!isChildFocused)
            {
                return false;
            }

            focusedDescendant = Maybe.Just(control);
            FocusState = FocusState.DescendantFocused;
            return true;
        }

        public override void Blur()
        {
            switch (FocusState)
            {
                case FocusState.None:
                    break;
                case FocusState.DescendantFocused:
                    focusedDescendant.Match(control => control.Blur());
                    break;
                case FocusState.Focused:
                    base.Blur();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void IFocusParent.PropagateBlur()
        {
            FocusState = FocusState.None;
            focusedDescendant = Maybe.Nothing;

            (Parent as IFocusParent)?.PropagateBlur();
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
