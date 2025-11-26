using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfeditor.Models.Bookmarks;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls
{
	// Token: 0x020001B1 RID: 433
	public partial class BookmarkRenameDialog : Window
	{
		// Token: 0x060018BC RID: 6332 RVA: 0x0005F784 File Offset: 0x0005D984
		private BookmarkRenameDialog(BookmarkModel bookmarkModel, string title)
		{
			this.InitializeComponent();
			this.bookmarkModel = bookmarkModel;
			this.originalTitle = ((bookmarkModel != null) ? bookmarkModel.Title : null) ?? title ?? string.Empty;
			this.TitleTextBox.Text = this.originalTitle;
			if (bookmarkModel == null)
			{
				this.CreateNewTips.Visibility = Visibility.Visible;
				this.RenameTips.Visibility = Visibility.Collapsed;
			}
			else
			{
				this.canBeEmpty = string.IsNullOrEmpty(this.originalTitle);
			}
			this.TitleTextBox.SelectAll();
			this.UpdateButtonState();
		}

		// Token: 0x170009A4 RID: 2468
		// (get) Token: 0x060018BD RID: 6333 RVA: 0x0005F814 File Offset: 0x0005DA14
		// (set) Token: 0x060018BE RID: 6334 RVA: 0x0005F81C File Offset: 0x0005DA1C
		public string BookmarkTitle { get; private set; }

		// Token: 0x060018BF RID: 6335 RVA: 0x0005F828 File Offset: 0x0005DA28
		private async void OKButton_Click(object sender, RoutedEventArgs e)
		{
			base.IsEnabled = false;
			try
			{
				string title = this.TitleTextBox.Text;
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				await requiredService.OperationManager.UpdateBookmarkTitleAsync(requiredService.Document, this.bookmarkModel, title, "");
				this.BookmarkTitle = title;
				base.DialogResult = new bool?(true);
				title = null;
			}
			finally
			{
				base.IsEnabled = false;
			}
		}

		// Token: 0x060018C0 RID: 6336 RVA: 0x0005F85F File Offset: 0x0005DA5F
		private void TitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdateButtonState();
		}

		// Token: 0x060018C1 RID: 6337 RVA: 0x0005F867 File Offset: 0x0005DA67
		private void UpdateButtonState()
		{
			this.OkBtn.IsEnabled = this.canBeEmpty || !string.IsNullOrEmpty(this.TitleTextBox.Text);
		}

		// Token: 0x060018C2 RID: 6338 RVA: 0x0005F892 File Offset: 0x0005DA92
		public static BookmarkRenameDialog Create(BookmarkModel bookmarkModel)
		{
			return new BookmarkRenameDialog(bookmarkModel, null)
			{
				Owner = Application.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};
		}

		// Token: 0x060018C3 RID: 6339 RVA: 0x0005F8B2 File Offset: 0x0005DAB2
		public static BookmarkRenameDialog Create(string title)
		{
			return new BookmarkRenameDialog(null, title)
			{
				Owner = Application.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};
		}

		// Token: 0x04000849 RID: 2121
		private readonly BookmarkModel bookmarkModel;

		// Token: 0x0400084A RID: 2122
		private readonly string originalTitle;

		// Token: 0x0400084B RID: 2123
		private bool canBeEmpty;
	}
}
