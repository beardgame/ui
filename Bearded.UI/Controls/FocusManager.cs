using System;

namespace Bearded.UI.Controls
{
    public sealed class FocusManager
    {
        private Control? currentFocus;

        public Control? FocusedControl => currentFocus;

        public void Focus(Control control)
        {
            ensureNoControlFocused();

            currentFocus = control;
        }

        public void BlurCurrentFocus()
        {
            currentFocus = null;
        }

        private void ensureNoControlFocused()
        {
            if (currentFocus != null)
            {
                if (!currentFocus.IsFocused)
                {
                    throw new InvalidOperationException(
                        "Control was set as unfocused without resetting focus manager.");
                }
                currentFocus.Blur();

            }
            currentFocus = null;
        }
    }
}
