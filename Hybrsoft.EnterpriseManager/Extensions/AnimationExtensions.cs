using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Composition;
using Microsoft.Graphics.Canvas.Effects;

namespace Hybrsoft.EnterpriseManager.Extensions
{
	static public class AnimationExtensions
	{
		static public void Grayscale(this UIElement element)
		{
			var brush = CreateGrayscaleEffectBrush();
			element.SetBrush(brush);
		}

		static public void SetBrush(this UIElement element, CompositionBrush brush)
		{
			var spriteVisual = CreateSpriteVisual(element);
			spriteVisual.Brush = brush;
			ElementCompositionPreview.SetElementChildVisual(element, spriteVisual);
		}

		static public void ClearEffects(this UIElement element)
		{
			ElementCompositionPreview.SetElementChildVisual(element, null);
		}

		static public SpriteVisual CreateSpriteVisual(UIElement element)
		{
			return CreateSpriteVisual(ElementCompositionPreview.GetElementVisual(element));
		}
		static public SpriteVisual CreateSpriteVisual(Visual elementVisual)
		{
			var compositor = elementVisual.Compositor;
			var spriteVisual = compositor.CreateSpriteVisual();
			var expression = compositor.CreateExpressionAnimation();
			expression.Expression = "visual.Size";
			expression.SetReferenceParameter("visual", elementVisual);
			spriteVisual.StartAnimation(nameof(Visual.Size), expression);
			return spriteVisual;
		}

		static public CompositionEffectBrush CreateGrayscaleEffectBrush()
		{
			var effect = new GrayscaleEffect
			{
				Name = "Grayscale",
				Source = new CompositionEffectSourceParameter("source")
			};

			var compositor = Window.Current.Compositor;
			var factory = compositor.CreateEffectFactory(effect);
			var brush = factory.CreateBrush();
			brush.SetSourceParameter("source", compositor.CreateBackdropBrush());
			return brush;
		}
	}
}
