using System;

namespace Bearded.UI.Controls
{
    public class FocusManager
    {
        private Control currentFocus;

        public Control FocusedControl => currentFocus;

        public void Focus(Control control)
        {
            ensureNoFocus();

            currentFocus = control;
        }

        public void Unfocus()
        {
            currentFocus = null;
        }

        private void ensureNoFocus()
        {
            if (currentFocus == null)
                return;

            if (!currentFocus.IsFocused)
                throw new InvalidOperationException("Control was set as unfocused without resetting focus manager.");

            currentFocus.Unfocus();
            currentFocus = null;
        }
    }
}
