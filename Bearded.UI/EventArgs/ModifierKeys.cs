using Bearded.Utilities.Input;
using OpenTK.Input;

namespace Bearded.UI.EventArgs
{
    public sealed class ModifierKeys
    {
        public bool Shift { get; }
        public bool Control { get; }
        public bool Alt { get; }
        public bool Win { get; }

        private ModifierKeys(bool shift, bool control, bool alt, bool win)
        {
            Shift = shift;
            Control = control;
            Alt = alt;
            Win = win;
        }

        public static ModifierKeys FromInputManager(InputManager inputManager)
        {
            return new ModifierKeys(
                inputManager.IsKeyPressed(Key.LShift) || inputManager.IsKeyPressed(Key.RShift),
                inputManager.IsKeyPressed(Key.LControl) || inputManager.IsKeyPressed(Key.RControl),
                inputManager.IsKeyPressed(Key.LAlt) || inputManager.IsKeyPressed(Key.RAlt),
                inputManager.IsKeyPressed(Key.LWin) || inputManager.IsKeyPressed(Key.RWin));
        }
    }
}
