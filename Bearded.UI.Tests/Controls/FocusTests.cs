using System;
using Bearded.UI.Controls;
using Bearded.UI.Rendering;
using FluentAssertions;
using Xunit;

namespace Bearded.UI.Tests.Controls
{
    public static class FocusTests
    {
        public class InitialState
        {
            [Fact]
            public void IsFocusedIsFalse()
            {
                var (_, child) = rootWithFocusableChild();

                child.IsFocused.Should().BeFalse();
            }

            [Fact]
            public void RootHasFocusedDescendantIsFalse()
            {
                var (root, _) = rootWithFocusableChild();

                root.HasFocusedDescendant.Should().BeFalse();
            }

            [Fact]
            public void FocusManagedFocusedControlIsNull()
            {
                var (root, _) = rootWithFocusableChild();

                root.FocusManager.FocusedControl.Should().BeNull();
            }
        }

        public static class Focus
        {
            public class OnUnfocusedControlWithoutExistingFocusedControl
            {
                [Fact]
                public void SetsIsFocusedTrue()
                {
                    var (_, child) = rootWithFocusableChild();

                    child.Focus();

                    child.IsFocused.Should().BeTrue();
                }

                [Fact]
                public void SetsFocusManagerFocusedControl()
                {
                    var (root, child) = rootWithFocusableChild();

                    child.Focus();

                    root.FocusManager.FocusedControl.Should().Be(child);
                }

                [Fact]
                public void CallsFocusedMethod()
                {
                    var (_, child) = rootWithFocusableChild();

                    child.Focus();

                    child.FocusedMethodCalled.Should().BeTrue();
                }

                [Fact]
                public void ThrowsIfControlCanNotBeFocused()
                {
                    var (_, child) = rootWithNonFocusableChild();

                    Action action = () => child.Focus();

                    action.Should().Throw<InvalidOperationException>();
                }

                [Fact]
                public void SetsHasFocusedDescendantTrueOnParent()
                {
                    var (_, intermediate, child) = rootWithNestedFocusableChild();

                    child.Focus();

                    intermediate.HasFocusedDescendant.Should().BeTrue();
                }

                [Fact]
                public void SetsHasFocusedDescendantTrueOnRoot()
                {
                    var (root, child) = rootWithFocusableChild();

                    child.Focus();

                    root.HasFocusedDescendant.Should().BeTrue();
                }
            }

            public class OnUnfocusedControlWithExistingFocusedControl
            {
                [Fact]
                public void SetsIsFocusedTrue()
                {
                    var (_, child1, child2) = rootWithTwoFocusableChildren();
                    child1.Focus();

                    child2.Focus();

                    child2.IsFocused.Should().BeTrue();
                }

                [Fact]
                public void SetsIsFocusedFalseOnPreviouslyFocusedControl()
                {
                    var (_, child1, child2) = rootWithTwoFocusableChildren();
                    child1.Focus();

                    child2.Focus();

                    child1.IsFocused.Should().BeFalse();
                }

                [Fact]
                public void CallsLostFocusMethodOnPreviouslyFocusedControl()
                {
                    var (_, child1, child2) = rootWithTwoFocusableChildren();
                    child1.Focus();

                    child2.Focus();

                    child1.LostFocusMethodCalled.Should().BeTrue();
                }

                [Fact]
                public void ReplacesFocusManagerFocusedControl()
                {
                    var (root, child1, child2) = rootWithTwoFocusableChildren();
                    child1.Focus();

                    child2.Focus();

                    root.FocusManager.FocusedControl.Should().Be(child2);
                }

                [Fact]
                public void SetsHasFocusedDescendantTrueOnParent()
                {
                    var (_, _, intermediate2, child1, child2) = rootWithTwoNestedFocusableChildren();
                    child1.Focus();

                    child2.Focus();

                    intermediate2.HasFocusedDescendant.Should().BeTrue();
                }

                [Fact]
                public void SetsHasFocusedDescendantFalseOnParentOfPreviouslyFocusedControl()
                {
                    var (_, intermediate1, _, child1, child2) = rootWithTwoNestedFocusableChildren();
                    child1.Focus();

                    child2.Focus();

                    intermediate1.HasFocusedDescendant.Should().BeFalse();
                }

                [Fact]
                public void RetainsHasFocusedDescendantTrueOnSharedParent()
                {
                    var (_, intermediate, child1, child2) = rootWithTwoFocusableChildrenSharingParent();
                    child1.Focus();

                    child2.Focus();

                    intermediate.HasFocusedDescendant.Should().BeTrue();
                }

                [Fact]
                public void RetainsHasFocusedDescendantTrueOnRoot()
                {
                    var (root, child1, child2) = rootWithTwoFocusableChildren();
                    child1.Focus();

                    child2.Focus();

                    root.HasFocusedDescendant.Should().BeTrue();
                }
            }

            public class OnFocusedControl
            {
                [Fact]
                public void DoesNotCallFocusedMethod()
                {
                    var (_, child) = rootWithFocusableChild();
                    child.Focus();
                    child.FocusedMethodCalled = false;

                    child.Focus();

                    child.FocusedMethodCalled.Should().BeFalse();
                }
            }
        }

        public static class Unfocus
        {
            public class OnFocusedControl
            {
                [Fact]
                public void SetsIsFocusedFalse()
                {
                    var (_, child) = rootWithFocusableChild();
                    child.Focus();

                    child.Unfocus();

                    child.IsFocused.Should().BeFalse();
                }

