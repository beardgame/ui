using OpenTK.Mathematics;

namespace Bearded.UI.EventArgs
{
    public class MouseEventArgs : RoutedEventArgs
    {
        public Vector2d MousePosition { get; }
        public ModifierKeys ModifierKeys { get; }

        public MouseEventArgs(Vector2d mousePosition, ModifierKeys modifierKeys)
        {
            MousePosition = mousePosition;
            ModifierKeys = modifierKeys;
        }
    }
}
