using System;
using Bearded.Utilities;

namespace Bearded.UI.Controls
{
    public class FocusManager
    {
        private Maybe<Control> currentFocus;

        public Maybe<Control> FocusedControl => currentFocus;

        public void Focus(Control control)
        {
            ensureNoControlFocused();

            currentFocus = Maybe.Just(control);
        }

        public void BlurCurrentFocus()
        {
            currentFocus = Maybe.Nothing;
        }

        private void ensureNoControlFocused()
        {
            currentFocus.Match(focus =>
            {
                if (!focus.IsFocused)
                {
                    throw new InvalidOperationException(
                        "Control was set as unfocused without resetting focus manager.");
                }
                focus.Blur();
            });
            currentFocus = Maybe.Nothing;
        }
    }
}
