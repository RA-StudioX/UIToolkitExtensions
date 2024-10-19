using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using System;

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

            // If a callback is provided, register the TransitionEndEvent
            if (callback != null)
            {
                element.RegisterCallback<TransitionEndEvent>(evt =>
                {
                    if (evt.stylePropertyNames.Contains(new StylePropertyName(propertyName)))
                    {
                        callback?.Invoke();
                        // Cleanup listener after the callback is invoked
                        element.UnregisterCallback<TransitionEndEvent>(null);
                    }
                });
            }
        }

        /// <summary>
        /// Sets up multiple transitions for a VisualElement with individual durations, optional easing modes, and optional delays.
        /// Invokes a callback when all transitions are completed.
        /// </summary>
        /// <param name="element">The VisualElement to set up the transitions on.</param>
        /// <param name="transitions">A dictionary where the key is the property name and the value is a tuple containing:
        /// - durationMs (required): The duration of the transition in milliseconds.
        /// - easingMode (optional): The easing mode for the transition. If not provided, uses the default easing mode.
        /// - delayMs (optional): The delay before the transition starts, in milliseconds. If not provided, defaults to 0.</param>
        /// <param name="callback">The action to invoke when all transitions are completed. Optional.</param>
        public static void SetupTransitions(this VisualElement element,
            Dictionary<string, (int durationMs, EasingMode? easingMode, int? delayMs)> transitions,
            Action callback = null)
        {
            int totalTransitions = transitions.Count;
            int completedTransitions = 0;

            // Setup the transitions
            element.style.transitionProperty = new List<StylePropertyName>(transitions.Keys.Select(k => new StylePropertyName(k)));
            element.style.transitionDuration = new List<TimeValue>(transitions.Values.Select(v => new TimeValue(v.durationMs, TimeUnit.Millisecond)));
            element.style.transitionTimingFunction = new List<EasingFunction>(transitions.Values.Select(v => new EasingFunction(v.easingMode ?? DefaultEasingMode)));
            element.style.transitionDelay = new List<TimeValue>(transitions.Values.Select(v => new TimeValue(v.delayMs ?? 0, TimeUnit.Millisecond)));

            // If a callback is provided, register the TransitionEndEvent
            if (callback != null)
            {
                // Register a callback for each transition
                foreach (var property in transitions.Keys)
                {
                    element.RegisterCallback<TransitionEndEvent>(evt =>
                    {
                        if (evt.stylePropertyNames.Contains(new StylePropertyName(property)))
                        {
                            completedTransitions++;

                            // If all transitions are completed, invoke the callback
                            if (completedTransitions >= totalTransitions)
                            {
                                callback.Invoke();
                                // Cleanup listener after the callback is invoked
                                element.UnregisterCallback<TransitionEndEvent>(null);
                            }
                        }
                    });
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
            element.UnregisterCallback<TransitionEndEvent>(null);
        }
    }
}