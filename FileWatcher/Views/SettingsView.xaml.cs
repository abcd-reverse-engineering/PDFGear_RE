using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace FileWatcher.Views
{
	// Token: 0x02000018 RID: 24
	public partial class SettingsView : Window
	{
		// Token: 0x06000074 RID: 116 RVA: 0x000037EF File Offset: 0x000019EF
		public SettingsView()
		{
			this.InitializeComponent();
			base.Loaded += this.SettingsView_Loaded;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000380F File Offset: 0x00001A0F
		private void SettingsView_Loaded(object sender, RoutedEventArgs e)
		{
			this.UpdateState();
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00003817 File Offset: 0x00001A17
		private void StackPanel_Click(object sender, RoutedEventArgs e)
		{
			this.UpdateState();
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00003820 File Offset: 0x00001A20
		private void UpdateState()
		{
			foreach (CheckBox checkBox in from c in this.Panel.Children.OfType<CheckBox>()
				where c.Content is string
				select c)
			{
				string text = "";
				string text2 = (string)checkBox.Content;
				if (!(text2 == "Desktop"))
				{
					if (!(text2 == "Download"))
					{
						if (text2 == "Documents")
						{
							text = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
						}
					}
					else
					{
						text = KnownFolders.GetPath(KnownFolder.Downloads);
					}
				}
				else
				{
					text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				}
				if (!string.IsNullOrEmpty(text))
				{
					if (checkBox.IsChecked.GetValueOrDefault())
					{
						App.Current.Watcher.AddPath(text, "*.pdf", true);
					}
					else
					{
						App.Current.Watcher.RemovePath(text);
					}
				}
			}
		}
	}
}
