using OpenToolkit.Mathematics;

namespace Bearded.UI.EventArgs
{
    public sealed class MouseScrollEventArgs : MouseEventArgs
    {
        public int DeltaScroll { get; }
        public float DeltaScrollF { get; }

        public MouseScrollEventArgs(
            Vector2d mousePosition, ModifierKeys modifierKeys, int deltaScroll, float deltaScrollF)
            : base(mousePosition, modifierKeys)
        {
            DeltaScroll = deltaScroll;
            DeltaScrollF = deltaScrollF;
        }
    }
}
