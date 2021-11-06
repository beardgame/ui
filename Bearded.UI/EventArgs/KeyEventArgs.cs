using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Bearded.UI.EventArgs
{
    public class KeyEventArgs : RoutedEventArgs
    {
        public Keys Key { get; }
        public ModifierKeys ModifierKeys { get; }

        public KeyEventArgs(Keys key, ModifierKeys modifierKeys)
        {
            Key = key;
            ModifierKeys = modifierKeys;
        }
    }
}
