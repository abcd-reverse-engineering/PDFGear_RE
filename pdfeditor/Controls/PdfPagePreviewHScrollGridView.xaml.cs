using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CommonLib.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using pdfeditor.Models.Thumbnails;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls
{
	// Token: 0x020001C7 RID: 455
	public partial class PdfPagePreviewHScrollGridView : PdfPagePreviewListView
	{
		// Token: 0x060019C8 RID: 6600 RVA: 0x00066961 File Offset: 0x00064B61
		static PdfPagePreviewHScrollGridView()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfPagePreviewHScrollGridView), new FrameworkPropertyMetadata(typeof(PdfPagePreviewHScrollGridView)));
		}

		// Token: 0x170009C4 RID: 2500
		// (get) Token: 0x060019C9 RID: 6601 RVA: 0x00066986 File Offset: 0x00064B86
		protected MainViewModel VM
		{
			get
			{
				return Ioc.Default.GetRequiredService<MainViewModel>();
			}
		}

		// Token: 0x060019CA RID: 6602 RVA: 0x00066994 File Offset: 0x00064B94
		public PdfPagePreviewHScrollGridView()
		{
			base.Loaded += this.PdfPagePreviewHScrollGridView_Loaded;
			base.Unloaded += this.PdfPagePreviewHScrollGridView_Unloaded;
			base.PreviewMouseWheel += this.PdfPagePreviewHScrollGridView_PreviewMouseWheel;
			base.SizeChanged += this.PdfPagePreviewHScrollGridView_SizeChanged;
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x000669EF File Offset: 0x00064BEF
		private void PdfPagePreviewHScrollGridView_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				StrongReferenceMessenger.Default.Unregister<ValueChangedMessage<global::System.ValueTuple<int, int>>, string>(this, "MESSAGE_PAGE_EDITOR_SELECT_INDEX");
			}
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x00066A0C File Offset: 0x00064C0C
		private void PdfPagePreviewHScrollGridView_Loaded(object sender, RoutedEventArgs e)
		{
			if (base.IsVisible)
			{
				this.BringSelectedIndexIntoView();
			}
			else
			{
				base.IsVisibleChanged += this.PdfPagePreviewHScrollGridView_IsVisibleChanged;
			}
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				StrongReferenceMessenger.Default.Unregister<ValueChangedMessage<global::System.ValueTuple<int, int>>, string>(this, "MESSAGE_PAGE_EDITOR_SELECT_INDEX");
				StrongReferenceMessenger.Default.Register(this, "MESSAGE_PAGE_EDITOR_SELECT_INDEX", new MessageHandler<object, ValueChangedMessage<global::System.ValueTuple<int, int>>>(this.OnSelectIndexChangeNotified));
			}
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x00066A70 File Offset: 0x00064C70
		private void OnSelectIndexChangeNotified(object recipient, [global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "startPage", "endPage" })] ValueChangedMessage<global::System.ValueTuple<int, int>> message)
		{
			global::System.ValueTuple<int, int> value = message.Value;
			int startPage = value.Item1;
			int endPage = value.Item2;
			if (startPage < 0 || endPage < startPage || endPage >= this.VM.ThumbnailItemSource.Count<PdfThumbnailModel>())
			{
				return;
			}
			Func<PdfThumbnailModel, bool> <>9__1;
			base.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
			{
				if (!this.IsLoaded)
				{
					return;
				}
				this.SelectedItems.Clear();
				IEnumerable<PdfThumbnailModel> thumbnailItemSource = this.VM.ThumbnailItemSource;
				Func<PdfThumbnailModel, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (PdfThumbnailModel model) => model.PageIndex >= startPage && model.PageIndex <= endPage);
				}
				IEnumerable<PdfThumbnailModel> enumerable = thumbnailItemSource.Where(func);
				this.SetSelectedItems(enumerable);
				this.BringSelectedIndexIntoView();
			}));
		}

		// Token: 0x060019CE RID: 6606 RVA: 0x00066AF2 File Offset: 0x00064CF2
		private void PdfPagePreviewHScrollGridView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (base.IsVisible)
			{
				base.IsVisibleChanged -= this.PdfPagePreviewHScrollGridView_IsVisibleChanged;
				this.BringSelectedIndexIntoView();
			}
		}

		// Token: 0x060019CF RID: 6607 RVA: 0x00066B14 File Offset: 0x00064D14
		private void PdfPagePreviewHScrollGridView_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (this == null)
			{
				return;
			}
			this.BringSelectedIndexIntoView();
		}

		// Token: 0x060019D0 RID: 6608 RVA: 0x00066B20 File Offset: 0x00064D20
		private void PdfPagePreviewHScrollGridView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				e.Handled = true;
				this.ScaleItems(e.Delta > 0);
				e.Handled = true;
			}
		}

		// Token: 0x060019D1 RID: 6609 RVA: 0x00066B47 File Offset: 0x00064D47
		private void ScaleItems(bool isZoomIn)
		{
			if (isZoomIn)
			{
				this.VM.ThumbnailZoonIn.Execute(null);
				return;
			}
			this.VM.ThumbnailZoonOut.Execute(null);
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x00066B6F File Offset: 0x00064D6F
		public void BringSelectedIndexIntoView()
		{
			base.Dispatcher.BeginInvoke(new Action(delegate
			{
				int selectedIndex = base.SelectedIndex;
				if (selectedIndex == -1 || selectedIndex > this.VM.TotalPagesCount - 1)
				{
					return;
				}
				VirtualThumbnailPanel virtualThumbnailPanel = this.GetItemsHost() as VirtualThumbnailPanel;
				if (virtualThumbnailPanel != null)
				{
					virtualThumbnailPanel.InvalidateMeasure();
					virtualThumbnailPanel.UpdateLayout();
					virtualThumbnailPanel.BringIndexIntoViewPublic(selectedIndex);
				}
			}), DispatcherPriority.Render, Array.Empty<object>());
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x00066B90 File Offset: 0x00064D90
		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			foreach (object obj in ((IEnumerable)base.Items))
			{
				PdfPagePreviewHScrollGridViewItem pdfPagePreviewHScrollGridViewItem = base.ItemContainerGenerator.ContainerFromItem(obj) as PdfPagePreviewHScrollGridViewItem;
				if (pdfPagePreviewHScrollGridViewItem != null)
				{
					pdfPagePreviewHScrollGridViewItem.IsSelectedIndex = pdfPagePreviewHScrollGridViewItem.IsSelected && base.Items.IndexOf(obj) == base.SelectedIndex;
				}
			}
		}

		// Token: 0x060019D4 RID: 6612 RVA: 0x00066C20 File Offset: 0x00064E20
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			PdfPagePreviewHScrollGridViewItem pdfPagePreviewHScrollGridViewItem = element as PdfPagePreviewHScrollGridViewItem;
			if (pdfPagePreviewHScrollGridViewItem != null)
			{
				pdfPagePreviewHScrollGridViewItem.IsSelectedIndex = base.Items.IndexOf(item) == base.SelectedIndex;
			}
		}

		// Token: 0x060019D5 RID: 6613 RVA: 0x00066C59 File Offset: 0x00064E59
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new PdfPagePreviewHScrollGridViewItem();
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x00066C60 File Offset: 0x00064E60
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is PdfPagePreviewHScrollGridViewItem;
		}
	}
}
