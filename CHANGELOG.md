# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.2.1] - 2024-10-21

### Improved

- In SpriteAnimationSequence changed SequenceStep list to Queue to improve preformence
- In SequenceStep changed AnimateInternal to use Time.time instead of tick time
- Implemented IDisopsable for SpriteAnimationBuilder improved memmory mangment

## [1.2.0] - 2024-10-21

### Changed

- Renamed SpriteSequenceAnimator to SpriteAnimator for clarity
- Renamed SpriteSequenceAnimationBuilder to SpriteAnimationBuilder
- Renamed SpriteSequenceAnimationSequence to SpriteAnimationSequence

### Added

- Enhanced control methods (Start, Stop, Pause, Resume) for both SpriteAnimationBuilder and SpriteAnimationSequence
- Added AnimationState enum to SpriteAnimationBuilder for better state management
- Implemented StateChanged event in SpriteAnimationBuilder for external state tracking

### Improved

- Optimized animation logic in SpriteAnimationBuilder
- Enhanced error handling and logging in SpriteAnimationBuilder
- Improved performance and reliability of SpriteAnimationSequence

## [1.1.0] - 2024-10-20

### Added

- SpriteSequenceAnimator class with the following features:
  - Animate VisualElements using sprite sequences
  - Customizable frame duration, loop count, and delay
  - Support for custom sprite application methods
  - Frame-specific actions
  - Pause, resume, and stop functionality
  - Animation sequences for multiple VisualElements

## [1.0.1] - 2024-10-19

### Fixed

- TransitionExtensions:
  - ClearTransitions Unregister callback correctly now.
  - both SetupTransition unregister callback correctly now

### Changed

- TransitionExtensions:
  - SetupTransitions with multiple properties changed to dictionary of string and object and parsed to get kvp
- README updated

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
