using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

namespace RAStudio.UIToolkit.Extensions
{
    /// <summary>
    /// Provides methods for animating UI Toolkit elements using sprite sequences.
    /// </summary>
    public static class SpriteSequenceAnimator
    {
        /// <summary>
        /// Starts building a sprite sequence animation for a VisualElement.
        /// </summary>
        /// <param name="element">The VisualElement to animate.</param>
        /// <param name="sprites">An array of sprites to use for the animation frames.</param>
        /// <returns>A SpriteSequenceAnimationBuilder to configure the animation.</returns>
        public static SpriteSequenceAnimationBuilder AnimateWithSpriteSequence(this VisualElement element, Sprite[] sprites)
        {
            return new SpriteSequenceAnimationBuilder(element, sprites);
        }

        /// <summary>
        /// Builder class for configuring sprite sequence animations.
        /// </summary>
        public class SpriteSequenceAnimationBuilder
        {
            internal readonly VisualElement _element;
            private readonly Sprite[] _sprites;
            private int _frameDuration = 100;
            private int _loopCount = 1;
            private long _delayMs = 0;
            private long? _durationMs = null;
            private Func<bool> _stopCondition = null;
            private Action _onComplete = null;
            private Action _onCompleteLoop = null;
            private Action<Sprite> _applySpriteAction;
            private Dictionary<int, Action> _frameActions = new Dictionary<int, Action>();
            private IVisualElementScheduledItem _scheduler;
            private bool _isPaused = false;
            private bool _isRunning = false;
            private bool _hasCompleted = false;

            internal SpriteSequenceAnimationBuilder(VisualElement element, Sprite[] sprites)
            {
                _element = element;
                _sprites = sprites;
                _applySpriteAction = (sprite) => _element.style.backgroundImage = new StyleBackground(sprite);
            }

            /// <summary>
            /// Sets the duration of each frame in milliseconds.
            /// </summary>
            /// <param name="duration">The duration in milliseconds.</param>
            /// <returns>The builder instance for method chaining.</returns>
            public SpriteSequenceAnimationBuilder WithFrameDuration(int duration)
            {
                _frameDuration = duration;
                return this;
            }

            /// <summary>
            /// Sets the number of times the animation should loop. Use -1 for infinite looping.
            /// </summary>
            /// <param name="count">The number of loops, or -1 for infinite looping.</param>
            /// <returns>The builder instance for method chaining.</returns>
            public SpriteSequenceAnimationBuilder WithLoop(int count = -1)
            {
                _loopCount = count;
                return this;
            }

            /// <summary>
            /// Sets a delay before the animation starts.
            /// </summary>
            /// <param name="delayMs">The delay in milliseconds.</param>
            /// <returns>The builder instance for method chaining.</returns>
            public SpriteSequenceAnimationBuilder WithDelay(long delayMs)
            {
                _delayMs = delayMs;
                return this;
            }

            /// <summary>
            /// Sets the total duration of the animation.
            /// </summary>
            /// <param name="durationMs">The total duration in milliseconds.</param>
            /// <returns>The builder instance for method chaining.</returns>
            public SpriteSequenceAnimationBuilder WithTotalDuration(long durationMs)
            {
                _durationMs = durationMs;
                return this;
            }

            /// <summary>
            /// Sets a condition that, when true, will stop the animation.
            /// </summary>
            /// <param name="condition">A function that returns true when the animation should stop.</param>
            /// <returns>The builder instance for method chaining.</returns>
            public SpriteSequenceAnimationBuilder WithStopCondition(Func<bool> condition)
            {
                _stopCondition = condition;
                return this;
            }

            /// <summary>
            /// Sets an action to be performed when the animation completes.
            /// </summary>
            /// <param name="onComplete">The action to perform on completion.</param>
            /// <returns>The builder instance for method chaining.</returns>
            public SpriteSequenceAnimationBuilder OnComplete(Action onComplete)
            {
                _onComplete = onComplete;
                return this;
            }

            /// <summary>
            /// Sets an action to be performed when each loop of the animation completes.
            /// </summary>
            /// <param name="onCompleteLoop">The action to perform on loop completion.</param>
            /// <returns>The builder instance for method chaining.</returns>
            public SpriteSequenceAnimationBuilder OnCompleteLoop(Action onCompleteLoop)
            {
                _onCompleteLoop = onCompleteLoop;
                return this;
            }

            /// <summary>
            /// Sets a custom action for applying sprites to the VisualElement.
            /// </summary>
            /// <param name="applyAction">The custom action to apply sprites.</param>
            /// <returns>The builder instance for method chaining.</returns>
            public SpriteSequenceAnimationBuilder WithCustomSpriteApplication(Action<Sprite> applyAction)
            {
                _applySpriteAction = applyAction;
                return this;
            }

