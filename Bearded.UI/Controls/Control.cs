using System;
using Bearded.UI.EventArgs;
using Bearded.UI.Rendering;
using Bearded.Utilities;

namespace Bearded.UI.Controls
{
    public abstract class Control
    {
        public IControlParent? Parent { get; private set; }

        private Frame frame;
        private bool frameNeedsUpdate = true;

        public HorizontalAnchors HorizontalAnchors { get; private set; } = Anchors.Default.H;
        public VerticalAnchors VerticalAnchors { get; private set; } = Anchors.Default.V;

        public Frame Frame
        {
            get
            {
                if (frameNeedsUpdate)
                {
                    recalculateFrame();
                }

                return frame;
            }
        }

        private bool isVisible = true;
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible == value)
                    return;
                isVisible = value;

                if (isVisible)
                {
                    MadeVisible();
                }
                else
                {
                    MadeInvisible();
                    if (IsFocused)
                        Unfocus();
                }
            }
        }

        public bool IsClickThrough { get; protected set; }

        public bool IsFocused { get; private set; }

        private bool canBeFocused = false;
        public bool CanBeFocused
        {
            get => canBeFocused;
            protected set
            {
                if (!value && IsFocused)
                {
                    Unfocus();
                }
                canBeFocused = value;
            }
        }

        public void Focus()
        {
            if (!TryFocus())
                throw new InvalidOperationException("Could not focus control.");
        }

        public void Unfocus()
        {
            if (IsFocused)
                LostFocus();

            IsFocused = false;
        }

        public virtual bool TryFocus()
        {
            if (!CanBeFocused || !IsVisible)
                return false;
            if (IsFocused)
                return true;

            if (Parent == null)
                throw new InvalidOperationException("Cannot focus a control without a parent.");

            IsFocused = Parent.FocusDescendant(this);

            if (IsFocused)
                Focused();

            return IsFocused;
        }

        public void SetAnchors(HorizontalAnchors horizontal, VerticalAnchors vertical)
        {
            HorizontalAnchors = horizontal;
            VerticalAnchors = vertical;

            SetFrameNeedsUpdateIfNeeded();
        }

        public void SetFrameNeedsUpdateIfNeeded()
        {
            if (frameNeedsUpdate)
                return;

            SetFrameNeedsUpdate();
        }

        public virtual void SetFrameNeedsUpdate()
        {
            if (Parent == null)
                throw new InvalidOperationException("Controls without a parent don't need updates.");

            frameNeedsUpdate = true;
        }

        private void recalculateFrame()
        {
            if (Parent == null)
                throw new InvalidOperationException("Trying to recalculate frames on a control without a parent.");

            var parentFrame = Parent.Frame;

            frame = new Frame(
                ((Anchors) HorizontalAnchors).CalculateIntervalWithin(parentFrame.X),
                ((Anchors) VerticalAnchors).CalculateIntervalWithin(parentFrame.Y));

            frameNeedsUpdate = false;

            FrameChanged();
        }

        public void RemoveFromParent()
        {
            if (Parent == null)
                throw new InvalidOperationException("This control doesn't have a parent.");

            Parent.Remove(this);
        }

        internal void AddTo(IControlParent parent)
        {
            if (Parent != null)
                throw new InvalidOperationException("This control already has a parent.");

            Parent = parent;

            OnAddingToParent();
        }

        internal void RemoveFrom(IControlParent parent)
        {
            if (parent != Parent)
                throw new InvalidOperationException("This control has a different parent.");

            OnRemovingFromParent();

            Parent = null;
        }

        public virtual void Render(IRendererRouter r)
        {
            RenderStronglyTyped(r);
        }

        protected abstract void RenderStronglyTyped(IRendererRouter r);

        public event GenericEventHandler<MouseEventArgs>? MouseEnter;
        public event GenericEventHandler<MouseEventArgs>? MouseMove;
        public event GenericEventHandler<MouseEventArgs>? MouseExit;
        public event GenericEventHandler<MouseButtonEventArgs>? MouseButtonDown;
        public event GenericEventHandler<MouseButtonEventArgs>? MouseButtonRelease;

        public virtual void PreviewMouseEntered(MouseEventArgs eventArgs) { }
        public virtual void MouseEntered(MouseEventArgs eventArgs)
        {
            MouseEnter?.Invoke(eventArgs);
        }
        public virtual void PreviewMouseMoved(MouseEventArgs eventArgs) { }
        public virtual void MouseMoved(MouseEventArgs eventArgs)
        {
            MouseMove?.Invoke(eventArgs);
        }
        public virtual void PreviewMouseExited(MouseEventArgs eventArgs) { }
        public virtual void MouseExited(MouseEventArgs eventArgs)
        {
            MouseExit?.Invoke(eventArgs);
        }
        public virtual void PreviewMouseButtonHit(MouseButtonEventArgs eventArgs) { }

        public virtual void MouseButtonHit(MouseButtonEventArgs eventArgs)
        {
            MouseButtonDown?.Invoke(eventArgs);
        }
        public virtual void PreviewMouseButtonReleased(MouseButtonEventArgs eventArgs) { }

        public virtual void MouseButtonReleased(MouseButtonEventArgs eventArgs)
        {
            MouseButtonRelease?.Invoke(eventArgs);
        }
        public virtual void PreviewMouseScrolled(MouseScrollEventArgs eventArgs) { }
        public virtual void MouseScrolled(MouseScrollEventArgs eventArgs) { }

        public virtual void PreviewKeyHit(KeyEventArgs eventArgs) { }
        public virtual void KeyHit(KeyEventArgs eventArgs) { }
        public virtual void PreviewKeyReleased(KeyEventArgs eventArgs) { }
        public virtual void KeyReleased(KeyEventArgs eventArgs) { }
        public virtual void PreviewCharacterTyped(CharEventArgs eventArgs) { }
        public virtual void CharacterTyped(CharEventArgs eventArgs) { }

        protected virtual void Focused() { }
        protected virtual void LostFocus() { }

        protected virtual void FrameChanged() { }

        protected virtual void MadeVisible() { } // Not called on initialization
        protected virtual void MadeInvisible() { }

        protected virtual void OnAddingToParent() { }
        protected virtual void OnRemovingFromParent() { }
    }
}
