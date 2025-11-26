using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x02000269 RID: 617
	public class ToolbarToggleButton : ToggleButton
	{
		// Token: 0x060023E0 RID: 9184 RVA: 0x000A7BF4 File Offset: 0x000A5DF4
		static ToolbarToggleButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarToggleButton), new FrameworkPropertyMetadata(typeof(ToolbarToggleButton)));
		}

		// Token: 0x060023E1 RID: 9185 RVA: 0x000A7C86 File Offset: 0x000A5E86
		public ToolbarToggleButton()
		{
			base.Loaded += this.ToolbarButton_Loaded;
			base.Unloaded += this.ToolbarButton_Unloaded;
			ToolbarButtonHelper.RegisterIsKeyboardFocused(this);
		}

		// Token: 0x17000B69 RID: 2921
		// (get) Token: 0x060023E2 RID: 9186 RVA: 0x000A7CB8 File Offset: 0x000A5EB8
		// (set) Token: 0x060023E3 RID: 9187 RVA: 0x000A7CC0 File Offset: 0x000A5EC0
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

		// Token: 0x060023E4 RID: 9188 RVA: 0x000A7D1B File Offset: 0x000A5F1B
		private void HeaderPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ToolbarButtonHelper.UpdateHeaderStates(this);
		}

		// Token: 0x060023E5 RID: 9189 RVA: 0x000A7D23 File Offset: 0x000A5F23
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.HeaderPresenter = base.GetTemplateChild("HeaderPresenter") as ContentPresenter;
			ToolbarButtonHelper.UpdateContentStates(this);
			ToolbarButtonHelper.UpdateHeaderStates(this);
		}

		// Token: 0x060023E6 RID: 9190 RVA: 0x000A7D50 File Offset: 0x000A5F50
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

		// Token: 0x060023E7 RID: 9191 RVA: 0x000A7DD8 File Offset: 0x000A5FD8
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

		// Token: 0x060023E8 RID: 9192 RVA: 0x000A7E29 File Offset: 0x000A6029
		private void Window_Activated(object sender, EventArgs e)
		{
			this.IsMouseOverInternal = false;
		}

		// Token: 0x060023E9 RID: 9193 RVA: 0x000A7E32 File Offset: 0x000A6032
		private void Window_Deactivated(object sender, EventArgs e)
		{
			this.IsMouseOverInternal = false;
		}

		// Token: 0x060023EA RID: 9194 RVA: 0x000A7E3B File Offset: 0x000A603B
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			this.IsMouseOverInternal = ToolbarButtonHelper.IsContentMouseOver(this, e);
		}

		// Token: 0x060023EB RID: 9195 RVA: 0x000A7E51 File Offset: 0x000A6051
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			this.IsMouseOverInternal = ToolbarButtonHelper.IsContentMouseOver(this, e);
		}

		// Token: 0x17000B6A RID: 2922
		// (get) Token: 0x060023EC RID: 9196 RVA: 0x000A7E67 File Offset: 0x000A6067
		// (set) Token: 0x060023ED RID: 9197 RVA: 0x000A7E74 File Offset: 0x000A6074
		public object Header
		{
			get
			{
				return base.GetValue(ToolbarToggleButton.HeaderProperty);
			}
			set
			{
				base.SetValue(ToolbarToggleButton.HeaderProperty, value);
			}
		}

		// Token: 0x17000B6B RID: 2923
		// (get) Token: 0x060023EE RID: 9198 RVA: 0x000A7E82 File Offset: 0x000A6082
		// (set) Token: 0x060023EF RID: 9199 RVA: 0x000A7E94 File Offset: 0x000A6094
		public DataTemplate HeaderTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(ToolbarToggleButton.HeaderTemplateProperty);
			}
			set
			{
				base.SetValue(ToolbarToggleButton.HeaderTemplateProperty, value);
			}
		}

		// Token: 0x17000B6C RID: 2924
		// (get) Token: 0x060023F0 RID: 9200 RVA: 0x000A7EA2 File Offset: 0x000A60A2
		// (set) Token: 0x060023F1 RID: 9201 RVA: 0x000A7EB4 File Offset: 0x000A60B4
		public Orientation Orientation
		{
			get
			{
				return (Orientation)base.GetValue(ToolbarToggleButton.OrientationProperty);
			}
			set
			{
				base.SetValue(ToolbarToggleButton.OrientationProperty, value);
			}
		}

		// Token: 0x060023F2 RID: 9202 RVA: 0x000A7EC8 File Offset: 0x000A60C8
		private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((Orientation)e.NewValue != (Orientation)e.OldValue)
			{
				ToolbarToggleButton toolbarToggleButton = d as ToolbarToggleButton;
				if (toolbarToggleButton != null)
				{
					ToolbarButtonHelper.UpdateHeaderStates(toolbarToggleButton);
				}
			}
		}

		// Token: 0x17000B6D RID: 2925
		// (get) Token: 0x060023F3 RID: 9203 RVA: 0x000A7EFF File Offset: 0x000A60FF
		// (set) Token: 0x060023F4 RID: 9204 RVA: 0x000A7F11 File Offset: 0x000A6111
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

		// Token: 0x060023F5 RID: 9205 RVA: 0x000A7F24 File Offset: 0x000A6124
		protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
		{
			base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);
			ToolbarButtonHelper.UpdateContentStates(this);
		}

		// Token: 0x060023F6 RID: 9206 RVA: 0x000A7F34 File Offset: 0x000A6134
		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
		{
			base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);
			ToolbarButtonHelper.UpdateContentStates(this);
		}

		// Token: 0x060023F7 RID: 9207 RVA: 0x000A7F44 File Offset: 0x000A6144
		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);
			ToolbarButtonHelper.UpdateContentStates(this);
			ToolbarButtonHelper.UpdateHeaderStates(this);
		}

		// Token: 0x04000F33 RID: 3891
		private ContentPresenter headerPresenter;

		// Token: 0x04000F34 RID: 3892
		private Window window;

		// Token: 0x04000F35 RID: 3893
		public static readonly DependencyProperty HeaderProperty = ToolbarButtonHelper.HeaderProperty.AddOwner(typeof(ToolbarToggleButton));

		// Token: 0x04000F36 RID: 3894
		public static readonly DependencyProperty HeaderTemplateProperty = ToolbarButtonHelper.HeaderTemplateProperty.AddOwner(typeof(ToolbarToggleButton));

		// Token: 0x04000F37 RID: 3895
		public static readonly DependencyProperty OrientationProperty = ToolbarButtonHelper.OrientationProperty.AddOwner(typeof(ToolbarToggleButton), new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(ToolbarToggleButton.OnOrientationPropertyChanged)));
	}
}
