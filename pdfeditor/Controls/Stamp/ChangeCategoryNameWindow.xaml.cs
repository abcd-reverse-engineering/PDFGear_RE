using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using pdfeditor.Properties;

namespace pdfeditor.Controls.Stamp
{
	// Token: 0x020001E7 RID: 487
	public partial class ChangeCategoryNameWindow : Window
	{
		// Token: 0x06001B9C RID: 7068 RVA: 0x00070884 File Offset: 0x0006EA84
		public ChangeCategoryNameWindow(string categoryName = null)
		{
			this.InitializeComponent();
			if (!string.IsNullOrEmpty(categoryName))
			{
				this.CategoryName.Text = categoryName;
				this.CategoryName.Focus();
				base.Dispatcher.BeginInvoke(new Action(delegate
				{
					this.CategoryName.SelectAll();
				}), Array.Empty<object>());
			}
		}

		// Token: 0x06001B9D RID: 7069 RVA: 0x000708DC File Offset: 0x0006EADC
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(this.CategoryName.Text))
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.CategoryNameWinEmptyMessage, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			this.NewCategoryName = this.CategoryName.Text;
			base.DialogResult = new bool?(true);
		}

		// Token: 0x06001B9E RID: 7070 RVA: 0x0007092D File Offset: 0x0006EB2D
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
		}

		// Token: 0x040009DE RID: 2526
		public string NewCategoryName;
	}
}
