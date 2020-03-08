using System;

namespace Bearded.UI.Controls
{
    public class FocusManager
    {
        private Control currentFocus;

        public Control FocusedControl => currentFocus;

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
            if (currentFocus == null)
                return;

            if (!currentFocus.IsFocused)
                throw new InvalidOperationException("Control was set as unfocused without resetting focus manager.");

            currentFocus.Blur();
            currentFocus = null;
        }
    }
}
