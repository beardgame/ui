using System;
using System.Collections.Generic;
using System.Linq;
using Bearded.UI.Controls;
using Bearded.UI.EventArgs;

namespace Bearded.UI.Events
{
    static class EventRouter
    {
        public enum PropagationTestOutcome
        {
            Miss,
            PassThrough,
            Hit
        }

        public delegate void RoutedEvent<in T>(Control control, T eventData) where T : RoutedEventArgs;
        public delegate PropagationTestOutcome PropagationTest(Control control);

        // Never returns the root as part of the path.
        public static EventPropagationPath FindPropagationPath(IControlParent root, PropagationTest propagationTest) =>
            new EventPropagationPath(findPropagationPathUsingTest(root, propagationTest).AsReadOnly());

        // Never returns the root as part of the path.
        public static EventPropagationPath FindPropagationPath(IControlParent root, Control leaf)
        {
            var parent = leaf.Parent;
            var path = new List<Control>{ leaf };

            while (parent != root)
            {
                if (!(parent is Control parentAsControl))
                {
                    // Tried creating path between unconnected controls.
                    return EventPropagationPath.Empty;
                }
                path.Add(parentAsControl);

                parent = parentAsControl.Parent;
            }

            path.Reverse();
            return new EventPropagationPath(path.AsReadOnly());
        }

        private static List<Control> findPropagationPathUsingTest(IControlParent root, PropagationTest propagationTest)
        {
            var path = new List<Control>();
            tryFindPropagationPathForAnyChild(root, propagationTest, path);
            return path;
        }

        private static bool tryFindPropagationPathForAnyChild(
            IControlParent parent, PropagationTest propagationTest, List<Control> path) =>
            parent.Children.Reverse()
                .Any(child => findPropagationPathUsingTest(child, propagationTest, path));

        private static bool findPropagationPathUsingTest(
            Control root, PropagationTest propagationTest, List<Control> path)
        {
            var outcome = propagationTest(root);
            switch (outcome)
            {
                case PropagationTestOutcome.Miss:
                    return false;
                case PropagationTestOutcome.PassThrough:
                    if (!(root is IControlParent passThroughRootAsParent)) return false;
                    path.Add(root);
                    if (tryFindPropagationPathForAnyChild(passThroughRootAsParent, propagationTest, path))
                    {
                        return true;
                    }
                    path.RemoveAt(path.Count - 1);
                    return false;
                case PropagationTestOutcome.Hit:
                    path.Add(root);
                    if (!(root is IControlParent hitRootAsParent)) return true;
                    tryFindPropagationPathForAnyChild(hitRootAsParent, propagationTest, path);
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
