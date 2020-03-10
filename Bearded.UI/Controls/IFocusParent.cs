namespace Bearded.UI.Controls
{
    interface IFocusParent
    {
        bool PropagateFocus(Control control);
        void PropagateBlur();
    }
}
