using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PDFLauncher.CustomControl
{
	// Token: 0x02000025 RID: 37
	public class PathTextBox : TextBox
	{
		// Token: 0x060001E0 RID: 480 RVA: 0x000072D4 File Offset: 0x000054D4
		static PathTextBox()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PathTextBox), new FrameworkPropertyMetadata(typeof(PathTextBox)));
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x000072F9 File Offset: 0x000054F9
		// (set) Token: 0x060001E2 RID: 482 RVA: 0x00007304 File Offset: 0x00005504
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

		// Token: 0x060001E3 RID: 483 RVA: 0x0000735F File Offset: 0x0000555F
		private void BrowserButton_Click(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			this.RequestDialog();
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00007370 File Offset: 0x00005570
		private bool RequestDialog()
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

		// Token: 0x060001E5 RID: 485 RVA: 0x00007453 File Offset: 0x00005653
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.BrowserButton = base.GetTemplateChild("BrowserButton") as Button;
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x00007471 File Offset: 0x00005671
		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			base.OnTextChanged(e);
			if (!base.IsFocused)
			{
				this.TryRaisePathChanged();
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00007488 File Offset: 0x00005688
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);
			if (Keyboard.FocusedElement != this.BrowserButton)
			{
				this.TryRaisePathChanged();
			}
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x000074A4 File Offset: 0x000056A4
		private bool TryGetDirectoryAndFile(string text, out string directory, out string filename)
		{
			directory = "";
			filename = "";
			if (string.IsNullOrEmpty(text))
			{
				return false;
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

		// Token: 0x060001E9 RID: 489 RVA: 0x0000752C File Offset: 0x0000572C
		protected virtual object CreateDialog(string initialDirectory, string filename)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00007534 File Offset: 0x00005734
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

		// Token: 0x060001EB RID: 491 RVA: 0x00007568 File Offset: 0x00005768
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

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060001EC RID: 492 RVA: 0x00007590 File Offset: 0x00005790
		// (remove) Token: 0x060001ED RID: 493 RVA: 0x000075C8 File Offset: 0x000057C8
		public event EventHandler<PathTextBoxPathChangedEventArgs> PathChanged;

		// Token: 0x040000E4 RID: 228
		private string oldText = string.Empty;

		// Token: 0x040000E5 RID: 229
		private Button browserButton;
	}
}
