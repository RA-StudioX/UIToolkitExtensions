using UnityEngine;
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
        private Queue<SequenceStep> _originalSteps = new Queue<SequenceStep>();
        private Queue<SequenceStep> _steps = new Queue<SequenceStep>();
        private int _totalLoops = 1;
        private int _currentLoop = 0;
        private Action _onCompleteOneLoop;
        private Action _onCompleteAllSequences;
        private VisualElement _rootElement;
        private bool _isRunning = false;
        private bool _isPaused = false;
        private float _sequenceStartTime;
        private float _pauseStartTime;
        private float _totalPausedTime;

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
            SequenceStep step = new SequenceStep(builder);
            _originalSteps.Enqueue(step);
            return this;
        }

        /// <summary>
        /// Adds a delay to the sequence.
        /// </summary>
        /// <param name="delayMs">The delay in milliseconds.</param>
        /// <returns>The sequence instance for method chaining.</returns>
        public SpriteAnimationSequence ThenWait(long delayMs)
        {
            SequenceStep step = new SequenceStep(delayMs);
            _originalSteps.Enqueue(step);
            return this;
        }

        /// <summary>
        /// Adds an action to be performed in the sequence.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <returns>The sequence instance for method chaining.</returns>
        public SpriteAnimationSequence ThenDo(Action action)
        {
            SequenceStep step = new SequenceStep(action);
            _originalSteps.Enqueue(step);
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
            if (_isRunning && !_isPaused)
            {
                return;
            }

            if (!_isRunning || (_steps.Count == 0 && _currentLoop >= _totalLoops))
            {
                ResetSequence();
            }

            _isRunning = true;
            _isPaused = false;
            _sequenceStartTime = Time.time;
            _totalPausedTime = 0f;
            _rootElement.schedule.Execute(PlayNextStep);
        }

        private void ResetSequence()
        {
            _steps = new Queue<SequenceStep>(_originalSteps);
            _currentLoop = 0;
        }

        /// <summary>
        /// Pauses the sequence of animations.
        /// </summary>
        public void Pause()
        {
            if (_isRunning && !_isPaused)
            {
                _isPaused = true;
                _pauseStartTime = Time.time;
                if (_steps.Count > 0 && !_steps.Peek().IsDelay && !_steps.Peek().IsAction)
                {
                    _steps.Peek().Builder.Pause();
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
                _totalPausedTime += Time.time - _pauseStartTime;
                if (_steps.Count > 0 && !_steps.Peek().IsDelay && !_steps.Peek().IsAction)
                {
                    _steps.Peek().Builder.Resume();
                }
                _rootElement.schedule.Execute(PlayNextStep);
            }
        }

        /// <summary>
        /// Stops the sequence of animations.
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            _isPaused = false;
            foreach (var step in _steps)
            {
                if (!step.IsDelay && !step.IsAction)
                {
                    step.Builder.Stop();
                }
            }
            ResetSequence();
        }

        private void PlayNextStep()
        {
            if (!_isRunning || _isPaused)
            {
                return;
            }

            if (_steps.Count == 0)
            {
                CompleteLoop();
                return;
            }

            var step = _steps.Dequeue();

            if (step.IsDelay)
            {
                float delayEndTime = Time.time + step.DelayMs / 1000f;
                _rootElement.schedule.Execute(() =>
                {
                    if (Time.time >= delayEndTime)
                    {
                        PlayNextStep();
                    }
                    else
                    {
                        _rootElement.schedule.Execute(PlayNextStep);
                    }
                }).StartingIn(16); // Check every frame (assuming 60 FPS)
            }
            else if (step.IsAction)
            {
                step.Action?.Invoke();
                PlayNextStep();
            }
            else
            {
                step.Builder.OnComplete(PlayNextStep);
                step.Builder.Start();
            }
        }

        private void CompleteLoop()
        {
            _onCompleteOneLoop?.Invoke();
            _currentLoop++;
            if (_currentLoop < _totalLoops || _totalLoops == -1)
            {
                ResetSequence();
                PlayNextStep();
            }
            else
            {
                _onCompleteAllSequences?.Invoke();
                _isRunning = false;
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

            public SequenceStep(SpriteAnimationBuilder builder)
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
}