using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace pdfeditor.Controls
{
	// Token: 0x020001BE RID: 446
	public partial class PathTextBox : TextBox
	{
		// Token: 0x06001974 RID: 6516 RVA: 0x00064EAE File Offset: 0x000630AE
		static PathTextBox()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PathTextBox), new FrameworkPropertyMetadata(typeof(PathTextBox)));
			PathTextBox.InvalidPathChars = Path.GetInvalidPathChars().Distinct<char>().ToImmutableHashSet<char>();
		}

		// Token: 0x170009B6 RID: 2486
		// (get) Token: 0x06001975 RID: 6517 RVA: 0x00064EE7 File Offset: 0x000630E7
		// (set) Token: 0x06001976 RID: 6518 RVA: 0x00064EF0 File Offset: 0x000630F0
		private Button BrowserButton
		{
			get
			{
				return this.browserButton;
			}
			set
			{
				if (this.browserButton != value)
				{
					if (this.browserButton != null)
					{
						this.browserButton.Click -= this.BrowserButton_Click;
					}
					this.browserButton = value;
					if (this.browserButton != null)
					{
						this.browserButton.Click += this.BrowserButton_Click;
					}
				}
			}
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x00064F4B File Offset: 0x0006314B
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.BrowserButton = base.GetTemplateChild("BrowserButton") as Button;
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x00064F69 File Offset: 0x00063169
		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			base.OnTextChanged(e);
			if (!base.IsFocused)
			{
				this.TryRaisePathChanged();
			}
		}

		// Token: 0x06001979 RID: 6521 RVA: 0x00064F80 File Offset: 0x00063180
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);
			if (Keyboard.FocusedElement != this.BrowserButton)
			{
				this.TryRaisePathChanged();
			}
		}

		// Token: 0x0600197A RID: 6522 RVA: 0x00064F9C File Offset: 0x0006319C
		private void BrowserButton_Click(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			this.RequestedDialog();
		}

		// Token: 0x0600197B RID: 6523 RVA: 0x00064FAC File Offset: 0x000631AC
		protected bool RequestedDialog()
		{
			string text = base.Text;
			string text2;
			string text3;
			if (!this.TryGetDirectoryAndFile(text, out text2, out text3))
			{
				text2 = "";
				text3 = "";
			}
			object obj = this.CreateDialog(text2, text3);
			if (obj == null)
			{
				return false;
			}
			FileDialog fileDialog = obj as FileDialog;
			if (fileDialog != null)
			{
				bool? flag = null;
				Window window = Window.GetWindow(this);
				if (window.IsVisible)
				{
					flag = fileDialog.ShowDialog(window);
				}
				else
				{
					flag = fileDialog.ShowDialog();
				}
				if (flag.GetValueOrDefault())
				{
					base.Text = fileDialog.FileName;
				}
				return flag.GetValueOrDefault();
			}
			CommonFileDialog commonFileDialog = obj as CommonFileDialog;
			if (commonFileDialog != null)
			{
				Window window2 = Window.GetWindow(this);
				CommonFileDialogResult commonFileDialogResult;
				if (window2.IsVisible)
				{
					commonFileDialogResult = commonFileDialog.ShowDialog(window2);
				}
				else
				{
					commonFileDialogResult = commonFileDialog.ShowDialog();
				}
				if (commonFileDialogResult == CommonFileDialogResult.Ok)
				{
					base.Text = commonFileDialog.FileName;
				}
				return commonFileDialogResult == CommonFileDialogResult.Ok;
			}
			return false;
		}

		// Token: 0x0600197C RID: 6524 RVA: 0x00065090 File Offset: 0x00063290
		private bool TryGetDirectoryAndFile(string text, out string directory, out string filename)
		{
			directory = "";
			filename = "";
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			for (int i = 0; i < text.Length; i++)
			{
				if (PathTextBox.InvalidPathChars.Contains(text[i]))
				{
					return false;
				}
			}
			if (text[text.Length - 1] == Path.DirectorySeparatorChar)
			{
				try
				{
					new DirectoryInfo(text);
					directory = text;
					return true;
				}
				catch
				{
					return false;
				}
			}
			try
			{
				FileInfo fileInfo = new FileInfo(text);
				directory = fileInfo.DirectoryName;
				filename = fileInfo.Name;
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x0600197D RID: 6525 RVA: 0x00065140 File Offset: 0x00063340
		protected virtual object CreateDialog(string initialDirectory, string filename)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600197E RID: 6526 RVA: 0x00065148 File Offset: 0x00063348
		private void TryRaisePathChanged()
		{
			string text = base.Text;
			string text2 = this.oldText;
			if (text != text2)
			{
				this.oldText = text;
				this.OnPathChanged(text, text2);
			}
		}

		// Token: 0x0600197F RID: 6527 RVA: 0x0006517C File Offset: 0x0006337C
		protected void OnPathChanged(string newPath, string oldPath)
		{
			PathTextBoxPathChangedEventArgs pathTextBoxPathChangedEventArgs = new PathTextBoxPathChangedEventArgs(newPath, oldPath);
			EventHandler<PathTextBoxPathChangedEventArgs> pathChanged = this.PathChanged;
			if (pathChanged == null)
			{
				return;
			}
			pathChanged(this, pathTextBoxPathChangedEventArgs);
		}

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x06001980 RID: 6528 RVA: 0x000651A4 File Offset: 0x000633A4
		// (remove) Token: 0x06001981 RID: 6529 RVA: 0x000651DC File Offset: 0x000633DC
		public event EventHandler<PathTextBoxPathChangedEventArgs> PathChanged;

		// Token: 0x040008C1 RID: 2241
		private static readonly ImmutableHashSet<char> InvalidPathChars;

		// Token: 0x040008C2 RID: 2242
		private string oldText = string.Empty;

		// Token: 0x040008C3 RID: 2243
		private Button browserButton;
	}
}
