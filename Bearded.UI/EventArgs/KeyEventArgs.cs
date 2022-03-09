using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Bearded.UI.EventArgs
{
    public sealed class KeyEventArgs : RoutedEventArgs
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
