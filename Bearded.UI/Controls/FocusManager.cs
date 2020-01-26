using System;

namespace Bearded.UI.Controls
{
    public class FocusManager
    {
        private Control currentFocus;

        public Control FocusedControl
        {
            get
            {
                if (currentFocus == null) return null;
                if (!currentFocus.IsFocused)
                    throw new InvalidOperationException("Found currently focused control as not focused.");
                return currentFocus;
            }
        }

        public void Focus(Control control)
        {
            ensureNoFocus();

            currentFocus = control;
        }

        public void Unfocus(Control control)
        {
            if (currentFocus != control)
                throw new InvalidOperationException("Can only unfocus currently focused control.");

            ensureNoFocus();
        }

        private void ensureNoFocus()
        {
            if (currentFocus == null)
                return;

            if (currentFocus.IsFocused)
                currentFocus.Unfocus();

            currentFocus = null;
        }
    }
}
