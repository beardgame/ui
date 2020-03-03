using System;
using Bearded.UI.EventArgs;
using Bearded.UI.Rendering;
using Bearded.Utilities;
using OpenTK.Input;
using MouseButtonEventArgs = Bearded.UI.EventArgs.MouseButtonEventArgs;

namespace Bearded.UI.Controls
{
    public class Button : CompositeControl
    {
        [Obsolete("Use #Triggered instead.")]
        public event VoidEventHandler Clicked;

        public event GenericEventHandler<TriggerEventArgs> Triggered;

        public bool IsEnabled { get; set; } = true;

        public Button()
        {
            CanBeFocused = true;
        }

        public override void MouseButtonReleased(MouseButtonEventArgs eventArgs)
        {
            base.MouseButtonReleased(eventArgs);
            if (eventArgs.MouseButton == MouseButton.Left && IsEnabled)
            {
                Triggered?.Invoke(new TriggerEventArgs(eventArgs.ModifierKeys));
                Clicked?.Invoke();
            }
            eventArgs.Handled = true;
        }

        protected override void RenderStronglyTyped(IRendererRouter r) => r.Render(this);

        public struct TriggerEventArgs
        {
            public ModifierKeys ModifierKeys { get; }

            public TriggerEventArgs(ModifierKeys modifierKeys)
            {
                ModifierKeys = modifierKeys;
            }
        }
    }
}
