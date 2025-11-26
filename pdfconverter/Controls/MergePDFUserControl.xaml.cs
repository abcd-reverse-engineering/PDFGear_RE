using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfconverter.Models;
using pdfconverter.Properties;
using pdfconverter.ViewModels;

namespace pdfconverter.Controls
{
	// Token: 0x020000A3 RID: 163
	public partial class MergePDFUserControl : UserControl
	{
		// Token: 0x06000717 RID: 1815 RVA: 0x00019AF9 File Offset: 0x00017CF9
		public MergePDFUserControl()
		{
			this.InitializeComponent();
			base.DataContext = Ioc.Default.GetRequiredService<MergePDFUCViewModel>();
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x00019B18 File Offset: 0x00017D18
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			GridView gridView = this.lsvFiles.View as GridView;
			double num = 0.0;
			for (int i = gridView.Columns.Count - 1; i > 0; i--)
			{
				num += gridView.Columns[i].ActualWidth;
			}
			gridView.Columns[0].Width = base.ActualWidth - num - 10.0;
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x00019B90 File Offset: 0x00017D90
		private void lsvFiles_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				MergePDFUCViewModel mergePDFUCViewModel = base.DataContext as MergePDFUCViewModel;
				foreach (string text in array)
				{
					if (string.IsNullOrWhiteSpace(text))
					{
						break;
					}
					DocsPathUtils.WriteFilesPathJson("unknow", text, null);
					mergePDFUCViewModel.AddOneFileToMergeList(text, null);
				}
			}
			e.Handled = true;
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x00019C08 File Offset: 0x00017E08
		private void lsvFiles_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Copy;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
			e.Handled = true;
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x00019C34 File Offset: 0x00017E34
		private void FrameworkElement_Drop(object sender, DragEventArgs e)
		{
			if (e.Effects == DragDropEffects.Move)
			{
				MergePDFUCViewModel mergePDFUCViewModel = base.DataContext as MergePDFUCViewModel;
				MergeFileItem mergeFileItem = null;
				if (this.DragData != null)
				{
					mergeFileItem = this.DragData;
				}
				MergeFileItem mergeFileItem2 = (sender as ListViewItem).Content as MergeFileItem;
				mergePDFUCViewModel.changeMergeFileItem(mergeFileItem, mergeFileItem2);
				this.lsvFiles.SelectedItem = mergeFileItem;
				int num = this.lsvFiles.Items.IndexOf(mergeFileItem);
				int num2 = this.lsvFiles.Items.IndexOf(mergeFileItem2);
				if (num > num2)
				{
					if (num < this.lsvFiles.Items.Count - 1)
					{
						this.lsvFiles.ScrollIntoView(this.lsvFiles.Items[num + 1]);
						return;
					}
				}
				else if (num > 0)
				{
					this.lsvFiles.ScrollIntoView(this.lsvFiles.Items[num - 1]);
				}
			}
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x00019D10 File Offset: 0x00017F10
		private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.DragData = null;
			ListViewItem listViewItem = sender as ListViewItem;
			if (listViewItem != null)
			{
				this.DragData = listViewItem.Content as MergeFileItem;
				this.viewItem = listViewItem;
				this.MouseOverIndex = this.lsvFiles.Items.IndexOf(this.DragData);
			}
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x00019D62 File Offset: 0x00017F62
		private void lsvFiles_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.DragData != null && e.LeftButton == MouseButtonState.Pressed)
			{
				base.Dispatcher.BeginInvoke(new Action(delegate
				{
					DragDropAdorner dragDropAdorner = new DragDropAdorner(this.viewItem);
					this.adornerlaryer = AdornerLayer.GetAdornerLayer(this.lsvFiles);
					this.adornerlaryer.Add(dragDropAdorner);
					DragDrop.DoDragDrop(this.lsvFiles, this.DragData, DragDropEffects.Move);
					AdornerLayer adornerLayer = this.adornerlaryer;
					if (adornerLayer != null)
					{
						adornerLayer.Remove(dragDropAdorner);
					}
					this.adornerlaryer = null;
					this.MouseOverIndex = 0;
				}), Array.Empty<object>());
				return;
			}
			this.DragData = null;
			this.viewItem = null;
		}

