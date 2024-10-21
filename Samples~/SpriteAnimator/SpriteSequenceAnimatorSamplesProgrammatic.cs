using UnityEngine;
using UnityEngine.UIElements;
using RAStudio.UIToolkit.Extensions;

namespace RAStudio.UIToolkit.Samples
{
    [RequireComponent(typeof(UIDocument))]
    public class SpriteSequenceAnimatorSamplesProgrammatic : MonoBehaviour
    {
        private UIDocument uiDocument;
        private Sprite[] spriteSequence;
        private SpriteAnimationBuilder controlledAnimationBuilder;
        private SpriteAnimationSequence sequenceAnimation;

        private void Awake()
        {
            // Get or add UIDocument component
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                uiDocument = gameObject.AddComponent<UIDocument>();
            }

            // Create sprite sequence
            spriteSequence = new Sprite[3];
            spriteSequence[0] = CreateCircleSprite(Color.red);
            spriteSequence[1] = CreateSquareSprite(Color.green);
            spriteSequence[2] = CreateTriangleSprite(Color.blue);
        }

        private void Start()
        {
            // Create and set up the UI
            SetupUI();
        }

        private void SetupUI()
        {
            // Create root VisualElement
            VisualElement root = new VisualElement();
            root.style.flexGrow = 1;
            root.style.flexDirection = FlexDirection.Column;

            // Create and add child elements with buttons
            root.Add(CreateAnimationExample("SimpleAnimation", "Simple Animation", StartSimpleAnimation));
            root.Add(CreateAnimationExample("DelayedAnimation", "Delayed Animation", StartDelayedAnimation));
            root.Add(CreateSequenceAnimationExample("SequenceAnimation", "Sequence Animation"));
            root.Add(CreateAnimationExample("CustomAnimation", "Custom Animation", StartCustomAnimation));
            root.Add(CreateAnimationExample("FrameActionAnimation", "Frame Action Animation", StartFrameActionAnimation));
            root.Add(CreateAnimationExample("ErrorHandlingAnimation", "Error Handling Animation", StartErrorHandlingAnimation));
            root.Add(CreateControlledAnimationExample("ControlledAnimation", "Controlled Animation"));

            // Set the root VisualElement
            uiDocument.rootVisualElement.Add(root);
        }

        private VisualElement CreateAnimationExample(string name, string labelText, System.Action<VisualElement> startAction)
        {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.marginBottom = 20;

            Label label = new Label(labelText);
            label.style.width = 150;

            VisualElement animatedElement = new VisualElement();
            animatedElement.name = name;
            animatedElement.style.width = 100;
            animatedElement.style.height = 100;
            animatedElement.style.backgroundColor = Color.gray;

            Button startButton = new Button { text = "Start" };
            startButton.style.width = 100;
            startButton.style.marginLeft = 10;
            startButton.clicked += () => startAction(animatedElement);

            container.Add(label);
            container.Add(animatedElement);
            container.Add(startButton);

            return container;
        }

        private VisualElement CreateSequenceAnimationExample(string name, string labelText)
        {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.marginBottom = 20;

            Label label = new Label(labelText);
            label.style.width = 150;

            VisualElement animatedElement = new VisualElement();
            animatedElement.name = name;
            animatedElement.style.width = 100;
            animatedElement.style.height = 100;
            animatedElement.style.backgroundColor = Color.gray;

            sequenceAnimation = CreateSequenceAnimation(animatedElement);

            Button startButton = new Button { text = "Start" };
            startButton.style.width = 80;
            startButton.style.marginLeft = 10;
            startButton.clicked += () => sequenceAnimation.Start();

            Button stopButton = new Button { text = "Stop" };
            stopButton.style.width = 80;
            stopButton.style.marginLeft = 10;
            stopButton.clicked += () => sequenceAnimation.Stop();

            Button pauseButton = new Button { text = "Pause" };
            pauseButton.style.width = 80;
            pauseButton.style.marginLeft = 10;
            pauseButton.clicked += () => sequenceAnimation.Pause();

            Button resumeButton = new Button { text = "Resume" };
            resumeButton.style.width = 80;
            resumeButton.style.marginLeft = 10;
            resumeButton.clicked += () => sequenceAnimation.Resume();

            container.Add(label);
            container.Add(animatedElement);
            container.Add(startButton);
            container.Add(stopButton);
            container.Add(pauseButton);
            container.Add(resumeButton);

            return container;
        }

