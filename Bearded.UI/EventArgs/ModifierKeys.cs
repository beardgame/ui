using System;
using Bearded.Utilities.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Bearded.UI.EventArgs
{
    public readonly struct ModifierKeys : IEquatable<ModifierKeys>
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

        public static ModifierKeys None => new ModifierKeys();

        public static ModifierKeys FromInputManager(InputManager inputManager)
        {
            return new ModifierKeys(
                inputManager.IsKeyPressed(Keys.LeftShift) || inputManager.IsKeyPressed(Keys.RightShift),
                inputManager.IsKeyPressed(Keys.LeftControl) || inputManager.IsKeyPressed(Keys.RightControl),
                inputManager.IsKeyPressed(Keys.LeftAlt) || inputManager.IsKeyPressed(Keys.RightAlt),
                inputManager.IsKeyPressed(Keys.LeftSuper) || inputManager.IsKeyPressed(Keys.RightSuper));
        }

        public ModifierKeys WithShift() => new ModifierKeys(true, Control, Alt, Win);

        public ModifierKeys WithControl() => new ModifierKeys(Shift, true, Alt, Win);

        public ModifierKeys WithAlt() => new ModifierKeys(Shift, Control, true, Win);

        public ModifierKeys WithWin() => new ModifierKeys(Shift, Control, Alt, true);

        public bool IsSupersetOf(ModifierKeys other) =>
            (Shift && other.Shift) == other.Shift &&
            (Control && other.Control) == other.Control &&
            (Alt && other.Alt) == other.Alt &&
            (Win && other.Win) == other.Win;

        public bool Equals(ModifierKeys other) =>
            Shift == other.Shift && Control == other.Control && Alt == other.Alt && Win == other.Win;

        public override bool Equals(object? obj) => obj is ModifierKeys other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Shift, Control, Alt, Win);

        public static bool operator ==(ModifierKeys left, ModifierKeys right) => Equals(left, right);

        public static bool operator !=(ModifierKeys left, ModifierKeys right) => !Equals(left, right);
    }
}