            /// <summary>
            /// Adds an action to be performed on a specific frame of the animation.
            /// </summary>
            /// <param name="frameIndex">The index of the frame to perform the action on.</param>
            /// <param name="action">The action to perform.</param>
            /// <returns>The builder instance for method chaining.</returns>
            public SpriteSequenceAnimationBuilder WithFrameAction(int frameIndex, Action action)
            {
                if (frameIndex < 0 || frameIndex >= _sprites.Length)
                {
                    Debug.LogWarning($"Frame index {frameIndex} is out of bounds. The sprite sequence has {_sprites.Length} frames. This action will be ignored.");
                }
                else
                {
                    _frameActions[frameIndex] = action;
                }
                return this;
            }

            /// <summary>
            /// Starts the animation.
            /// </summary>
            /// <returns>The scheduled item for the animation.</returns>
            public IVisualElementScheduledItem Start()
            {
                if (_isRunning && !_hasCompleted)
                {
                    // If already running and not completed, do nothing
                    return _scheduler;
                }

                _isRunning = true;
                _isPaused = false;
                _hasCompleted = false;
                _scheduler = AnimateSpriteSequenceInternal(_element, _sprites, _frameDuration, _loopCount, _delayMs, _durationMs, _stopCondition, OnAnimationComplete, _onCompleteLoop, _applySpriteAction, _frameActions);
                return _scheduler;
            }

            /// <summary>
            /// Stops the animation.
            /// </summary>
            public void Stop()
            {
                if (_scheduler != null)
                {
                    _scheduler.Pause();
                    _scheduler = null;
                }
                _isRunning = false;
                _isPaused = false;
                _hasCompleted = false;
            }

            /// <summary>
            /// Pauses the animation.
            /// </summary>
            public void Pause()
            {
                if (_isRunning && !_isPaused && !_hasCompleted)
                {
                    _scheduler?.Pause();
                    _isPaused = true;
                }
            }

            /// <summary>
            /// Resumes the paused animation.
            /// </summary>
            public void Resume()
            {
                if (_isRunning && _isPaused && !_hasCompleted)
                {
                    _scheduler?.Resume();
                    _isPaused = false;
                }
            }

            private void OnAnimationComplete()
            {
                _isRunning = false;
                _hasCompleted = true;
                _onComplete?.Invoke();
            }

            internal IVisualElementScheduledItem StartInSequence(Action onStepComplete)
            {
                Stop(); // Ensure any existing animation is stopped
                _isRunning = true;
                _isPaused = false;
                _hasCompleted = false;
                _scheduler = AnimateSpriteSequenceInternal(_element, _sprites, _frameDuration, _loopCount, _delayMs, _durationMs, _stopCondition, () =>
                {
                    OnAnimationComplete();
                    onStepComplete();
                }, _onCompleteLoop, _applySpriteAction, _frameActions);
                return _scheduler;
            }
        }

        /// <summary>
        /// Creates a new sprite sequence animation sequence.
        /// </summary>
        /// <param name="rootElement">The root VisualElement to use for scheduling.</param>
        /// <returns>A new SpriteSequenceAnimationSequence object.</returns>
        public static SpriteSequenceAnimationSequence CreateSpriteSequenceAnimationSequence(VisualElement rootElement)
        {
            return new SpriteSequenceAnimationSequence(rootElement);
        }

        /// <summary>
        /// Represents a sequence of sprite sequence animations for multiple VisualElements.
        /// </summary>
        public class SpriteSequenceAnimationSequence
        {
            private readonly List<SequenceStep> _steps = new List<SequenceStep>();
            private int _currentStep = 0;
            private int _totalLoops = 1;
            private Action _onCompleteOneLoop;
            private Action _onCompleteAllSequences;
            private VisualElement _rootElement;

            public SpriteSequenceAnimationSequence(VisualElement rootElement)
            {
                _rootElement = rootElement;
            }

            /// <summary>
            /// Adds a sprite sequence animation to the sequence.
            /// </summary>
            /// <param name="builder">The SpriteSequenceAnimationBuilder to add.</param>
            /// <returns>The sequence instance for method chaining.</returns>
            public SpriteSequenceAnimationSequence Then(SpriteSequenceAnimationBuilder builder)
            {
                _steps.Add(new SequenceStep(builder));
                return this;
            }

            /// <summary>
            /// Adds a delay to the sequence.
            /// </summary>
            /// <param name="delayMs">The delay in milliseconds.</param>
            /// <returns>The sequence instance for method chaining.</returns>
            public SpriteSequenceAnimationSequence ThenWait(long delayMs)
            {
                _steps.Add(new SequenceStep(delayMs));
                return this;
            }

            /// <summary>
            /// Adds an action to be performed in the sequence.
            /// </summary>
            /// <param name="action">The action to perform.</param>
            /// <returns>The sequence instance for method chaining.</returns>
            public SpriteSequenceAnimationSequence ThenDo(Action action)
            {
                _steps.Add(new SequenceStep(action));
                return this;
            }

            /// <summary>
            /// Sets the number of times the entire sequence should loop.
            /// </summary>
            /// <param name="loops">The number of loops.</param>
            /// <returns>The sequence instance for method chaining.</returns>
            public SpriteSequenceAnimationSequence WithTotalLoops(int loops)
            {
                _totalLoops = loops;
                return this;
            }

