using OpenToolkit.Windowing.Common.Input;

namespace Bearded.UI.EventArgs
{
    public class KeyEventArgs : RoutedEventArgs
    {
        public Key Key { get; }
        public ModifierKeys ModifierKeys { get; }

        public KeyEventArgs(Key key, ModifierKeys modifierKeys)
        {
            Key = key;
            ModifierKeys = modifierKeys;
        }
    }
}
