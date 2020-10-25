using Bearded.UI.EventArgs;
using Bearded.UI.Rendering;
using Bearded.Utilities;
using OpenToolkit.Windowing.Common.Input;
using MouseButtonEventArgs = Bearded.UI.EventArgs.MouseButtonEventArgs;

namespace Bearded.UI.Controls
{
    public class Button : CompositeControl
    {
        public event GenericEventHandler<ClickEventArgs> Clicked;

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
                Click(new ClickEventArgs(eventArgs.ModifierKeys));
            }
            eventArgs.Handled = true;
        }

        public void Click(ClickEventArgs eventArgs)
        {
            Clicked?.Invoke(eventArgs);
        }

        protected override void RenderStronglyTyped(IRendererRouter r) => r.Render(this);

        public readonly struct ClickEventArgs
        {
            public ModifierKeys ModifierKeys { get; }

            public ClickEventArgs(ModifierKeys modifierKeys)
            {
                ModifierKeys = modifierKeys;
            }
        }
    }
}
