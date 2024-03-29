﻿using System.Collections.Immutable;
using Bearded.UI.Controls;
using Bearded.UI.EventArgs;
using Bearded.Utilities.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using MouseButtonEventArgs = Bearded.UI.EventArgs.MouseButtonEventArgs;
using MouseEventArgs = Bearded.UI.EventArgs.MouseEventArgs;

namespace Bearded.UI.Events
{
    sealed class MouseEventManager
    {
        private static readonly ImmutableArray<MouseButton> mouseButtonsWithEvents =
            ImmutableArray.Create(MouseButton.Left, MouseButton.Middle, MouseButton.Right);

        private readonly RootControl root;
        private readonly InputManager inputManager;

        private EventPropagationPath? previousPropagationPath;

        internal MouseEventManager(RootControl root, InputManager inputManager)
        {
            this.root = root;
            this.inputManager = inputManager;
        }

        internal void Update()
        {
            var mousePosition = root.TransformViewportPosToFramePos(inputManager.MousePosition);
            var mouseButtons = MouseButtons.FromInputManager(inputManager);
            var modifierKeys = ModifierKeys.FromInputManager(inputManager);

            var path = EventRouter.FindPropagationPath(
                root, control =>
                {
                    if (!control.IsVisible || !control.Frame.ContainsPoint(mousePosition))
                    {
                        return EventRouter.PropagationTestOutcome.Miss;
                    }

                    return control.IsClickThrough
                        ? EventRouter.PropagationTestOutcome.PassThrough
                        : EventRouter.PropagationTestOutcome.Hit;
                });

            var (removedFromPath, addedToPath) = previousPropagationPath != null
                ? EventPropagationPath.CalculateDeviation(previousPropagationPath, path)
                : (EventPropagationPath.Empty, path);
            var eventArgs = new MouseEventArgs(mousePosition, mouseButtons, modifierKeys);

            // Mouse exit
            removedFromPath.PropagateEvent(
                eventArgs,
                (c, e) => c.PreviewMouseExited(e),
                (c, e) => c.MouseExited(e));

            // Mouse enter
            addedToPath.PropagateEvent(
                eventArgs,
                (c, e) => c.PreviewMouseEntered(e),
                (c, e) => c.MouseEntered(e));

            // Mouse move
            path.PropagateEvent(
                eventArgs,
                (c, e) => c.PreviewMouseMoved(e),
                (c, e) => c.MouseMoved(e));

            // Mouse clicks
            foreach (var btn in mouseButtonsWithEvents)
            {
                var action = inputManager.Actions.Mouse.FromButton(btn);
                if (action.Hit)
                {
                    path.PropagateEvent(
                        new MouseButtonEventArgs(mousePosition, mouseButtons, modifierKeys, btn),
                        (c, e) => c.PreviewMouseButtonHit(e),
                        (c, e) => c.MouseButtonHit(e));
                }
                if (action.Released)
                {
                    path.PropagateEvent(
                        new MouseButtonEventArgs(mousePosition, mouseButtons, modifierKeys, btn),
                        (c, e) => c.PreviewMouseButtonReleased(e),
                        (c, e) => c.MouseButtonReleased(e));
                }
            }

            // Mouse scroll
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (inputManager.DeltaScrollF != 0)
            {
                path.PropagateEvent(
                    new MouseScrollEventArgs(
                        mousePosition, mouseButtons, modifierKeys, inputManager.DeltaScroll, inputManager.DeltaScrollF),
                    (c, e) => c.PreviewMouseScrolled(e),
                    (c, e) => c.MouseScrolled(e));
            }

            previousPropagationPath = path;
        }
    }
}
