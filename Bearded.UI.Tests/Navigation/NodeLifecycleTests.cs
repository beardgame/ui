using System;
using System.Collections.Generic;
using Bearded.UI.Controls;
using Bearded.UI.Navigation;
using FluentAssertions;
using Xunit;

namespace Bearded.UI.Tests.Navigation;

public sealed class NodeLifecycleTests
{
    private readonly NavigationController nav;

    public NodeLifecycleTests()
    {
        var root = new CompositeControl();
        nav = new NavigationController(
            root,
            new DependencyResolver(),
            new Dictionary<Type, object> { { typeof(TestNode), TestNode.ModelFactory } },
            new Dictionary<Type, object> { { typeof(TestNode), TestNode.ViewFactory } });
    }

    [Fact]
    public void PushingNodesInitializesCorrectly()
    {
        var node = nav.Push<TestNode, TestParameter>(TestParameter.UniqueInstance());

        node.Initialized.Should().BeTrue();
        node.Terminated.Should().BeFalse();
    }

    [Fact]
    public void ReplacingNodesInitializesAndTerminatesCorrectly()
    {
        var existingNode = nav.Push<TestNode, TestParameter>(TestParameter.UniqueInstance());
        var replacingNode = nav.Replace<TestNode, TestParameter>(TestParameter.UniqueInstance(), existingNode);

        existingNode.Terminated.Should().BeTrue();
        replacingNode.Initialized.Should().BeTrue();
        replacingNode.Terminated.Should().BeFalse();
    }

    [Fact]
    public void ReplacingAllNodesInitializesAndTerminatesCorrectly()
    {
        var existingNode1 = nav.Push<TestNode, TestParameter>(TestParameter.UniqueInstance());
        var existingNode2 = nav.Push<TestNode, TestParameter>(TestParameter.UniqueInstance());
        var replacingNode = nav.ReplaceAll<TestNode, TestParameter>(TestParameter.UniqueInstance());

        existingNode1.Terminated.Should().BeTrue();
        existingNode2.Terminated.Should().BeTrue();
        replacingNode.Initialized.Should().BeTrue();
        replacingNode.Terminated.Should().BeFalse();
    }

    [Fact]
    public void ClosingNodesTerminatesCorrectly()
    {
        var node = nav.Push<TestNode, TestParameter>(TestParameter.UniqueInstance());
        nav.Close(node);

        node.Initialized.Should().BeTrue();
        node.Terminated.Should().BeTrue();
    }

    [Fact]
    public void ClosingAllNodesTerminatesCorrectly()
    {
        var node1 = nav.Push<TestNode, TestParameter>(TestParameter.UniqueInstance());
        var node2 = nav.Push<TestNode, TestParameter>(TestParameter.UniqueInstance());
        nav.CloseAll();

        node1.Initialized.Should().BeTrue();
        node1.Terminated.Should().BeTrue();
        node2.Initialized.Should().BeTrue();
        node2.Terminated.Should().BeTrue();
    }

    [Fact]
    public void ExitingNavigationTerminatesCorrectly()
    {
        var node1 = nav.Push<TestNode, TestParameter>(TestParameter.UniqueInstance());
        var node2 = nav.Push<TestNode, TestParameter>(TestParameter.UniqueInstance());
        nav.Exit();

        node1.Initialized.Should().BeTrue();
        node1.Terminated.Should().BeTrue();
        node2.Initialized.Should().BeTrue();
        node2.Terminated.Should().BeTrue();
    }

    [Fact]
    public void ParameterIsPassedIntoModel()
    {
        var param = TestParameter.UniqueInstance();
        var node = nav.Push<TestNode, TestParameter>(param);

        node.PassedInParameter.Should().Be(param);
    }

    private sealed class TestNode : NavigationNode<TestParameter>
    {
        public static Func<TestNode> ModelFactory => () => new TestNode();
        public static Func<TestNode, Control> ViewFactory => _ => new SimpleControl();

        public bool Initialized { get; private set; }
        public bool Terminated { get; private set; }
        public TestParameter? PassedInParameter { get; private set; }

        protected override void Initialize(DependencyResolver dependencies, TestParameter parameters)
        {
            Initialized = true;
            PassedInParameter = parameters;
        }

        public override void Terminate()
        {
            Terminated = true;
        }
    }

    private sealed class TestParameter
    {
        // Equality should by default use reference equality, so each new instance is unique.
        public static TestParameter UniqueInstance() => new();
    }
}
