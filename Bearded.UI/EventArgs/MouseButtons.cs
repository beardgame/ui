using System;
using Bearded.Utilities.Input;

namespace Bearded.UI.EventArgs
{
    public readonly struct MouseButtons : IEquatable<MouseButtons>
    {
        public bool Left { get; }
        public bool Right { get; }
        public bool Middle { get; }

        public MouseButtons(bool left, bool right, bool middle)
        {
            Left = left;
            Right = right;
            Middle = middle;
        }

        public static MouseButtons None => new();

        public static MouseButtons FromInputManager(InputManager inputManager)
        {
            return new MouseButtons(
                inputManager.LeftMousePressed, inputManager.RightMousePressed, inputManager.MiddleMousePressed);
        }

        public MouseButtons WithLeft() => new(true, Right, Middle);
        public MouseButtons WithRight() => new(Left, true, Middle);
        public MouseButtons WithMiddle() => new(Left, Right, true);

        public bool IsSupersetOf(MouseButtons other) =>
            (Left && other.Left) == other.Left &&
            (Right && other.Right) == other.Right &&
            (Middle && other.Middle) == other.Middle;

        public bool Equals(MouseButtons other) =>
            Left == other.Left && Right == other.Right && Middle == other.Middle;

        public override bool Equals(object? obj) => obj is MouseButtons other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Left, Right, Middle);

        public static bool operator ==(MouseButtons left, MouseButtons right) => Equals(left, right);

        public static bool operator !=(MouseButtons left, MouseButtons right) => !Equals(left, right);
    }
}
