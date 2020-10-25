using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common.Input;

namespace Bearded.UI.EventArgs
{
    public class MouseButtonEventArgs : MouseEventArgs
    {
        public MouseButton MouseButton { get; }

        public MouseButtonEventArgs(Vector2d mousePosition, ModifierKeys modifierKeys, MouseButton mouseButton)
            : base(mousePosition, modifierKeys)
        {
            MouseButton = mouseButton;
        }
    }
}
