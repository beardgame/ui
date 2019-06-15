# Bearded.UI

Bearded.UI is a UI framework with positioning, anchoring, event handling, navigation, and more. It is rendering-agnostic, meaning you should be able to use it in any project. The math types and utilities are based on the OpenTK library and other Bearded utilities.

## Components

The library has several components that can all be used together for a fully functional UI framework:

* _Anchoring system_: controls in Bearded.UI use anchors to position themselves. Controls are always positioned relative to their parent. You can anchor controls to either to the four sides. The easiest way to apply positioning to a control is to use the `Anchor` extension.
* _Event handling_: the hierarchical structure is not only used for positioning, but also for routing input events. Input events will automatically be sent to correct control, controls can be focused to capture input events, and a global fallback registry can be installed to handle shortcuts.
* _Navigation_: the navigation system allows you to register certain controls as navigation nodes. This navigation can be used for navigating between entire screens, but can also be used to switch what is shown within a given control.

## How to use this library

### Controls

The main element of Bearded.UI are controls. Controls can either be simple controls ("leaves" in the control tree) or composite controls ("interior nodes").

To add a new control, consider if the control can be composited of existing controls. If it is, create a class extending `CompositeControl`. Composite controls can still add their own behaviours, but they can also fall back on other components to handle part of the behaviour. A button for example has a label component that displays the contents of the button. For a minimal implementation of a control, look at the class `Button`.

Controls can implement the event methods, e.g. `MouseButtonHit`. This method will automatically be called if the input receives that event. The event will always be routed to the lowest control in the hierarchy that still satisfies its condition (e.g. the mouse cursor is within the bounds of that control). If a control handles an event, it should set `Handled` to true. This prevents any parent controls from receiving the same event. If a parent control wants to prevent child controls from receiving an event, they can implement preview event methods, e.g. `PreviewMouseButtonHit`. Preview event methods are called from the root component down (so in the opposite direction from normal event bubbling).

For composite components, you can add child components using the `Add` method. By default, a control will occupy the same space as its parent. Using anchor templates, you can change this. The easiest is to use the `Anchor` extension method when adding a control. For example:

```cs
    Add(new Button()
        .Anchor(a => a
            .Top(margin: 5, height: 50))
            .Left(margin: 5)
            .Right(margin: 5));
```

This adds a button that is 50 units high, and will scale horizontally to always have a margin of 5 pixels on the top, left, and right with the parent control.

### Initial setup

TODO

### Rendering

TODO

### Navigation

TODO