                [Fact]
                public void ResetsFocusManagerFocusedControl()
                {
                    var (root, child) = rootWithFocusableChild();
                    child.Focus();

                    child.Unfocus();

                    root.FocusManager.FocusedControl.Should().BeNull();
                }

                [Fact]
                public void CallsLostFocusMethod()
                {
                    var (_, child) = rootWithFocusableChild();
                    child.Focus();

                    child.Unfocus();

                    child.LostFocusMethodCalled.Should().BeTrue();
                }

                [Fact]
                public void SetsHasFocusedDescendantFalseOnParent()
                {
                    var (_, intermediate, child) = rootWithNestedFocusableChild();
                    child.Focus();

                    child.Unfocus();

                    intermediate.HasFocusedDescendant.Should().BeFalse();
                }
            }

            public class OnUnfocusedControl
            {
                [Fact]
                public void DoesNotCallLostFocusMethod()
                {
                    var (_, child) = rootWithFocusableChild();

                    child.Unfocus();

                    child.LostFocusMethodCalled.Should().BeFalse();
                }
            }

            public class OnParentOfFocusedControl
            {
                [Fact]
                public void SetsIsFocusedFalseOnPreviouslyFocusedControl()
                {
                    var (_, intermediate, child) = rootWithNestedFocusableChild();
                    child.Focus();

                    intermediate.Unfocus();

                    child.IsFocused.Should().BeFalse();
                }

                [Fact]
                public void ResetsFocusManagerFocusedControl()
                {
                    var (root, intermediate, child) = rootWithNestedFocusableChild();
                    child.Focus();

                    intermediate.Unfocus();

                    root.FocusManager.FocusedControl.Should().BeNull();
                }

                [Fact]
                public void CallsLostFocusMethodOnPreviouslyFocusedControl()
                {
                    var (_, intermediate, child) = rootWithNestedFocusableChild();
                    child.Focus();

                    intermediate.Unfocus();

                    child.LostFocusMethodCalled.Should().BeTrue();
                }

                [Fact]
                public void SetsHasFocusedDescendantFalse()
                {
                    var (_, intermediate, child) = rootWithNestedFocusableChild();
                    child.Focus();

                    intermediate.Unfocus();

                    intermediate.HasFocusedDescendant.Should().BeFalse();
                }
            }
        }

        // root - child
        private static (RootControl, TestControl) rootWithFocusableChild()
        {
            var root = new RootControl();
            var child = new FocusableTestControl();
            root.Add(child);
            return (root, child);
        }

        // root - intermediate - child
        private static (RootControl, CompositeControl, TestControl) rootWithNestedFocusableChild()
        {
            var root = new RootControl();
            var intermediateNode = new CompositeControl();
            var child = new FocusableTestControl();
            root.Add(intermediateNode);
            intermediateNode.Add(child);
            return (root, intermediateNode, child);
        }

        // root - child1
        //      \ child2
        private static (RootControl, TestControl, TestControl) rootWithTwoFocusableChildren()
        {
            var root = new RootControl();
            var child1 = new FocusableTestControl();
            var child2 = new FocusableTestControl();
            root.Add(child1);
            root.Add(child2);
            return (root, child1, child2);
        }

        // root - intermediate1 - child1
        //      \ intermediate2 - child2
        private static (RootControl, CompositeControl, CompositeControl, TestControl, TestControl)
            rootWithTwoNestedFocusableChildren()
        {
            var root = new RootControl();
            var intermediateNode1 = new CompositeControl();
            var intermediateNode2 = new CompositeControl();
            var child1 = new FocusableTestControl();
            var child2 = new FocusableTestControl();
            root.Add(intermediateNode1);
            root.Add(intermediateNode2);
            intermediateNode1.Add(child1);
            intermediateNode2.Add(child2);
            return (root, intermediateNode1, intermediateNode2, child1, child2);
        }

        // root - intermediate - child1
        //                     \ child2
        private static (RootControl, CompositeControl, TestControl, TestControl)
            rootWithTwoFocusableChildrenSharingParent()
        {
            var root = new RootControl();
            var intermediateNode = new CompositeControl();
            var child1 = new FocusableTestControl();
            var child2 = new FocusableTestControl();
            root.Add(intermediateNode);
            intermediateNode.Add(child1);
            intermediateNode.Add(child2);
            return (root, intermediateNode, child1, child2);
        }

        // root - child
        private static (RootControl, TestControl) rootWithNonFocusableChild()
        {
            var root = new RootControl();
            var child = new NonFocusableTestControl();
            root.Add(child);
            return (root, child);
        }

        private class FocusableTestControl : TestControl
        {
            internal FocusableTestControl()
            {
                CanBeFocused = true;
            }
        }

        private class NonFocusableTestControl : TestControl
        {
            internal NonFocusableTestControl()
            {
                CanBeFocused = false;
            }
        }

        private class TestControl : Control
        {
            internal bool FocusedMethodCalled;
            internal bool LostFocusMethodCalled;

            protected override void RenderStronglyTyped(IRendererRouter r) {}

            protected override void Focused()
            {
                base.Focused();
                FocusedMethodCalled = true;
            }

            protected override void LostFocus()
            {
                base.LostFocus();
                LostFocusMethodCalled = true;
            }
        }
    }
}
