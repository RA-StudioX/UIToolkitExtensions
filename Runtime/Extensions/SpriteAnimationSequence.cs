using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

namespace RAStudio.UIToolkit.Extensions
{
    /// <summary>
    /// Represents a sequence of sprite animations for multiple VisualElements.
    /// </summary>
    public class SpriteAnimationSequence
    {
        private readonly List<SequenceStep> _steps = new List<SequenceStep>();
        private int _currentStep = 0;
        private int _totalLoops = 1;
        private Action _onCompleteOneLoop;
        private Action _onCompleteAllSequences;
        private VisualElement _rootElement;
        private bool _isRunning = false;
        private bool _isPaused = false;

        /// <summary>
        /// Initializes a new instance of the SpriteAnimationSequence class.
        /// </summary>
        /// <param name="rootElement">The root VisualElement to use for scheduling.</param>
        public SpriteAnimationSequence(VisualElement rootElement)
        {
            _rootElement = rootElement;
        }

        /// <summary>
        /// Adds a sprite animation to the sequence.
        /// </summary>
        /// <param name="builder">The SpriteAnimationBuilder to add.</param>
        /// <returns>The sequence instance for method chaining.</returns>
        public SpriteAnimationSequence Then(SpriteAnimationBuilder builder)
        {
            _steps.Add(new SequenceStep(builder));
            return this;
        }

        /// <summary>
        /// Adds a delay to the sequence.
        /// </summary>
        /// <param name="delayMs">The delay in milliseconds.</param>
        /// <returns>The sequence instance for method chaining.</returns>
        public SpriteAnimationSequence ThenWait(long delayMs)
        {
            _steps.Add(new SequenceStep(delayMs));
            return this;
        }

        /// <summary>
        /// Adds an action to be performed in the sequence.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <returns>The sequence instance for method chaining.</returns>
        public SpriteAnimationSequence ThenDo(Action action)
        {
            _steps.Add(new SequenceStep(action));
            return this;
        }

        /// <summary>
        /// Sets the number of times the entire sequence should loop.
        /// </summary>
        /// <param name="loops">The number of loops.</param>
        /// <returns>The sequence instance for method chaining.</returns>
        public SpriteAnimationSequence WithTotalLoops(int loops)
        {
            _totalLoops = loops;
            return this;
        }

        /// <summary>
        /// Sets an action to be performed when one loop of the sequence completes.
        /// </summary>
        /// <param name="action">The action to perform on loop completion.</param>
        /// <returns>The sequence instance for method chaining.</returns>
        public SpriteAnimationSequence OnCompleteOneLoop(Action action)
        {
            _onCompleteOneLoop = action;
            return this;
        }

        /// <summary>
        /// Sets an action to be performed when all loops of the sequence complete.
        /// </summary>
        /// <param name="action">The action to perform on sequence completion.</param>
        /// <returns>The sequence instance for method chaining.</returns>
        public SpriteAnimationSequence OnCompleteAllSequences(Action action)
        {
            _onCompleteAllSequences = action;
            return this;
        }

        /// <summary>
        /// Starts the sequence of animations.
        /// </summary>
        public void Start()
        {
            if (_isRunning)
            {
                return;
            }
            _isRunning = true;
            _isPaused = false;
            _currentStep = 0;
            PlayNextStep(_totalLoops);
        }

        /// <summary>
        /// Pauses the sequence of animations.
        /// </summary>
        public void Pause()
        {
            if (_isRunning && !_isPaused)
            {
                _isPaused = true;
                if (_currentStep < _steps.Count && !_steps[_currentStep].IsDelay && !_steps[_currentStep].IsAction)
                {
                    _steps[_currentStep].Builder.Pause();
                }
            }
        }

        /// <summary>
        /// Resumes the paused sequence of animations.
        /// </summary>
        public void Resume()
        {
            if (_isRunning && _isPaused)
            {
                _isPaused = false;
                if (_currentStep < _steps.Count && !_steps[_currentStep].IsDelay && !_steps[_currentStep].IsAction)
                {
                    _steps[_currentStep].Builder.Resume();
                }
                else
                {
                    PlayNextStep(_totalLoops);
                }
            }
        }

        /// <summary>
        /// Stops the sequence of animations.
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            _isPaused = false;
            _currentStep = 0;
            foreach (var step in _steps)
            {
                if (!step.IsDelay && !step.IsAction)
                {
                    step.Builder.Stop();
                }
            }
        }

        private void PlayNextStep(int remainingLoops)
        {
            if (!_isRunning || _isPaused)
            {
                return;
            }

            if (_currentStep >= _steps.Count)
            {
                _onCompleteOneLoop?.Invoke();
                if (remainingLoops > 1 || remainingLoops == -1)
                {
                    _currentStep = 0;
                    PlayNextStep(remainingLoops == -1 ? -1 : remainingLoops - 1);
                }
                else
                {
                    _onCompleteAllSequences?.Invoke();
                    _isRunning = false;
                }
                return;
            }

            var step = _steps[_currentStep];
            _currentStep++;

            if (step.IsDelay)
            {
                _rootElement.schedule.Execute(() => PlayNextStep(remainingLoops)).StartingIn(step.DelayMs);
            }
            else if (step.IsAction)
            {
                step.Action?.Invoke();
                PlayNextStep(remainingLoops);
            }
            else
            {
                step.Builder.OnComplete(() => PlayNextStep(remainingLoops));
                step.Builder.Start();
            }
        }


        /// <summary>
        /// Represents a single step in the animation sequence.
        /// </summary>
        private class SequenceStep
        {
            /// <summary>
            /// Gets the SpriteAnimationBuilder for this step.
            /// </summary>
            public SpriteAnimationBuilder Builder { get; }

            /// <summary>
            /// Gets the delay in milliseconds for this step.
            /// </summary>
            public long DelayMs { get; }

            /// <summary>
            /// Gets a value indicating whether this step is a delay.
            /// </summary>
            public bool IsDelay { get; }

            /// <summary>
            /// Gets a value indicating whether this step is an action.
            /// </summary>
            public bool IsAction { get; }

            /// <summary>
            /// Gets the action to be performed in this step.
            /// </summary>
            public Action Action { get; }

            /// <summary>
            /// Initializes a new instance of the SequenceStep class for an animation.
            /// </summary>
            /// <param name="builder">The SpriteAnimationBuilder for this step.</param>
            public SequenceStep(SpriteAnimationBuilder builder)
            {
                Builder = builder;
                IsDelay = false;
                IsAction = false;
            }

            /// <summary>
            /// Initializes a new instance of the SequenceStep class for a delay.
            /// </summary>
            /// <param name="delayMs">The delay in milliseconds.</param>
            public SequenceStep(long delayMs)
            {
                DelayMs = delayMs;
                IsDelay = true;
                IsAction = false;
            }

            /// <summary>
            /// Initializes a new instance of the SequenceStep class for an action.
            /// </summary>
            /// <param name="action">The action to be performed.</param>
            public SequenceStep(Action action)
            {
                Action = action;
                IsAction = true;
                IsDelay = false;
            }
        }
    }
}