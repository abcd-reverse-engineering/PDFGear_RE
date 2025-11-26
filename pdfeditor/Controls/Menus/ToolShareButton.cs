using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x0200025F RID: 607
	public class ToolShareButton : Button
	{
		// Token: 0x06002327 RID: 8999 RVA: 0x000A5D2F File Offset: 0x000A3F2F
		static ToolShareButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolShareButton), new FrameworkPropertyMetadata(typeof(ToolShareButton)));
		}

		// Token: 0x06002328 RID: 9000 RVA: 0x000A5D54 File Offset: 0x000A3F54
		public ToolShareButton()
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				base.Loaded += this.ToolbarButton_Loaded;
				base.Unloaded += this.ToolbarButton_Unloaded;
			}
			ToolbarButtonHelper.RegisterIsKeyboardFocused(this);
		}

		// Token: 0x17000B41 RID: 2881
		// (get) Token: 0x06002329 RID: 9001 RVA: 0x000A5D8E File Offset: 0x000A3F8E
		// (set) Token: 0x0600232A RID: 9002 RVA: 0x000A5D98 File Offset: 0x000A3F98
		private ContentPresenter HeaderPresenter
		{
			get
			{
				return this.headerPresenter;
			}
			set
			{
				if (this.headerPresenter != value)
				{
					if (this.headerPresenter != null)
					{
						this.headerPresenter.SizeChanged -= this.HeaderPresenter_SizeChanged;
					}
					this.headerPresenter = value;
					if (this.headerPresenter != null)
					{
						this.headerPresenter.SizeChanged += this.HeaderPresenter_SizeChanged;
					}
				}
			}
		}

		// Token: 0x0600232B RID: 9003 RVA: 0x000A5DF3 File Offset: 0x000A3FF3
		private void HeaderPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ToolbarButtonHelper.UpdateHeaderStates(this);
		}

		// Token: 0x0600232C RID: 9004 RVA: 0x000A5DFB File Offset: 0x000A3FFB
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.HeaderPresenter = base.GetTemplateChild("HeaderPresenter") as ContentPresenter;
			ToolbarButtonHelper.UpdateContentStates(this);
			ToolbarButtonHelper.UpdateHeaderStates(this);
		}

		// Token: 0x0600232D RID: 9005 RVA: 0x000A5E28 File Offset: 0x000A4028
		private void ToolbarButton_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.window != null)
			{
				this.window.Activated -= this.Window_Activated;
				this.window.Deactivated -= this.Window_Deactivated;
			}
			this.window = Window.GetWindow(this);
			if (this.window != null)
			{
				this.window.Activated += this.Window_Activated;
				this.window.Deactivated += this.Window_Deactivated;
			}
		}

		// Token: 0x0600232E RID: 9006 RVA: 0x000A5EB0 File Offset: 0x000A40B0
		private void ToolbarButton_Unloaded(object sender, RoutedEventArgs e)
		{
			if (this.window != null)
			{
				this.window.Activated -= this.Window_Activated;
				this.window.Deactivated -= this.Window_Deactivated;
			}
			this.window = null;
			this.IsMouseOverInternal = false;
		}

		// Token: 0x0600232F RID: 9007 RVA: 0x000A5F01 File Offset: 0x000A4101
		private void Window_Activated(object sender, EventArgs e)
		{
			this.IsMouseOverInternal = false;
		}

		// Token: 0x06002330 RID: 9008 RVA: 0x000A5F0A File Offset: 0x000A410A
		private void Window_Deactivated(object sender, EventArgs e)
		{
			this.IsMouseOverInternal = false;
		}

		// Token: 0x06002331 RID: 9009 RVA: 0x000A5F13 File Offset: 0x000A4113
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			this.IsMouseOverInternal = ToolbarButtonHelper.IsContentMouseOver(this, e);
		}

		// Token: 0x06002332 RID: 9010 RVA: 0x000A5F29 File Offset: 0x000A4129
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			this.IsMouseOverInternal = ToolbarButtonHelper.IsContentMouseOver(this, e);
		}

		// Token: 0x06002333 RID: 9011 RVA: 0x000A5F40 File Offset: 0x000A4140
		private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((Orientation)e.NewValue != (Orientation)e.OldValue)
			{
				ToolbarButton toolbarButton = d as ToolbarButton;
				if (toolbarButton != null)
				{
					ToolbarButtonHelper.UpdateHeaderStates(toolbarButton);
				}
			}
		}

		// Token: 0x17000B42 RID: 2882
		// (get) Token: 0x06002334 RID: 9012 RVA: 0x000A5F77 File Offset: 0x000A4177
		// (set) Token: 0x06002335 RID: 9013 RVA: 0x000A5F89 File Offset: 0x000A4189
		private bool IsMouseOverInternal
		{
			get
			{
				return (bool)base.GetValue(ToolbarButtonHelper.IsMouseOverInternalProperty);
			}
			set
			{
				base.SetValue(ToolbarButtonHelper.IsMouseOverInternalProperty, value);
			}
		}

		// Token: 0x06002336 RID: 9014 RVA: 0x000A5F9C File Offset: 0x000A419C
		protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
		{
			base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);
			ToolbarButtonHelper.UpdateContentStates(this);
		}

		// Token: 0x06002337 RID: 9015 RVA: 0x000A5FAC File Offset: 0x000A41AC
		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
		{
			base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);
			ToolbarButtonHelper.UpdateContentStates(this);
		}

		// Token: 0x06002338 RID: 9016 RVA: 0x000A5FBC File Offset: 0x000A41BC
		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);
			ToolbarButtonHelper.UpdateContentStates(this);
			ToolbarButtonHelper.UpdateHeaderStates(this);
		}

		// Token: 0x04000F00 RID: 3840
		private ContentPresenter headerPresenter;

		// Token: 0x04000F01 RID: 3841
		private Window window;
	}
}
