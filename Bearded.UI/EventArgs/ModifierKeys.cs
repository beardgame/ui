using System;
using Bearded.Utilities.Input;
using OpenTK.Windowing.Common.Input;
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

        public static ModifierKeys None { get; } = GetBuilder().Build();

        public static ModifierKeys FromInputManager(InputManager inputManager)
        {
            return new ModifierKeys(
                inputManager.IsKeyPressed(Keys.LeftShift) || inputManager.IsKeyPressed(Keys.RightShift),
                inputManager.IsKeyPressed(Keys.LeftControl) || inputManager.IsKeyPressed(Keys.RightControl),
                inputManager.IsKeyPressed(Keys.LeftAlt) || inputManager.IsKeyPressed(Keys.RightAlt),
                inputManager.IsKeyPressed(Keys.LeftSuper) || inputManager.IsKeyPressed(Keys.RightSuper));
        }

        public static Builder GetBuilder() => new Builder();

        public sealed class Builder
        {
            private bool shift;
            private bool control;
            private bool alt;
            private bool win;

            internal Builder() {}

            public Builder IncludeShift()
            {
                shift = true;
                return this;
            }

            public Builder IncludeControl()
            {
                control = true;
                return this;
            }

            public Builder IncludeAlt()
            {
                alt = true;
                return this;
            }

            public Builder IncludeWin()
            {
                win = true;
                return this;
            }

            public ModifierKeys Build() => new ModifierKeys(shift, control, alt, win);
        }

        public bool IsSupersetOf(ModifierKeys other) =>
            (Shift && other.Shift) == other.Shift &&
            (Control && other.Control) == other.Control &&
            (Alt && other.Alt) == other.Alt &&
            (Win && other.Win) == other.Win;

        public bool Equals(ModifierKeys other) =>
            Shift == other.Shift && Control == other.Control && Alt == other.Alt && Win == other.Win;

        public override bool Equals(object obj) => obj is ModifierKeys other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Shift.GetHashCode();
                hashCode = (hashCode * 397) ^ Control.GetHashCode();
                hashCode = (hashCode * 397) ^ Alt.GetHashCode();
                hashCode = (hashCode * 397) ^ Win.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ModifierKeys left, ModifierKeys right) => Equals(left, right);

        public static bool operator !=(ModifierKeys left, ModifierKeys right) => !Equals(left, right);
    }
}
