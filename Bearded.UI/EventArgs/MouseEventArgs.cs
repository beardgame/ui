using OpenTK.Mathematics;

namespace Bearded.UI.EventArgs
{
    public class MouseEventArgs : RoutedEventArgs
    {
        public Vector2d MousePosition { get; }
        public MouseButtons MouseButtons { get; }
        public ModifierKeys ModifierKeys { get; }

        public MouseEventArgs(Vector2d mousePosition, MouseButtons mouseButtons, ModifierKeys modifierKeys)
        {
            MousePosition = mousePosition;
            MouseButtons = mouseButtons;
            ModifierKeys = modifierKeys;
        }
    }
}
