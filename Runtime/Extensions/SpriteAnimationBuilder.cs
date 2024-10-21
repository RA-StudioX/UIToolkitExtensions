using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

namespace RAStudio.UIToolkit.Extensions
{
    /// <summary>
    /// Builder class for configuring and controlling sprite animations.
    /// </summary>
    public class SpriteAnimationBuilder
    {
        public enum AnimationState { Stopped, Running, Paused, Completed }

        private readonly VisualElement _element;
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
        private IVisualElementScheduledItem _internalScheduler;
        private AnimationState _currentState = AnimationState.Stopped;
        private int _currentFrame = 0;
        private int _currentLoop = 0;
        private long _lastUpdateTime = 0;

        public event Action<AnimationState> StateChanged;

        /// <summary>
        /// Initializes a new instance of the SpriteAnimationBuilder class.
        /// </summary>
        /// <param name="element">The VisualElement to animate.</param>
        /// <param name="sprites">An array of sprites to use for the animation frames.</param>
        public SpriteAnimationBuilder(VisualElement element, Sprite[] sprites)
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
        public SpriteAnimationBuilder WithFrameDuration(int duration)
        {
            _frameDuration = duration;
            return this;
        }

        /// <summary>
        /// Sets the number of times the animation should loop. Use -1 for infinite looping.
        /// </summary>
        /// <param name="count">The number of loops, or -1 for infinite looping.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SpriteAnimationBuilder WithLoop(int count = -1)
        {
            _loopCount = count;
            return this;
        }

        /// <summary>
        /// Sets a delay before the animation starts.
        /// </summary>
        /// <param name="delayMs">The delay in milliseconds.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SpriteAnimationBuilder WithDelay(long delayMs)
        {
            _delayMs = delayMs;
            return this;
        }

        /// <summary>
        /// Sets the total duration of the animation.
        /// </summary>
        /// <param name="durationMs">The total duration in milliseconds.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SpriteAnimationBuilder WithTotalDuration(long durationMs)
        {
            _durationMs = durationMs;
            return this;
        }

        /// <summary>
        /// Sets a condition that, when true, will stop the animation.
        /// </summary>
        /// <param name="condition">A function that returns true when the animation should stop.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SpriteAnimationBuilder WithStopCondition(Func<bool> condition)
        {
            _stopCondition = condition;
            return this;
        }

        /// <summary>
        /// Sets an action to be performed when the animation completes.
        /// </summary>
        /// <param name="onComplete">The action to perform on completion.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SpriteAnimationBuilder OnComplete(Action onComplete)
        {
            _onComplete = onComplete;
            return this;
        }

        /// <summary>
        /// Sets an action to be performed when each loop of the animation completes.
        /// </summary>
        /// <param name="onCompleteLoop">The action to perform on loop completion.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SpriteAnimationBuilder OnCompleteLoop(Action onCompleteLoop)
        {
            _onCompleteLoop = onCompleteLoop;
            return this;
        }

        /// <summary>
        /// Sets a custom action for applying sprites to the VisualElement.
        /// </summary>
        /// <param name="applyAction">The custom action to apply sprites.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SpriteAnimationBuilder WithCustomSpriteApplication(Action<Sprite> applyAction)
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
        public SpriteAnimationBuilder WithFrameAction(int frameIndex, Action action)
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
        /// Starts or resumes the animation.
        /// </summary>
        /// <returns>The SpriteAnimationBuilder instance.</returns>
        public SpriteAnimationBuilder Start()
        {
            switch (_currentState)
            {
                case AnimationState.Stopped:
                    _internalScheduler = AnimateInternal();
                    SetState(AnimationState.Running);
                    break;
                case AnimationState.Paused:
                    Resume();
                    break;
            }
            return this;
        }

        /// <summary>
        /// Stops the animation and resets it to the beginning.
        /// </summary>
        public void Stop()
        {
            if (_currentState != AnimationState.Stopped)
            {
                _internalScheduler?.Pause();
                _internalScheduler = null;
                _currentFrame = 0;
                _currentLoop = 0;
                SetState(AnimationState.Stopped);
            }
        }

        /// <summary>
        /// Pauses the animation.
        /// </summary>
        public void Pause()
        {
            if (_currentState == AnimationState.Running)
            {
                _internalScheduler?.Pause();
                SetState(AnimationState.Paused);
            }
        }

        /// <summary>
        /// Resumes the paused animation.
        /// </summary>
        public void Resume()
        {
            if (_currentState == AnimationState.Paused)
            {
                _internalScheduler?.Resume();
                SetState(AnimationState.Running);
            }
        }

        private void SetState(AnimationState newState)
        {
            if (_currentState != newState)
            {
                _currentState = newState;
                StateChanged?.Invoke(_currentState);
            }
        }

        private IVisualElementScheduledItem AnimateInternal()
        {
            _lastUpdateTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            return _element.schedule.Execute(() =>
            {
                if (_currentState != AnimationState.Running) return;

                long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                long elapsedTime = currentTime - _lastUpdateTime;

                if (_durationMs.HasValue)
                {
                    _currentFrame = (int)((elapsedTime % _durationMs.Value) / _frameDuration) % _sprites.Length;
                }
                else
                {
                    if (_currentFrame >= _sprites.Length)
                    {
                        _currentFrame = 0;
                        _currentLoop++;
                        _onCompleteLoop?.Invoke();

                        if (_loopCount != -1 && _currentLoop >= _loopCount)
                        {
                            Complete();
                            return;
                        }
                    }
                }

                _applySpriteAction(_sprites[_currentFrame]);

                if (_frameActions.TryGetValue(_currentFrame, out Action frameAction))
                {
                    frameAction.Invoke();
                }

                if (!_durationMs.HasValue)
                {
                    _currentFrame++;
                }

                if ((_durationMs.HasValue && elapsedTime >= _durationMs.Value) ||
                    (_stopCondition != null && _stopCondition()))
                {
                    Complete();
                }
            }).Every(_frameDuration).StartingIn(_delayMs);
        }

        private void Complete()
        {
            SetState(AnimationState.Completed);
            _internalScheduler?.Pause();
            _onComplete?.Invoke();
        }
    }
}