using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using pdfeditor.Models.Menus.ToolbarSettings;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x0200027E RID: 638
	public partial class ToolbarSettingPanel : UserControl
	{
		// Token: 0x060024D6 RID: 9430 RVA: 0x000AA658 File Offset: 0x000A8858
		public ToolbarSettingPanel()
		{
			this.InitializeComponent();
			this.ShowContent = (Storyboard)this.HostCanvas.Resources["ShowContent"];
			this.HideContent = (Storyboard)this.HostCanvas.Resources["HideContent"];
			base.Loaded += this.ToolbarSettingPanel_Loaded;
		}

		// Token: 0x060024D7 RID: 9431 RVA: 0x000AA6C3 File Offset: 0x000A88C3
		private void ToolbarSettingPanel_Loaded(object sender, RoutedEventArgs e)
		{
			this.ClipBorder.Visibility = Visibility.Collapsed;
			this.LayoutRootShadow.Opacity = 0.0;
			this.LayoutRootTrans.Y = -44.0;
		}

		// Token: 0x060024D8 RID: 9432 RVA: 0x000AA6FC File Offset: 0x000A88FC
		private void UpdateModel()
		{
			this.ContentItemsControl.ItemsSource = this.Model;
			if (this.Model == null)
			{
				this.ShowContent.Stop();
				this.HideContent.Begin();
				return;
			}
			this.HideContent.Stop();
			this.ShowContent.Begin();
		}

		// Token: 0x17000BA2 RID: 2978
		// (get) Token: 0x060024D9 RID: 9433 RVA: 0x000AA74F File Offset: 0x000A894F
		// (set) Token: 0x060024DA RID: 9434 RVA: 0x000AA761 File Offset: 0x000A8961
		public ToolbarSettingModel Model
		{
			get
			{
				return (ToolbarSettingModel)base.GetValue(ToolbarSettingPanel.ModelProperty);
			}
			set
			{
				base.SetValue(ToolbarSettingPanel.ModelProperty, value);
			}
		}

		// Token: 0x060024DB RID: 9435 RVA: 0x000AA76F File Offset: 0x000A896F
		private static void OnModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				((ToolbarSettingPanel)d).UpdateModel();
			}
		}

		// Token: 0x060024DC RID: 9436 RVA: 0x000AA78C File Offset: 0x000A898C
		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ClipBorder.Width = e.NewSize.Width;
		}

		// Token: 0x04000F8B RID: 3979
		private Storyboard ShowContent;

		// Token: 0x04000F8C RID: 3980
		private Storyboard HideContent;

		// Token: 0x04000F8D RID: 3981
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingModel), typeof(ToolbarSettingPanel), new PropertyMetadata(null, new PropertyChangedCallback(ToolbarSettingPanel.OnModelPropertyChanged)));
	}
}