            /// <summary>
            /// Sets an action to be performed when one loop of the sequence completes.
            /// </summary>
            /// <param name="action">The action to perform on loop completion.</param>
            /// <returns>The sequence instance for method chaining.</returns>
            public SpriteSequenceAnimationSequence OnCompleteOneLoop(Action action)
            {
                _onCompleteOneLoop = action;
                return this;
            }

            /// <summary>
            /// Sets an action to be performed when all loops of the sequence complete.
            /// </summary>
            /// <param name="action">The action to perform on sequence completion.</param>
            /// <returns>The sequence instance for method chaining.</returns>
            public SpriteSequenceAnimationSequence OnCompleteAllSequences(Action action)
            {
                _onCompleteAllSequences = action;
                return this;
            }

            /// <summary>
            /// Starts the sequence of animations.
            /// </summary>
            /// <returns>The scheduled item for the sequence.</returns>
            public IVisualElementScheduledItem Start()
            {
                _currentStep = 0;
                return PlayNextStep(_totalLoops);
            }

            private IVisualElementScheduledItem PlayNextStep(int remainingLoops)
            {
                if (_currentStep >= _steps.Count)
                {
                    _onCompleteOneLoop?.Invoke();
                    if (remainingLoops > 1 || remainingLoops == -1)
                    {
                        _currentStep = 0;
                        return PlayNextStep(remainingLoops == -1 ? -1 : remainingLoops - 1);
                    }
                    else
                    {
                        _onCompleteAllSequences?.Invoke();
                        return null;
                    }
                }

                var step = _steps[_currentStep];
                _currentStep++;

                if (step.IsDelay)
                {
                    return _rootElement.schedule.Execute(() => PlayNextStep(remainingLoops)).StartingIn(step.DelayMs);
                }
                else if (step.IsAction)
                {
                    step.Action?.Invoke();
                    return PlayNextStep(remainingLoops);
                }
                else
                {
                    return step.Builder.StartInSequence(() => PlayNextStep(remainingLoops));
                }
            }

            private class SequenceStep
            {
                public SpriteSequenceAnimationBuilder Builder { get; }
                public long DelayMs { get; }
                public bool IsDelay { get; }
                public bool IsAction { get; }
                public Action Action { get; }

                public SequenceStep(SpriteSequenceAnimationBuilder builder)
                {
                    Builder = builder;
                    IsDelay = false;
                    IsAction = false;
                }

                public SequenceStep(long delayMs)
                {
                    DelayMs = delayMs;
                    IsDelay = true;
                    IsAction = false;
                }

                public SequenceStep(Action action)
                {
                    Action = action;
                    IsAction = true;
                    IsDelay = false;
                }
            }
        }

        private static IVisualElementScheduledItem AnimateSpriteSequenceInternal(
            VisualElement element,
            Sprite[] sprites,
            int frameDuration,
            int loopCount,
            long delayMs,
            long? totalDurationMs,
            Func<bool> stopCondition,
            Action onComplete,
            Action onCompleteLoop,
            Action<Sprite> applySpriteAction,
            Dictionary<int, Action> frameActions)
        {
            int currentFrame = 0;
            long startTime = 0;
            int currentLoop = 0;

            IVisualElementScheduledItem scheduler = null;
            scheduler = element.schedule.Execute(() =>
            {
                if (startTime == 0)
                {
                    startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                }

                long elapsedTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - startTime;

                if (totalDurationMs.HasValue)
                {
                    currentFrame = (int)((elapsedTime % totalDurationMs.Value) / frameDuration) % sprites.Length;
                }
                else
                {
                    if (currentFrame >= sprites.Length)
                    {
                        currentFrame = 0;
                        currentLoop++;
                        onCompleteLoop?.Invoke();

                        if (loopCount != -1 && currentLoop >= loopCount)
                        {
                            scheduler.Pause();
                            onComplete?.Invoke();
                            return;
                        }
                    }
                }

                applySpriteAction(sprites[currentFrame]);

                // Execute frame-specific action if it exists
                if (frameActions.TryGetValue(currentFrame, out Action frameAction))
                {
                    frameAction.Invoke();
                }

                if (!totalDurationMs.HasValue)
                {
                    currentFrame++;
                }

                if ((totalDurationMs.HasValue && elapsedTime >= totalDurationMs.Value) ||
                    (stopCondition != null && stopCondition()))
                {
                    scheduler.Pause();
                    onComplete?.Invoke();
                }
            });

            if (delayMs > 0)
            {
                scheduler.StartingIn(delayMs);
            }

            if (totalDurationMs.HasValue)
            {
                scheduler.ForDuration(totalDurationMs.Value);
            }

            if (stopCondition != null)
            {
                scheduler.Until(stopCondition);
            }

            scheduler.Every(frameDuration);

            return scheduler;
        }
    }
}