using System;
using UnityEngine.UIElements;

namespace RAStudio.UIToolkit.Extensions
{
    public static class VisualElementExtensions
    {
        /// <summary>
        /// Sets the display property of the VisualElement to none.
        /// </summary>
        /// <param name="element">The VisualElement to hide.</param>
        public static void Hide(this VisualElement element)
        {
            element.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Sets the display property of the VisualElement to flex.
        /// </summary>
        /// <param name="element">The VisualElement to show.</param>
        public static void Show(this VisualElement element)
        {
            element.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// Toggles the display property of the VisualElement between none and flex.
        /// </summary>
        /// <param name="element">The VisualElement to toggle.</param>
        public static void ToggleVisibility(this VisualElement element)
        {
            element.style.display = element.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary> 
        /// Adds a class to the VisualElement after a delay.
        /// </summary>
        /// <param name="element">The VisualElement to add the class to.</param>
        /// <param name="className">The name of the class to add.</param>
        /// <param name="delayMs">The delay before the class is added, in milliseconds.</param>
        public static void AddClassWithDelay(this VisualElement element, string className, int delayMs)
        {
            element.schedule.Execute(() => element.AddToClassList(className)).StartingIn(delayMs);
        }

        /// <summary> 
        /// Removes a class from the VisualElement after a delay.
        /// </summary>
        /// <param name="element">The VisualElement to remove the class from.</param>
        /// <param name="className">The name of the class to remove.</param>
        /// <param name="delayMs">The delay before the class is removed, in milliseconds.</param>
        public static void RemoveClassWithDelay(this VisualElement element, string className, int delayMs)
        {
            element.schedule.Execute(() => element.RemoveFromClassList(className)).StartingIn(delayMs);
        }

        /// <summary> 
        /// Sets a style property to the VisualElement after a delay.
        /// </summary>
        /// <param name="element">The VisualElement to apply the style to.</param>
        /// <param name="styleAction">An action that sets the style to be applied.</param>
        /// <param name="delayMs">The delay before the style is applied, in milliseconds.</param>
        public static void SetStyleWithDelay(this VisualElement element, Action<IStyle> styleAction, int delayMs)
        {
            element.schedule.Execute(() => styleAction(element.style)).StartingIn(delayMs);
        }
    }
}
