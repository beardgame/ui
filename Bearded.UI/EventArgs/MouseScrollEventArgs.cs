using OpenTK.Mathematics;

namespace Bearded.UI.EventArgs
{
    public sealed class MouseScrollEventArgs : MouseEventArgs
    {
        public int DeltaScroll { get; }
        public float DeltaScrollF { get; }

        public MouseScrollEventArgs(
            Vector2d mousePosition,
            MouseButtons mouseButtons,
            ModifierKeys modifierKeys,
            int deltaScroll,
            float deltaScrollF)
            : base(mousePosition, mouseButtons, modifierKeys)
        {
            DeltaScroll = deltaScroll;
            DeltaScrollF = deltaScrollF;
        }
    }
}
