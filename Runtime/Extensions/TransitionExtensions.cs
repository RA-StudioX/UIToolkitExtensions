using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using System.Linq;

namespace RAStudio.UIToolkit.Extensions
{
    /// <summary>
    /// Provides extension methods for setting up transitions on VisualElements.
    /// </summary>
    public static class TransitionExtensions
    {
        /// <summary>
        /// Default easing mode to use when none is specified.
        /// </summary>
        public const EasingMode DefaultEasingMode = EasingMode.Linear;

        /// <summary>
        /// Sets up a transition for a specific property on a VisualElement and optionally invokes a callback when the transition completes.
        /// </summary>
        /// <param name="element">The VisualElement to set up the transition on.</param>
        /// <param name="propertyName">The name of the property to transition.</param>
        /// <param name="durationMs">The duration of the transition in milliseconds.</param>
        /// <param name="easingMode">The easing mode for the transition. If not provided, uses the default easing mode.</param>
        /// <param name="delayMs">The delay before the transition starts, in milliseconds. Default is 0.</param>
        /// <param name="callback">The action to invoke when the transition ends. Optional.</param>
        public static void SetupTransition(this VisualElement element, string propertyName, int durationMs, EasingMode? easingMode = null, int delayMs = 0, Action callback = null)
        {
            element.style.transitionProperty = new List<StylePropertyName> { new StylePropertyName(propertyName) };
            element.style.transitionDuration = new List<TimeValue> { new TimeValue(durationMs, TimeUnit.Millisecond) };
            element.style.transitionTimingFunction = new List<EasingFunction> { new EasingFunction(easingMode ?? DefaultEasingMode) };
            element.style.transitionDelay = new List<TimeValue> { new TimeValue(delayMs, TimeUnit.Millisecond) };

            if (callback != null)
            {
                EventCallback<TransitionEndEvent> handler = null;
                handler = (evt) =>
                {
                    if (evt.stylePropertyNames.Contains(new StylePropertyName(propertyName)))
                    {
                        callback?.Invoke();
                        element.UnregisterCallback(handler);
                    }
                };
                element.RegisterCallback(handler);
            }
        }


        /// <summary>
        /// Sets up multiple transitions for a VisualElement with flexible configuration options.
        /// Invokes a callback when all transitions are completed.
        /// </summary>
        /// <param name="element">The VisualElement to set up the transitions on.</param>
        /// <param name="transitions">A dictionary where the key is the property name and the value can be one of the following:
        /// - int: Represents the duration in milliseconds.
        /// - (int durationMs, EasingMode easingMode, int delayMs): A tuple with duration, optional easing mode, and optional delay.
        /// - (int durationMs, EasingMode easingMode): A tuple with duration and optional easing mode.
        /// - (int durationMs, int delayMs): A tuple with duration and optional delay.</param>
        /// <param name="callback">The action to invoke when all transitions are completed. Optional.</param>
        /// <exception cref="ArgumentException">Thrown when an invalid value type is provided for a transition.</exception>
        /// <example>
        /// Here are some examples of how to use this method:
        /// <code>
        /// var transitions = new Dictionary<string, object>
        /// {
        ///     { "opacity", 2000 },                                  // Just duration
        ///     { "scale", (1500, EasingMode.EaseOutBounce, 500) },   // Duration, easing, and delay
        ///     { "position", (1000, EasingMode.EaseInOut) },         // Duration and easing
        ///     { "rotation", (800, 200) }                            // Duration and delay
        /// };
        /// 
        /// element.SetupTransitions(transitions, () => Debug.Log("All transitions completed!"));
        /// </code>
        /// </example>
        public static void SetupTransitions(this VisualElement element,
            Dictionary<string, object> transitions,
            Action callback = null)
        {
            var defaultDelay = 0;

            element.style.transitionProperty = new List<StylePropertyName>(transitions.Keys.Select(k => new StylePropertyName(k)));

            var durations = new List<TimeValue>();
            var timingFunctions = new List<EasingFunction>();
            var delays = new List<TimeValue>();

            foreach (var kvp in transitions)
            {
                int durationMs;
                EasingMode? easingMode = null;
                int? delayMs = null;

                if (kvp.Value is int duration)
                {
                    durationMs = duration;
                }
                else if (kvp.Value is ValueTuple<int, EasingMode, int> tuple3)
                {
                    (durationMs, easingMode, delayMs) = tuple3;
                }
                else if (kvp.Value is ValueTuple<int, EasingMode> tuple2Easing)
                {
                    (durationMs, easingMode) = tuple2Easing;
                }
                else if (kvp.Value is ValueTuple<int, int> tuple2Delay)
                {
                    (durationMs, delayMs) = tuple2Delay;
                }
                else
                {
                    throw new ArgumentException($"Invalid value type for key {kvp.Key}");
                }

                durations.Add(new TimeValue(durationMs, TimeUnit.Millisecond));
                timingFunctions.Add(new EasingFunction(easingMode ?? DefaultEasingMode));
                delays.Add(new TimeValue(delayMs ?? defaultDelay, TimeUnit.Millisecond));
            }

            element.style.transitionDuration = durations;
            element.style.transitionTimingFunction = timingFunctions;
            element.style.transitionDelay = delays;

            if (callback != null)
            {
                int totalTransitions = transitions.Count;
                int completedTransitions = 0;

                List<EventCallback<TransitionEndEvent>> handlers = new List<EventCallback<TransitionEndEvent>>();

                foreach (var property in transitions.Keys)
                {
                    EventCallback<TransitionEndEvent> handler = null;
                    handler = (evt) =>
                    {
                        if (!evt.stylePropertyNames.Contains(new StylePropertyName(property)))
                        {
                            return;
                        }

                        completedTransitions++;
                        element.UnregisterCallback(handler);

                        if (completedTransitions < totalTransitions)
                        {
                            return;
                        }

                        callback.Invoke();
                        foreach (var h in handlers)
                        {
                            element.UnregisterCallback(h);
                        }
                    };

                    handlers.Add(handler);
                    element.RegisterCallback(handler);
                }
            }
    }

    /// <summary>
    /// Clears all transitions from the VisualElement.
    /// </summary>
    /// <param name="element">The VisualElement to clear transitions from.</param>
    public static void ClearTransitions(this VisualElement element)
        {
            element.style.transitionProperty = StyleKeyword.Null;
            element.style.transitionDuration = StyleKeyword.Null;
            element.style.transitionTimingFunction = StyleKeyword.Null;
            element.style.transitionDelay = StyleKeyword.Null;
            element.UnregisterCallback<TransitionEndEvent>((evt) => { });
        }
    }
}