using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using pdfeditor.Models.Thumbnails;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.Utils.Behaviors;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x0200025D RID: 605
	public partial class SelectPageComboBox : UserControl
	{
		// Token: 0x060022F9 RID: 8953 RVA: 0x000A5035 File Offset: 0x000A3235
		public SelectPageComboBox()
		{
			this.InitializeComponent();
		}

		// Token: 0x060022FA RID: 8954 RVA: 0x000A5043 File Offset: 0x000A3243
		private void _SelectPageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.ApplyComboBoxSelectedItem();
			this._SelectPageComboBox.SelectedIndex = -1;
		}

		// Token: 0x060022FB RID: 8955 RVA: 0x000A5057 File Offset: 0x000A3257
		private void _TextBoxEditBehavior_TextChanged(object sender, EventArgs e)
		{
			if (this.innerSet)
			{
				return;
			}
			this.ApplyInput();
		}

		// Token: 0x060022FC RID: 8956 RVA: 0x000A5068 File Offset: 0x000A3268
		private void _ArrowButton_Click(object sender, RoutedEventArgs e)
		{
			this._SelectPageComboBox.IsDropDownOpen = true;
		}

		// Token: 0x17000B3A RID: 2874
		// (get) Token: 0x060022FD RID: 8957 RVA: 0x000A5076 File Offset: 0x000A3276
		// (set) Token: 0x060022FE RID: 8958 RVA: 0x000A5088 File Offset: 0x000A3288
		public PdfPageEditList Pages
		{
			get
			{
				return (PdfPageEditList)base.GetValue(SelectPageComboBox.PagesProperty);
			}
			set
			{
				base.SetValue(SelectPageComboBox.PagesProperty, value);
			}
		}

		// Token: 0x060022FF RID: 8959 RVA: 0x000A5098 File Offset: 0x000A3298
		private static void OnPagesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SelectPageComboBox selectPageComboBox = d as SelectPageComboBox;
			if (selectPageComboBox != null && !object.Equals(e.NewValue, e.OldValue))
			{
				INotifyPropertyChanged notifyPropertyChanged = e.OldValue as INotifyPropertyChanged;
				if (notifyPropertyChanged != null)
				{
					WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(notifyPropertyChanged, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(selectPageComboBox.OnPagesPropertyChanged));
				}
				INotifyPropertyChanged notifyPropertyChanged2 = e.NewValue as INotifyPropertyChanged;
				if (notifyPropertyChanged2 != null)
				{
					WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(notifyPropertyChanged2, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(selectPageComboBox.OnPagesPropertyChanged));
				}
				selectPageComboBox.UpdateDisplayText();
			}
		}

		// Token: 0x17000B3B RID: 2875
		// (get) Token: 0x06002300 RID: 8960 RVA: 0x000A5118 File Offset: 0x000A3318
		// (set) Token: 0x06002301 RID: 8961 RVA: 0x000A512A File Offset: 0x000A332A
		public PdfPagePreviewGridView PreviewGridView
		{
			get
			{
				return (PdfPagePreviewGridView)base.GetValue(SelectPageComboBox.PreviewGridViewProperty);
			}
			set
			{
				base.SetValue(SelectPageComboBox.PreviewGridViewProperty, value);
			}
		}

		// Token: 0x06002302 RID: 8962 RVA: 0x000A5138 File Offset: 0x000A3338
		private void OnPagesPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (this.innerSet)
			{
				return;
			}
			this.UpdateDisplayText();
		}

		// Token: 0x06002303 RID: 8963 RVA: 0x000A514C File Offset: 0x000A334C
		private void UpdateDisplayText()
		{
			try
			{
				this.innerSet = true;
				if (this.Pages == null || this.Pages.Count == 0 || this.Pages.SelectedItems.Count == 0)
				{
					this._TextBoxEditBehavior.Text = "";
				}
				else if (this.Pages.AllItemSelected.GetValueOrDefault())
				{
					int displayPageIndex = this.Pages[this.Pages.Count - 1].DisplayPageIndex;
					if (displayPageIndex == 1)
					{
						this._TextBoxEditBehavior.Text = "1";
					}
					else
					{
						this._TextBoxEditBehavior.Text = string.Format("1-{0}", displayPageIndex);
					}
				}
				else
				{
					IEnumerable<int> enumerable = this.Pages.SelectedItems.Select((PdfPageEditListModel c) => c.PageIndex);
					this._TextBoxEditBehavior.Text = enumerable.ConvertToRange();
				}
			}
			finally
			{
				this.innerSet = false;
			}
		}

		// Token: 0x06002304 RID: 8964 RVA: 0x000A5260 File Offset: 0x000A3460
		private void ApplyInput()
		{
			if (this.Pages == null || this.Pages.Count == 0)
			{
				return;
			}
			try
			{
				this.innerSet = true;
				string text = this._TextBoxEditBehavior.Text.Replace("，", ",");
				if (string.IsNullOrEmpty(text))
				{
					this.Pages.AllItemSelected = new bool?(false);
				}
				else if (!new HashSet<string>((from c in (from c in this._SelectPageComboBox.Items.OfType<ComboBoxItem>()
						select c.Tag).OfType<string>()
					where !string.IsNullOrEmpty(c)
					select c).Distinct<string>()).Contains(text))
				{
					int[] array;
					int num;
					if (PdfObjectExtensions.TryParsePageRange(text, out array, out num))
					{
						GAManager.SendEvent("PageView", "SelectPage", "CustomPages", 1L);
						this.Pages.AllItemSelected = new bool?(false);
						int count = this.Pages.Count;
						int num2 = -1;
						foreach (int num3 in array)
						{
							if (num3 >= 0 && num3 < count)
							{
								if (num2 == -1)
								{
									num2 = num3;
								}
								if (this.IsItemInPreviewViewport(this.Pages[num3]) == SelectPageComboBox.ItemViewportRelationship.Contains)
								{
									num2 = -2;
								}
								this.Pages[num3].Selected = true;
							}
						}
						if (this.PreviewGridView != null && num2 >= 0)
						{
							this.BringIndexIntoGridViewViewport(num2);
						}
					}
				}
			}
			finally
			{
				this.innerSet = false;
			}
		}

		// Token: 0x06002305 RID: 8965 RVA: 0x000A5418 File Offset: 0x000A3618
		private void ApplyComboBoxSelectedItem()
		{
			if (this.Pages == null || this.Pages.Count == 0)
			{
				return;
			}
			try
			{
				this.innerSet = true;
				ComboBoxItem comboBoxItem = (ComboBoxItem)this._SelectPageComboBox.SelectedItem;
				if (comboBoxItem != null)
				{
					string text = comboBoxItem.Tag as string;
					if (!(text == "UnselectAll"))
					{
						if (!(text == "AllPages"))
						{
							if (!(text == "AllEvenPages"))
							{
								if (text == "AllOddPages")
								{
									this._TextBoxEditBehavior.Text = pdfeditor.Properties.Resources.SelectPageAllOddPagesItem;
									this.Pages.AllItemSelected = new bool?(false);
									int count = this.Pages.Count;
									for (int i = 0; i < count; i++)
									{
										if (i % 2 == 0)
										{
											this.Pages[i].Selected = true;
										}
									}
									GAManager.SendEvent("PageView", "SelectPage", "AllOddPages", 1L);
								}
							}
							else
							{
								this._TextBoxEditBehavior.Text = pdfeditor.Properties.Resources.SelectPageAllEvenPagesItem;
								this.Pages.AllItemSelected = new bool?(false);
								int count2 = this.Pages.Count;
								for (int j = 0; j < count2; j++)
								{
									if (j % 2 == 1)
									{
										this.Pages[j].Selected = true;
									}
								}
								GAManager.SendEvent("PageView", "SelectPage", "AllEvenPages", 1L);
							}
						}
						else
						{
							this._TextBoxEditBehavior.Text = pdfeditor.Properties.Resources.SelectPageAllPagesItem;
							this.Pages.AllItemSelected = new bool?(true);
							GAManager.SendEvent("PageView", "SelectPage", "AllPages", 1L);
						}
					}
					else
					{
						this._TextBoxEditBehavior.Text = pdfeditor.Properties.Resources.SelectPageUnselectAllItem;
						this.Pages.AllItemSelected = new bool?(false);
						GAManager.SendEvent("PageView", "SelectPage", "UnselectAll", 1L);
					}
				}
			}
			finally
			{
				this.innerSet = false;
			}
		}

		// Token: 0x06002306 RID: 8966 RVA: 0x000A561C File Offset: 0x000A381C
		private SelectPageComboBox.ItemViewportRelationship IsItemInPreviewViewport(PdfPageEditListModel item)
		{
			PdfPagePreviewGridView previewGridView = this.PreviewGridView;
			if (previewGridView == null || !previewGridView.IsLoaded)
			{
				return SelectPageComboBox.ItemViewportRelationship.None;
			}
			PdfPagePreviewGridViewItem pdfPagePreviewGridViewItem = (PdfPagePreviewGridViewItem)previewGridView.ItemContainerGenerator.ContainerFromItem(item);
			if (pdfPagePreviewGridViewItem == null)
			{
				return SelectPageComboBox.ItemViewportRelationship.None;
			}
			Rect rect = new Rect(0.0, 0.0, previewGridView.ActualWidth, previewGridView.ActualHeight);
			Rect rect2 = pdfPagePreviewGridViewItem.TransformToVisual(previewGridView).TransformBounds(new Rect(0.0, 0.0, pdfPagePreviewGridViewItem.ActualWidth, pdfPagePreviewGridViewItem.ActualHeight));
			if (!rect.IntersectsWith(rect2))
			{
				return SelectPageComboBox.ItemViewportRelationship.None;
			}
			if (rect.Contains(rect2))
			{
				return SelectPageComboBox.ItemViewportRelationship.Contains;
			}
			return SelectPageComboBox.ItemViewportRelationship.IntersectsWith;
		}

		// Token: 0x06002307 RID: 8967 RVA: 0x000A56C4 File Offset: 0x000A38C4
		private bool BringIndexIntoGridViewViewport(int index)
		{
			PdfPagePreviewGridView previewGridView = this.PreviewGridView;
			PdfPageEditList pages = this.Pages;
			if (previewGridView == null || pages == null || index < 0 || index >= pages.Count)
			{
				return false;
			}
			previewGridView.ScrollIntoView(pages[index]);
			return false;
		}

		// Token: 0x04000EF2 RID: 3826
		private bool innerSet;

		// Token: 0x04000EF3 RID: 3827
		public static readonly DependencyProperty PagesProperty = DependencyProperty.Register("Pages", typeof(PdfPageEditList), typeof(SelectPageComboBox), new PropertyMetadata(null, new PropertyChangedCallback(SelectPageComboBox.OnPagesPropertyChanged)));

		// Token: 0x04000EF4 RID: 3828
		public static readonly DependencyProperty PreviewGridViewProperty = DependencyProperty.Register("PreviewGridView", typeof(PdfPagePreviewGridView), typeof(SelectPageComboBox), new PropertyMetadata(null));

		// Token: 0x02000721 RID: 1825
		private enum ItemViewportRelationship
		{
			// Token: 0x04002454 RID: 9300
			None,
			// Token: 0x04002455 RID: 9301
			IntersectsWith,
			// Token: 0x04002456 RID: 9302
			Contains
		}
	}
}