		// Token: 0x0600071E RID: 1822 RVA: 0x00019DA1 File Offset: 0x00017FA1
		private void ListViewItem_DragOver(object sender, DragEventArgs e)
		{
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x00019DA3 File Offset: 0x00017FA3
		private void TextBox_MouseMove(object sender, MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (this.DragData != null && e.LeftButton == MouseButtonState.Pressed)
			{
				e.Handled = true;
				return;
			}
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x00019DC8 File Offset: 0x00017FC8
		private void TextBox_TextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9.-]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x00019DF4 File Offset: 0x00017FF4
		private void TextBox_Error(object sender, ValidationErrorEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (Validation.GetErrors(textBox).Count<ValidationError>() > 0)
			{
				ModernMessageBox.Show(pdfconverter.Properties.Resources.FileConvertMsgInvaildPageNum, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				textBox.Text = (textBox.DataContext as MergeFileItem).PageFrom.ToString();
				textBox.Focus();
			}
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x00019E50 File Offset: 0x00018050
		private void TextBox_Error_1(object sender, ValidationErrorEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (Validation.GetErrors(textBox).Count<ValidationError>() > 0)
			{
				ModernMessageBox.Show(pdfconverter.Properties.Resources.FileConvertMsgInvaildPageNum, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				textBox.Text = (textBox.DataContext as MergeFileItem).PageTo.ToString();
				textBox.Focus();
			}
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x00019EAB File Offset: 0x000180AB
		private void lsvFiles_PreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			AdornerLayer adornerLayer = this.adornerlaryer;
			if (adornerLayer == null)
			{
				return;
			}
			adornerLayer.Update();
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x00019EC0 File Offset: 0x000180C0
		private void ListViewItem_DragEnter(object sender, DragEventArgs e)
		{
			ListViewItem listViewItem = sender as ListViewItem;
			if (listViewItem != null)
			{
				MergeFileItem mergeFileItem = null;
				if (this.DragData != null)
				{
					mergeFileItem = this.DragData;
				}
				MergeFileItem mergeFileItem2 = listViewItem.Content as MergeFileItem;
				this.lsvFiles.SelectedItem = mergeFileItem2;
				this.lsvFiles.Items.IndexOf(mergeFileItem);
				int num = this.lsvFiles.Items.IndexOf(mergeFileItem2);
				if (num > this.MouseOverIndex && num < this.lsvFiles.Items.Count - 1)
				{
					this.lsvFiles.ScrollIntoView(this.lsvFiles.Items[num + 1]);
					this.MouseOverIndex = num;
					return;
				}
				if (num < this.MouseOverIndex && num > 0)
				{
					this.lsvFiles.ScrollIntoView(this.lsvFiles.Items[num - 1]);
					this.MouseOverIndex = num;
				}
			}
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x00019F9F File Offset: 0x0001819F
		private void ListViewItem_DragLeave(object sender, DragEventArgs e)
		{
			if (sender is ListViewItem)
			{
				this.lsvFiles.SelectedItem = null;
			}
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x0001A0B4 File Offset: 0x000182B4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 5:
			{
				EventSetter eventSetter = new EventSetter();
				eventSetter.Event = UIElement.DropEvent;
				eventSetter.Handler = new DragEventHandler(this.FrameworkElement_Drop);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.PreviewMouseLeftButtonDownEvent;
				eventSetter.Handler = new MouseButtonEventHandler(this.ListViewItem_PreviewMouseLeftButtonDown);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.DragOverEvent;
				eventSetter.Handler = new DragEventHandler(this.ListViewItem_DragOver);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.DragEnterEvent;
				eventSetter.Handler = new DragEventHandler(this.ListViewItem_DragEnter);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.DragLeaveEvent;
				eventSetter.Handler = new DragEventHandler(this.ListViewItem_DragLeave);
				((Style)target).Setters.Add(eventSetter);
				return;
			}
			case 6:
				((TextBox)target).MouseMove += this.TextBox_MouseMove;
				((TextBox)target).PreviewTextInput += this.TextBox_TextInput;
				((TextBox)target).AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(this.TextBox_Error));
				return;
			case 7:
				((TextBox)target).PreviewTextInput += this.TextBox_TextInput;
				((TextBox)target).AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(this.TextBox_Error_1));
				((TextBox)target).MouseMove += this.TextBox_MouseMove;
				return;
			default:
				return;
			}
		}

		// Token: 0x04000379 RID: 889
		private MergeFileItem DragData;

		// Token: 0x0400037A RID: 890
		private ListViewItem viewItem;

		// Token: 0x0400037B RID: 891
		private AdornerLayer adornerlaryer;

		// Token: 0x0400037C RID: 892
		private int MouseOverIndex;
	}
}
