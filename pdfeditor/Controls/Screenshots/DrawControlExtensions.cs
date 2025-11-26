using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000200 RID: 512
	public static class DrawControlExtensions
	{
		// Token: 0x06001CE9 RID: 7401 RVA: 0x0007D734 File Offset: 0x0007B934
		public static void Select(this UIElement element)
		{
			if (element == null)
			{
				return;
			}
			Border border = element as Border;
			if (border != null && border.Child is TextBox)
			{
				border.BorderThickness = new Thickness(1.0);
				return;
			}
			AdornerLayer.GetAdornerLayer(element).Add(new SelectedAdorner(element));
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x0007D784 File Offset: 0x0007B984
		public static void UnSelect(this UIElement element)
		{
			if (element == null)
			{
				return;
			}
			Border border = element as Border;
			if (border != null && border.Child is TextBox)
			{
				border.BorderThickness = new Thickness(0.0);
				Keyboard.ClearFocus();
				element.Focus();
				return;
			}
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
			if (adornerLayer != null)
			{
				Adorner[] adorners = adornerLayer.GetAdorners(element);
				if (adorners != null)
				{
					for (int i = adorners.Length - 1; i >= 0; i--)
					{
						adornerLayer.Remove(adorners[i]);
					}
				}
			}
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x0007D7FC File Offset: 0x0007B9FC
		public static void Zoom(this UIElement element, double zoomFactor)
		{
			FrameworkElement frameworkElement = element as FrameworkElement;
			Polyline polyline = frameworkElement as Polyline;
			if (polyline != null)
			{
				for (int i = 0; i < polyline.Points.Count; i++)
				{
					Point point = polyline.Points[i];
					point.X *= zoomFactor;
					point.Y *= zoomFactor;
					polyline.Points[i] = point;
				}
			}
			else
			{
				Border border = frameworkElement as Border;
				if (border == null || !(border.Child is TextBox))
				{
					frameworkElement.Height = frameworkElement.ActualHeight * zoomFactor;
					frameworkElement.Width = frameworkElement.ActualWidth * zoomFactor;
				}
				double left = Canvas.GetLeft(frameworkElement);
				double top = Canvas.GetTop(frameworkElement);
				Canvas.SetLeft(frameworkElement, left * zoomFactor);
				Canvas.SetTop(frameworkElement, top * zoomFactor);
			}
			TransformGroup transformGroup = frameworkElement.RenderTransform as TransformGroup;
			if (transformGroup != null && transformGroup.Children.Count > 0)
			{
				TransformGroup transformGroup2 = new TransformGroup();
				foreach (Transform transform in transformGroup.Children)
				{
					RotateTransform rotateTransform = transform as RotateTransform;
					if (rotateTransform != null)
					{
						transformGroup2.Children.Add(rotateTransform);
					}
					else
					{
						TranslateTransform translateTransform = (TranslateTransform)transform;
						translateTransform.X *= zoomFactor;
						translateTransform.Y *= zoomFactor;
						transformGroup2.Children.Add(translateTransform);
					}
				}
				element.RenderTransform = transformGroup2;
			}
		}
	}
}
