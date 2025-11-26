using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x02000262 RID: 610
	public partial class ToolbarButton : Button
	{
		// Token: 0x06002346 RID: 9030 RVA: 0x000A628C File Offset: 0x000A448C
		static ToolbarButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarButton), new FrameworkPropertyMetadata(typeof(ToolbarButton)));
		}

		// Token: 0x06002347 RID: 9031 RVA: 0x000A631E File Offset: 0x000A451E
		public ToolbarButton()
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				base.Loaded += this.ToolbarButton_Loaded;
				base.Unloaded += this.ToolbarButton_Unloaded;
			}
			ToolbarButtonHelper.RegisterIsKeyboardFocused(this);
		}

		// Token: 0x17000B44 RID: 2884
		// (get) Token: 0x06002348 RID: 9032 RVA: 0x000A6358 File Offset: 0x000A4558
		// (set) Token: 0x06002349 RID: 9033 RVA: 0x000A6360 File Offset: 0x000A4560
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

		// Token: 0x0600234A RID: 9034 RVA: 0x000A63BB File Offset: 0x000A45BB
		private void HeaderPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ToolbarButtonHelper.UpdateHeaderStates(this);
		}

		// Token: 0x0600234B RID: 9035 RVA: 0x000A63C3 File Offset: 0x000A45C3
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.HeaderPresenter = base.GetTemplateChild("HeaderPresenter") as ContentPresenter;
			ToolbarButtonHelper.UpdateContentStates(this);
			ToolbarButtonHelper.UpdateHeaderStates(this);
		}

		// Token: 0x0600234C RID: 9036 RVA: 0x000A63F0 File Offset: 0x000A45F0
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

		// Token: 0x0600234D RID: 9037 RVA: 0x000A6478 File Offset: 0x000A4678
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

		// Token: 0x0600234E RID: 9038 RVA: 0x000A64C9 File Offset: 0x000A46C9
		private void Window_Activated(object sender, EventArgs e)
		{
			this.IsMouseOverInternal = false;
		}

		// Token: 0x0600234F RID: 9039 RVA: 0x000A64D2 File Offset: 0x000A46D2
		private void Window_Deactivated(object sender, EventArgs e)
		{
			this.IsMouseOverInternal = false;
		}

		// Token: 0x06002350 RID: 9040 RVA: 0x000A64DB File Offset: 0x000A46DB
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			this.IsMouseOverInternal = ToolbarButtonHelper.IsContentMouseOver(this, e);
		}

		// Token: 0x06002351 RID: 9041 RVA: 0x000A64F1 File Offset: 0x000A46F1
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			this.IsMouseOverInternal = ToolbarButtonHelper.IsContentMouseOver(this, e);
		}

		// Token: 0x17000B45 RID: 2885
		// (get) Token: 0x06002352 RID: 9042 RVA: 0x000A6507 File Offset: 0x000A4707
		// (set) Token: 0x06002353 RID: 9043 RVA: 0x000A6514 File Offset: 0x000A4714
		public object Header
		{
			get
			{
				return base.GetValue(ToolbarButton.HeaderProperty);
			}
			set
			{
				base.SetValue(ToolbarButton.HeaderProperty, value);
			}
		}

		// Token: 0x17000B46 RID: 2886
		// (get) Token: 0x06002354 RID: 9044 RVA: 0x000A6522 File Offset: 0x000A4722
		// (set) Token: 0x06002355 RID: 9045 RVA: 0x000A6534 File Offset: 0x000A4734
		public DataTemplate HeaderTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(ToolbarButton.HeaderTemplateProperty);
			}
			set
			{
				base.SetValue(ToolbarButton.HeaderTemplateProperty, value);
			}
		}

		// Token: 0x17000B47 RID: 2887
		// (get) Token: 0x06002356 RID: 9046 RVA: 0x000A6542 File Offset: 0x000A4742
		// (set) Token: 0x06002357 RID: 9047 RVA: 0x000A6554 File Offset: 0x000A4754
		public Orientation Orientation
		{
			get
			{
				return (Orientation)base.GetValue(ToolbarButton.OrientationProperty);
			}
			set
			{
				base.SetValue(ToolbarButton.OrientationProperty, value);
			}
		}

		// Token: 0x06002358 RID: 9048 RVA: 0x000A6568 File Offset: 0x000A4768
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

		// Token: 0x17000B48 RID: 2888
		// (get) Token: 0x06002359 RID: 9049 RVA: 0x000A659F File Offset: 0x000A479F
		// (set) Token: 0x0600235A RID: 9050 RVA: 0x000A65B1 File Offset: 0x000A47B1
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

		// Token: 0x0600235B RID: 9051 RVA: 0x000A65C4 File Offset: 0x000A47C4
		protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
		{
			base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);
			ToolbarButtonHelper.UpdateContentStates(this);
		}

		// Token: 0x0600235C RID: 9052 RVA: 0x000A65D4 File Offset: 0x000A47D4
		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
		{
			base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);
			ToolbarButtonHelper.UpdateContentStates(this);
		}

		// Token: 0x0600235D RID: 9053 RVA: 0x000A65E4 File Offset: 0x000A47E4
		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);
			ToolbarButtonHelper.UpdateContentStates(this);
			ToolbarButtonHelper.UpdateHeaderStates(this);
		}

		// Token: 0x04000F03 RID: 3843
		private ContentPresenter headerPresenter;

		// Token: 0x04000F04 RID: 3844
		private Window window;

		// Token: 0x04000F05 RID: 3845
		public static readonly DependencyProperty HeaderProperty = ToolbarButtonHelper.HeaderProperty.AddOwner(typeof(ToolbarButton));

		// Token: 0x04000F06 RID: 3846
		public static readonly DependencyProperty HeaderTemplateProperty = ToolbarButtonHelper.HeaderTemplateProperty.AddOwner(typeof(ToolbarButton));

		// Token: 0x04000F07 RID: 3847
		public static readonly DependencyProperty OrientationProperty = ToolbarButtonHelper.OrientationProperty.AddOwner(typeof(ToolbarButton), new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(ToolbarButton.OnOrientationPropertyChanged)));
	}
}
