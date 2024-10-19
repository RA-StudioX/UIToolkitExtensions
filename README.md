<p align="center">
    <a href="https://ra-studio.net" target="_blank">
        <img src="Images/RAStudio-logo.svg" alt="RA Studio Logo" width="200"/>
    </a>
</p>

# UI Toolkit Extensions

## Description

UI Toolkit Extensions is a Unity package that enhances the functionality of Unity's UI Toolkit. It provides extended Visual Elements and utilities for creating smooth transitions, making it easier to create rich and interactive user interfaces.

## Features

- Extended Visual Elements with additional functionality
- Transition utilities for creating smooth animations
- Easy integration with existing UI Toolkit projects

## Installation

To install this package, follow these steps:

1. Open the Unity Package Manager (Window > Package Manager)
2. Click the '+' button and select "Add package from git URL..."
3. Enter the following URL: `https://github.com/RA-StudioX/UIToolkitExtensions.git`
4. Click 'Add'

## Usage

### VisualElementExtensions

```csharp
using UnityEngine;
using UnityEngine.UIElements;
using RAStudio.UIToolkit.Extensions;

public class ExampleUsage : MonoBehaviour
{
    public void Start()
    {
        VisualElement element = new VisualElement();

        // Hide and show the element
        element.Hide();
        element.Show();

        // Toggle visibility
        element.ToggleVisibility();

        // Add and remove classes with delay
        element.AddClassWithDelay("highlight", 1000);
        element.RemoveClassWithDelay("highlight", 2000);

        // Set style with delay
        element.SetStyleWithDelay(style => style.backgroundColor = Color.red, 1500);
    }
}
```

### TransitionExtensions

```csharp
using UnityEngine;
using UnityEngine.UIElements;
using RAStudio.UIToolkit.Extensions;
using System.Collections.Generic;

public class TransitionExample : MonoBehaviour
{
    public void Start()
    {
        VisualElement element = new VisualElement();

        // Setup a single transition
        element.SetupTransition("opacity", 500, EasingMode.EaseInOut, 100, () => Debug.Log("Transition complete"));

        // Setup multiple transitions
        var transitions = new Dictionary<string, (int durationMs, EasingMode? easingMode, int? delayMs)>
        {
            { "opacity", (500, EasingMode.EaseInOut, 100) },
            { "scale", (750, EasingMode.EaseOutBounce, 0) }
            { "color", (100) }
        };
        element.SetupTransitions(transitions, () => Debug.Log("All transitions complete"));

        // Clear transitions
        element.ClearTransitions();
    }
}
```

## Documentation

For more detailed information about the package, its components, and how to use them, please refer to the [full documentation](https://github.com/RA-StudioX/UIToolkitExtensions/blob/main/Documentation~/UIToolkitExtensions.md).

## Requirements

- Unity 2021.3 or later
- UIToolkit

## License

This package is licensed under the MIT License. See the [LICENSE](https://github.com/RA-StudioX/UIToolkitExtensions/blob/main/LICENSE.md) file for details.

## Author

RA Studio

- Email: contact@ra-studio.net
- Website: https://ra-studio.net
- GitHub: https://github.com/RA-StudioX

## Support

If you encounter any issues or have questions, please file an issue on the [GitHub repository](https://github.com/RA-StudioX/UIToolkitExtensions/issues).
