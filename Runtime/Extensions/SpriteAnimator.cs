using UnityEngine;
using UnityEngine.UIElements;

namespace RAStudio.UIToolkit.Extensions
{
    /// <summary>
    /// Provides methods for animating UI Toolkit elements using sprite sequences.
    /// </summary>
    public static class SpriteAnimator
    {
        /// <summary>
        /// Starts building a sprite sequence animation for a VisualElement.
        /// </summary>
        /// <param name="element">The VisualElement to animate.</param>
        /// <param name="sprites">An array of sprites to use for the animation frames.</param>
        /// <returns>A SpriteAnimationBuilder to configure the animation.</returns>
        public static SpriteAnimationBuilder AnimateWithSprites(this VisualElement element, Sprite[] sprites)
        {
            return new SpriteAnimationBuilder(element, sprites);
        }

        /// <summary>
        /// Creates a new sprite animation sequence.
        /// </summary>
        /// <param name="rootElement">The root VisualElement to use for scheduling.</param>
        /// <returns>A new SpriteAnimationSequence object.</returns>
        public static SpriteAnimationSequence CreateAnimationSequence(VisualElement rootElement)
        {
            return new SpriteAnimationSequence(rootElement);
        }
    }
}