        private VisualElement CreateControlledAnimationExample(string name, string labelText)
        {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.marginBottom = 20;

            Label label = new Label(labelText);
            label.style.width = 150;

            VisualElement animatedElement = new VisualElement();
            animatedElement.name = name;
            animatedElement.style.width = 100;
            animatedElement.style.height = 100;
            animatedElement.style.backgroundColor = Color.gray;

            // Create the builder once
            controlledAnimationBuilder = CreateControlledAnimationBuilder(animatedElement);

            Button startButton = new Button { text = "Start" };
            startButton.style.width = 80;
            startButton.style.marginLeft = 10;
            startButton.clicked += () => controlledAnimationBuilder.Start();

            Button stopButton = new Button { text = "Stop" };
            stopButton.style.width = 80;
            stopButton.style.marginLeft = 10;
            stopButton.clicked += () => controlledAnimationBuilder.Stop();

            Button pauseButton = new Button { text = "Pause" };
            pauseButton.style.width = 80;
            pauseButton.style.marginLeft = 10;
            pauseButton.clicked += () => controlledAnimationBuilder.Pause();

            Button resumeButton = new Button { text = "Resume" };
            resumeButton.style.width = 80;
            resumeButton.style.marginLeft = 10;
            resumeButton.clicked += () => controlledAnimationBuilder.Resume();

            container.Add(label);
            container.Add(animatedElement);
            container.Add(startButton);
            container.Add(stopButton);
            container.Add(pauseButton);
            container.Add(resumeButton);

            return container;
        }

        private void StartSimpleAnimation(VisualElement element)
        {
            element.AnimateWithSprites(spriteSequence)
                .WithFrameDuration(500)
                .WithLoop(-1)
                .OnCompleteLoop(() => Debug.Log("Loop completed!"))
                .Start();
        }

        private void StartDelayedAnimation(VisualElement element)
        {
            element.AnimateWithSprites(spriteSequence)
                .WithFrameDuration(500)
                .WithDelay(1000)
                .WithLoop(2)
                .OnComplete(() => Debug.Log("Delayed animation completed!"))
                .OnCompleteLoop(() => Debug.Log("Delayed animation loop completed!"))
                .Start();
        }

        private SpriteAnimationSequence CreateSequenceAnimation(VisualElement element)
        {
            return SpriteAnimator.CreateAnimationSequence(element)
                .Then(element.AnimateWithSprites(spriteSequence).WithFrameDuration(500).WithLoop(1))
                .ThenWait(500)
                .Then(element.AnimateWithSprites(spriteSequence).WithFrameDuration(500).WithLoop(1))
                .ThenDo(() => Debug.Log("Waiting between animations"))
                .ThenWait(1000)
                .Then(element.AnimateWithSprites(spriteSequence).WithFrameDuration(500).WithLoop(1))
                .WithTotalLoops(3)
                .OnCompleteOneLoop(() => Debug.Log("One sequence loop completed!"))
                .OnCompleteAllSequences(() => Debug.Log("All sequences completed!"));
        }

        private void StartCustomAnimation(VisualElement element)
        {
            element.AnimateWithSprites(spriteSequence)
                .WithFrameDuration(500)
                .WithCustomSpriteApplication(sprite =>
                {
                    element.style.backgroundImage = new StyleBackground(sprite);
                    element.style.scale = new StyleScale(new Vector3(1.5f, 1.5f, 1f));
                })
                .WithLoop(1)
                .Start();
        }

        private void StartFrameActionAnimation(VisualElement element)
        {
            element.AnimateWithSprites(spriteSequence)
                .WithFrameDuration(500)
                .WithLoop(2)
                .WithFrameAction(0, () => Debug.Log("Action on frame 0 (Circle)"))
                .WithFrameAction(1, () => Debug.Log("Action on frame 1 (Square)"))
                .WithFrameAction(2, () => Debug.Log("Action on frame 2 (Triangle)"))
                .OnComplete(() => Debug.Log("Frame action animation completed!"))
                .Start();
        }

        private void StartErrorHandlingAnimation(VisualElement element)
        {
            element.AnimateWithSprites(spriteSequence)
                .WithFrameDuration(500)
                .WithLoop(1)
                .WithFrameAction(0, () => Debug.Log("Valid frame action"))
                .WithFrameAction(5, () => Debug.Log("This action won't be added due to invalid frame index"))
                .OnComplete(() => Debug.Log("Error handling animation completed!"))
                .Start();
        }

        private SpriteAnimationBuilder CreateControlledAnimationBuilder(VisualElement element)
        {
            return element.AnimateWithSprites(spriteSequence)
                .WithFrameDuration(500)
                .WithFrameAction(0, () => Debug.Log("Action on frame 0 (Circle)"))
                .WithLoop(-1)
                .OnCompleteLoop(() => Debug.Log("Controlled animation loop completed!"));
        }

        private Sprite CreateCircleSprite(Color color)
        {
            Texture2D texture = new Texture2D(100, 100);
            Vector2 center = new Vector2(50, 50);
            float radius = 40;

            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance <= radius)
                    {
                        texture.SetPixel(x, y, color);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f));
        }

        private Sprite CreateSquareSprite(Color color)
        {
            Texture2D texture = new Texture2D(100, 100);
            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    if (x >= 20 && x < 80 && y >= 20 && y < 80)
                    {
                        texture.SetPixel(x, y, color);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f));
        }

        private Sprite CreateTriangleSprite(Color color)
        {
            Texture2D texture = new Texture2D(100, 100);
            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    if (y <= 2 * x && y <= -2 * x + 200 && y >= 20)
                    {
                        texture.SetPixel(x, y, color);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f));
        }
    }
}