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

        public static ModifierKeys None { get; } = GetBuilder().Build();

        public static ModifierKeys FromInputManager(InputManager inputManager)
        {
            return new ModifierKeys(
                inputManager.IsKeyPressed(Key.LShift) || inputManager.IsKeyPressed(Key.RShift),
                inputManager.IsKeyPressed(Key.LControl) || inputManager.IsKeyPressed(Key.RControl),
                inputManager.IsKeyPressed(Key.LAlt) || inputManager.IsKeyPressed(Key.RAlt),
                inputManager.IsKeyPressed(Key.LWin) || inputManager.IsKeyPressed(Key.RWin));
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
    }
}
