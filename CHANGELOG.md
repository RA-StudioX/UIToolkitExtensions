# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-10-19

### Added

- Initial release of UI Toolkit Extensions package
- VisualElementExtensions class with the following methods:
  - `Hide()`: Hides a VisualElement
  - `Show()`: Shows a VisualElement
  - `ToggleVisibility()`: Toggles the visibility of a VisualElement
  - `AddClassWithDelay()`: Adds a class to a VisualElement after a specified delay
  - `RemoveClassWithDelay()`: Removes a class from a VisualElement after a specified delay
  - `SetStyleWithDelay()`: Sets a style property on a VisualElement after a specified delay
- TransitionExtensions class with the following methods:
  - `SetupTransition()`: Sets up a transition for a specific property on a VisualElement
  - `SetupTransitions()`: Sets up multiple transitions for a VisualElement
  - `ClearTransitions()`: Clears all transitions from a VisualElement
- Comprehensive documentation including usage examples and best practices
- MIT License
