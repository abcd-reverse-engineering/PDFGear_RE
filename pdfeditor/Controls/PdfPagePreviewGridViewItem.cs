using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfeditor.Controls
{
	// Token: 0x020001C6 RID: 454
	public class PdfPagePreviewGridViewItem : PdfPagePreviewListViewItem
	{
		// Token: 0x060019BC RID: 6588 RVA: 0x00066708 File Offset: 0x00064908
		static PdfPagePreviewGridViewItem()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfPagePreviewGridViewItem), new FrameworkPropertyMetadata(typeof(PdfPagePreviewGridViewItem)));
		}

		// Token: 0x060019BD RID: 6589 RVA: 0x0006672D File Offset: 0x0006492D
		public PdfPagePreviewGridViewItem()
		{
			base.Loaded += this.PdfPagePreviewGridViewItem_Loaded;
			base.Unloaded += this.PdfPagePreviewGridViewItem_Unloaded;
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x00066759 File Offset: 0x00064959
		private void PdfPagePreviewGridViewItem_Loaded(object sender, RoutedEventArgs e)
		{
			this.parentListView = ItemsControl.ItemsControlFromItemContainer(this) as PdfPagePreviewGridView;
		}

		// Token: 0x060019BF RID: 6591 RVA: 0x0006676C File Offset: 0x0006496C
		private void PdfPagePreviewGridViewItem_Unloaded(object sender, RoutedEventArgs e)
		{
			this.parentListView = null;
		}

		// Token: 0x060019C0 RID: 6592 RVA: 0x00066775 File Offset: 0x00064975
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseLeftButtonDown(e);
			this.lastClickPos = new Point?(e.GetPosition(this));
			this.lastClickScreenPos = new Point?(base.PointToScreen(this.lastClickPos.Value));
		}

		// Token: 0x060019C1 RID: 6593 RVA: 0x000667AC File Offset: 0x000649AC
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);
			this.lastClickPos = null;
			this.lastClickScreenPos = null;
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x000667D0 File Offset: 0x000649D0
		protected override void OnPreviewMouseMove(MouseEventArgs e)
		{
			base.OnPreviewMouseMove(e);
			if (this.lastClickScreenPos != null && !PdfPagePreviewGridViewItem.draging && e.LeftButton == MouseButtonState.Pressed)
			{
				Point position = e.GetPosition(this);
				Point point = base.PointToScreen(position);
				if (Math.Abs(point.X - this.lastClickScreenPos.Value.X) > 20.0 || Math.Abs(point.Y - this.lastClickScreenPos.Value.Y) > 20.0)
				{
					this.lastClickPos = null;
					this.lastClickScreenPos = null;
					base.IsSelected = true;
					PdfPagePreviewGridView pdfPagePreviewGridView = this.parentListView;
					if (pdfPagePreviewGridView != null)
					{
						pdfPagePreviewGridView.OnItemsDragStart(this);
					}
				}
			}
			if (PdfPagePreviewGridViewItem.draging)
			{
				e.Handled = true;
			}
		}

		// Token: 0x060019C3 RID: 6595 RVA: 0x000668B3 File Offset: 0x00064AB3
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			Panel.SetZIndex(this, 1);
			base.OnMouseEnter(e);
		}

		// Token: 0x060019C4 RID: 6596 RVA: 0x000668C3 File Offset: 0x00064AC3
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			Panel.SetZIndex(this, 0);
			base.OnMouseLeave(e);
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x000668D3 File Offset: 0x00064AD3
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			VisualStateManager.GoToState(this, "FocusBorderInvisible", true);
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x000668EC File Offset: 0x00064AEC
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			if (base.IsMouseOver && (Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed || Mouse.MiddleButton == MouseButtonState.Pressed || Mouse.XButton1 == MouseButtonState.Pressed || Mouse.XButton2 == MouseButtonState.Pressed))
			{
				VisualStateManager.GoToState(this, "FocusBorderInvisible", true);
				return;
			}
			VisualStateManager.GoToState(this, "FocusBorderVisible", true);
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x0006694B File Offset: 0x00064B4B
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);
			VisualStateManager.GoToState(this, "FocusBorderInvisible", true);
		}

		// Token: 0x040008EA RID: 2282
		internal static volatile bool draging;

		// Token: 0x040008EB RID: 2283
		private Point? lastClickPos;

		// Token: 0x040008EC RID: 2284
		private Point? lastClickScreenPos;

		// Token: 0x040008ED RID: 2285
		private PdfPagePreviewGridView parentListView;
	}
}
