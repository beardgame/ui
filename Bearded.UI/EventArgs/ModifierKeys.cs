using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bearded.Utilities.Input;
using OpenTK.Input;

namespace Bearded.UI.EventArgs
{
    public readonly struct ModifierKeys : IEquatable<ModifierKeys>
    {
        [Flags]
        private enum ModifierKeysFlags
        {
            None = 0,
            LShift = 1,
            RShift = 1 << 1,
            LControl = 1 << 2,
            RControl = 1 << 3,
            LAlt = 1 << 4,
            RAlt = 1 << 5,
            LWin = 1 << 6,
            RWin = 1 << 7,

            Shift = LShift | RShift,
            Control = LControl | RControl,
            Alt = LAlt | RAlt,
            Win = LWin | RWin,
        }

        private static readonly ImmutableDictionary<ModifierKeysFlags, Key> flagToOpenTKKey =
            ImmutableDictionary.CreateRange(
                new Dictionary<ModifierKeysFlags, Key>
                {
                    {ModifierKeysFlags.LShift, Key.LShift},
                    {ModifierKeysFlags.RShift, Key.RShift},
                    {ModifierKeysFlags.LControl, Key.LControl},
                    {ModifierKeysFlags.RControl, Key.RControl},
                    {ModifierKeysFlags.LAlt, Key.LAlt},
                    {ModifierKeysFlags.RAlt, Key.RAlt},
                    {ModifierKeysFlags.LWin, Key.LWin},
                    {ModifierKeysFlags.RWin, Key.RWin}
                });

        private readonly ModifierKeysFlags pressedKeys;

        public bool Shift => (pressedKeys & ModifierKeysFlags.Shift) != ModifierKeysFlags.None;
        public bool Control => (pressedKeys & ModifierKeysFlags.Control) != ModifierKeysFlags.None;
        public bool Alt => (pressedKeys & ModifierKeysFlags.Alt) != ModifierKeysFlags.None;
        public bool Win => (pressedKeys & ModifierKeysFlags.Win) != ModifierKeysFlags.None;

        private ModifierKeys(ModifierKeysFlags pressedKeys)
        {
            this.pressedKeys = pressedKeys;
        }

        public static ModifierKeys None { get; } = GetBuilder().Build();

        public static ModifierKeys FromInputManager(InputManager inputManager)
        {
            return new ModifierKeys(flagToOpenTKKey
                .Where(pair => inputManager.IsKeyPressed(pair.Value))
                .Select(pair => pair.Key)
                .Aggregate(ModifierKeysFlags.None, (m1, m2) => m1 | m2));
        }

        public static Builder GetBuilder() => new Builder();

        public sealed class Builder
        {
            private ModifierKeysFlags pressedKeys;

            internal Builder() {}

            public Builder IncludeLShift()
            {
                pressedKeys |= ModifierKeysFlags.LShift;
                return this;
            }

            public Builder IncludeRShift()
            {
                pressedKeys |= ModifierKeysFlags.RShift;
                return this;
            }

            public Builder IncludeLControl()
            {
                pressedKeys |= ModifierKeysFlags.LControl;
                return this;
            }

            public Builder IncludeRControl()
            {
                pressedKeys |= ModifierKeysFlags.RControl;
                return this;
            }

            public Builder IncludeLAlt()
            {
                pressedKeys |= ModifierKeysFlags.LAlt;
                return this;
            }

            public Builder IncludeRAlt()
            {
                pressedKeys |= ModifierKeysFlags.RAlt;
                return this;
            }

            public Builder IncludeLWin()
            {
                pressedKeys |= ModifierKeysFlags.LWin;
                return this;
            }

            public Builder IncludeRWin()
            {
                pressedKeys |= ModifierKeysFlags.RWin;
                return this;
            }

            public ModifierKeys Build() => new ModifierKeys(pressedKeys);
        }

        public bool IsStrictSupersetOf(ModifierKeys other) => (pressedKeys & other.pressedKeys) == other.pressedKeys;

        public bool IsSupersetOfIgnoringLeftRight(ModifierKeys other) =>
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
