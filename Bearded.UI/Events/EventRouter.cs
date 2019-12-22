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

        public static EventPropagationPath FindPropagationPath(IControlParent root, PropagationTest propagationTest)
        {
            return new EventPropagationPath(findPropagationPathUsingTest(root, propagationTest).ToList().AsReadOnly());
        }

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

        private static IEnumerable<Control> findPropagationPathUsingTest(
            IControlParent root, PropagationTest propagationTest)
        {
            var path = new List<Control>();
            findPropagationPathUsingTest(root, propagationTest, path);
            return path;
        }

        private static bool findPropagationPathUsingTest(
            IControlParent root, PropagationTest propagationTest, List<Control> path)
        {
            foreach (var child in root.Children.Reverse())
            {
                var outcome = propagationTest(child);
                switch (outcome)
                {
                    case PropagationTestOutcome.Miss:
                        break;
                    case PropagationTestOutcome.PassThrough:
                        if (child is IControlParent childAsPassThroughParent
                            && childAsPassThroughParent.Children.Count > 0)
                        {
                            path.Add(child);
                            var hasPathWithHit =
                                findPropagationPathUsingTest(childAsPassThroughParent, propagationTest, path);
                            if (hasPathWithHit)
                            {
                                return true;
                            }
                            // backtrack
                            path.RemoveAt(path.Count - 1);
                        }
                        break;
                    case PropagationTestOutcome.Hit:
                        path.Add(child);
                        if (child is IControlParent childAsHitParent && childAsHitParent.Children.Count > 0)
                        {
                            findPropagationPathUsingTest(childAsHitParent, propagationTest, path);
                        }
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return false;
        }
    }
}
