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
            return new EventPropagationPath(findPropagationPathEnumerable(root, propagationTest).ToList().AsReadOnly());
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

        private static IEnumerable<Control> findPropagationPathEnumerable(
            IControlParent root, PropagationTest propagationTest)
        {
            if (root == null) yield break;

            foreach (var child in root.Children.Reverse())
            {
                var outcome = propagationTest(child);
                switch (outcome)
                {
                    case PropagationTestOutcome.Miss:
                        continue;
                    case PropagationTestOutcome.PassThrough:
                        if (!(child is IControlParent childAsParent) || childAsParent.Children.Count == 0)
                        {
                            continue;
                        }
                        var potentialPath = findPropagationPathEnumerable(childAsParent, propagationTest).ToList();
                        if (potentialPath.Count == 0
                            || propagationTest(potentialPath.Last()) == PropagationTestOutcome.PassThrough)
                        {
                            continue;
                        }
                        yield return child;
                        foreach (var descendant in potentialPath)
                        {
                            yield return descendant;
                        }
                        break;
                    case PropagationTestOutcome.Hit:
                        yield return child;
                        foreach (
                            var descendant in findPropagationPathEnumerable(child as IControlParent, propagationTest))
                        {
                            yield return descendant;
                        }
                        yield break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
