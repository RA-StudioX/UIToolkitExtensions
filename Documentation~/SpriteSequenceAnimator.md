# SpriteSequenceAnimator Documentation

## Table of Contents

1. [Introduction](#introduction)
2. [Usage](#usage)
3. [SpriteSequenceAnimationBuilder](#spritesequenceanimationbuilder)
   - [Methods](#methods)
4. [SpriteSequenceAnimationSequence](#spritesequenceanimationsequence)
   - [Methods](#methods-1)
5. [Examples](#examples)
6. [Best Practices](#best-practices)
7. [Troubleshooting](#troubleshooting)

## Introduction

The SpriteSequenceAnimator is a powerful tool for creating sprite-based animations in Unity's UI Toolkit. It allows you to animate VisualElements using sequences of sprites, with fine-grained control over timing, looping, and custom actions.

## Usage

To use the SpriteSequenceAnimator, you need to include the following namespace:

```csharp
using RAStudio.UIToolkit.Extensions;
```

Basic usage involves creating an animation using the `AnimateWithSpriteSequence` extension method on a VisualElement:

```csharp
VisualElement element = new VisualElement();
Sprite[] sprites = LoadSprites(); // Load your sprites

var animation = element.AnimateWithSpriteSequence(sprites)
    .WithFrameDuration(100)
    .WithLoop(-1)
    .Start();
```

## SpriteSequenceAnimationBuilder

The SpriteSequenceAnimationBuilder class allows you to configure various aspects of the animation before starting it.

### Methods

- `WithFrameDuration(int duration)`: Sets the duration of each frame in milliseconds.
- `WithLoop(int count)`: Sets the number of times the animation should loop. Use -1 for infinite looping.
- `WithDelay(long delayMs)`: Sets a delay before the animation starts.
- `WithTotalDuration(long durationMs)`: Sets the total duration of the animation.
- `WithStopCondition(Func<bool> condition)`: Sets a condition that, when true, will stop the animation.
- `OnComplete(Action onComplete)`: Sets an action to be performed when the animation completes.
- `OnCompleteLoop(Action onCompleteLoop)`: Sets an action to be performed when each loop of the animation completes.
- `WithCustomSpriteApplication(Action<Sprite> applyAction)`: Sets a custom action for applying sprites to the VisualElement.
- `WithFrameAction(int frameIndex, Action action)`: Adds an action to be performed on a specific frame of the animation.
- `Start()`: Starts the animation.
- `Stop()`: Stops the animation.
- `Pause()`: Pauses the animation.
- `Resume()`: Resumes the paused animation.

## SpriteSequenceAnimationSequence

The SpriteSequenceAnimationSequence class allows you to create sequences of sprite animations for multiple VisualElements.

### Methods

- `Then(SpriteSequenceAnimationBuilder builder)`: Adds a sprite sequence animation to the sequence.
- `ThenWait(long delayMs)`: Adds a delay to the sequence.
- `ThenDo(Action action)`: Adds an action to be performed in the sequence.
- `WithTotalLoops(int loops)`: Sets the number of times the entire sequence should loop.
- `OnCompleteOneLoop(Action action)`: Sets an action to be performed when one loop of the sequence completes.
- `OnCompleteAllSequences(Action action)`: Sets an action to be performed when all loops of the sequence complete.
- `Start()`: Starts the sequence of animations.

## Examples

### Basic Animation

```csharp
VisualElement element = new VisualElement();
Sprite[] sprites = LoadSprites();

element.AnimateWithSpriteSequence(sprites)
    .WithFrameDuration(100)
    .WithLoop(3)
    .OnComplete(() => Debug.Log("Animation complete"))
    .Start();
```

### Animation with Custom Sprite Application

```csharp
element.AnimateWithSpriteSequence(sprites)
    .WithFrameDuration(100)
    .WithCustomSpriteApplication(sprite => element.style.backgroundImage = new StyleBackground(sprite))
    .Start();
```

### Animation Sequence

```csharp
var sequence = SpriteSequenceAnimator.CreateSpriteSequenceAnimationSequence(rootElement)
    .Then(element1.AnimateWithSpriteSequence(sprites1).WithFrameDuration(100))
    .ThenWait(500)
    .Then(element2.AnimateWithSpriteSequence(sprites2).WithFrameDuration(150))
    .WithTotalLoops(2)
    .OnCompleteAllSequences(() => Debug.Log("All animations complete"))
    .Start();
```

## Best Practices

1. Preload your sprites to avoid performance issues during animation.
2. Use appropriate frame durations to achieve smooth animations without overwhelming the UI.
3. Consider using animation sequences for complex, multi-element animations.
4. Use custom sprite application methods when you need more control over how sprites are applied to VisualElements.
5. Implement stop conditions for animations that should react to user input or game state changes.

## Troubleshooting

- If the animation is not visible, ensure that the VisualElement has a proper size and is visible in the hierarchy.
- If the animation seems jerky, try adjusting the frame duration or reducing the complexity of your sprites.
- If you're experiencing performance issues, consider reducing the number of simultaneous animations or optimizing your sprite images.
- Ensure that your sprite array is not empty before starting the animation.

For additional support or to report issues, please visit our [GitHub repository](https://github.com/RA-StudioX/UIToolkitExtensions/issues).
