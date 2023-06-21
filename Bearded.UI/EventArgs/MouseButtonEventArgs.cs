using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Bearded.UI.EventArgs
{
    public sealed class MouseButtonEventArgs : MouseEventArgs
    {
        public MouseButton MouseButton { get; }

        public MouseButtonEventArgs(
            Vector2d mousePosition, MouseButtons mouseButtons, ModifierKeys modifierKeys, MouseButton mouseButton)
            : base(mousePosition, mouseButtons, modifierKeys)
        {
            MouseButton = mouseButton;
        }
    }
}
