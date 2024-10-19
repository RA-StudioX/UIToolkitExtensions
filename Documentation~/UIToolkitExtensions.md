# UI Toolkit Extensions Documentation

## Table of Contents

1. [Introduction](#introduction)
2. [Installation](#installation)
3. [VisualElementExtensions](#visualelementextensions)
   - [Hide](#hide)
   - [Show](#show)
   - [ToggleVisibility](#togglevisibility)
   - [AddClassWithDelay](#addclasswithdelay)
   - [RemoveClassWithDelay](#removeclasswithdelay)
   - [SetStyleWithDelay](#setstyledwithdelay)
4. [TransitionExtensions](#transitionextensions)
   - [SetupTransition](#setuptransition)
   - [SetupTransitions](#setuptransitions)
   - [ClearTransitions](#cleartransitions)
5. [Best Practices](#best-practices)
6. [Troubleshooting](#troubleshooting)

## Introduction

UI Toolkit Extensions is a package designed to enhance the functionality of Unity's UI Toolkit. It provides a set of extension methods for VisualElements and utilities for creating smooth transitions, allowing developers to create more dynamic and interactive user interfaces with less code.

## Installation

To install the UI Toolkit Extensions package:

1. Open the Unity Package Manager (Window > Package Manager)
2. Click the '+' button and select "Add package from git URL..."
3. Enter the following URL: `https://github.com/RA-StudioX/UIToolkitExtensions.git`
4. Click 'Add'

## VisualElementExtensions

### Hide

Hides the VisualElement by setting its display style to `DisplayStyle.None`.

```csharp
public static void Hide(this VisualElement element)
```

Example:
```csharp
myElement.Hide();
```

### Show

Shows the VisualElement by setting its display style to `DisplayStyle.Flex`.

```csharp
public static void Show(this VisualElement element)
```

Example:
```csharp
myElement.Show();
```

### ToggleVisibility

Toggles the visibility of the VisualElement between hidden and visible states.

```csharp
public static void ToggleVisibility(this VisualElement element)
```

Example:
```csharp
myElement.ToggleVisibility();
```

### AddClassWithDelay

Adds a class to the VisualElement after a specified delay.

```csharp
public static void AddClassWithDelay(this VisualElement element, string className, int delayMs)
```

Example:
```csharp
myElement.AddClassWithDelay("highlight", 1000); // Adds "highlight" class after 1 second
```

### RemoveClassWithDelay

Removes a class from the VisualElement after a specified delay.

```csharp
public static void RemoveClassWithDelay(this VisualElement element, string className, int delayMs)
```

Example:
```csharp
myElement.RemoveClassWithDelay("highlight", 2000); // Removes "highlight" class after 2 seconds
```

### SetStyleWithDelay

Sets a style property on the VisualElement after a specified delay.

```csharp
public static void SetStyleWithDelay(this VisualElement element, Action<IStyle> styleAction, int delayMs)
```

Example:
```csharp
myElement.SetStyleWithDelay(style => style.backgroundColor = Color.red, 1500); // Sets background color to red after 1.5 seconds
```

## TransitionExtensions

### SetupTransition

Sets up a transition for a specific property on a VisualElement.

```csharp
public static void SetupTransition(this VisualElement element, string propertyName, int durationMs, EasingMode? easingMode = null, int delayMs = 0, Action callback = null)
```

Example:
```csharp
myElement.SetupTransition("opacity", 500, EasingMode.EaseInOut, 100, () => Debug.Log("Transition complete"));
```

### SetupTransitions

Sets up multiple transitions for a VisualElement with individual durations, easing modes, and delays.

```csharp
public static void SetupTransitions(this VisualElement element, Dictionary<string, (int durationMs, EasingMode? easingMode, int? delayMs)> transitions, Action callback = null)
```

Example:
```csharp
var transitions = new Dictionary<string, (int durationMs, EasingMode? easingMode, int? delayMs)>
{
    { "opacity", (500, EasingMode.EaseInOut, 100) },
    { "scale", (750, EasingMode.EaseOutBounce, 0) }
};
myElement.SetupTransitions(transitions, () => Debug.Log("All transitions complete"));
```

### ClearTransitions

Clears all transitions from the VisualElement.

```csharp
public static void ClearTransitions(this VisualElement element)
```

Example:
```csharp
myElement.ClearTransitions();
```

## Best Practices

1. Use transitions sparingly to avoid overwhelming the user or impacting performance.
2. Combine transitions with other UI Toolkit features for more complex animations.
3. Always provide fallback behavior for users who may have reduced motion settings enabled.
4. Use meaningful class names and consistent naming conventions when working with delayed class additions/removals.

## Troubleshooting

- If transitions are not working, ensure that the VisualElement has a valid layout and is visible in the hierarchy.
- Check that the property names used in transitions match the CSS properties exactly.
- If using multiple transitions, make sure there are no conflicting property animations.
- For performance reasons, prefer using CSS transitions for simple animations and reserve these extension methods for more complex scenarios or when you need precise control over the animation timing.

For additional support or to report issues, please visit our [GitHub repository](https://github.com/RA-StudioX/UIToolkitExtensions/issues).
