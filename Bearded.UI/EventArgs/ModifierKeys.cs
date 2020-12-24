using Bearded.Utilities.Input;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;

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
                inputManager.IsKeyPressed(Keys.LeftShift) || inputManager.IsKeyPressed(Keys.RightShift),
                inputManager.IsKeyPressed(Keys.LeftControl) || inputManager.IsKeyPressed(Keys.RightControl),
                inputManager.IsKeyPressed(Keys.LeftAlt) || inputManager.IsKeyPressed(Keys.RightAlt),
                inputManager.IsKeyPressed(Keys.LeftSuper) || inputManager.IsKeyPressed(Keys.RightSuper));
        }
    }
}
