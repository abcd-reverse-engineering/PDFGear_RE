using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonLib.Controls;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x02000268 RID: 616
	public class ToolbarRadioButton : KeyedRadioButton
	{
		// Token: 0x060023C0 RID: 9152 RVA: 0x000A7720 File Offset: 0x000A5920
		static ToolbarRadioButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarRadioButton), new FrameworkPropertyMetadata(typeof(ToolbarRadioButton)));
		}

		// Token: 0x060023C1 RID: 9153 RVA: 0x000A781A File Offset: 0x000A5A1A
		public ToolbarRadioButton()
		{
			base.Loaded += this.ToolbarButton_Loaded;
			base.Unloaded += this.ToolbarButton_Unloaded;
			ToolbarButtonHelper.RegisterIsKeyboardFocused(this);
		}

		// Token: 0x17000B62 RID: 2914
		// (get) Token: 0x060023C2 RID: 9154 RVA: 0x000A784C File Offset: 0x000A5A4C
		// (set) Token: 0x060023C3 RID: 9155 RVA: 0x000A7854 File Offset: 0x000A5A54
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

		// Token: 0x060023C4 RID: 9156 RVA: 0x000A78AF File Offset: 0x000A5AAF
		private void HeaderPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ToolbarButtonHelper.UpdateHeaderStates(this);
		}

		// Token: 0x060023C5 RID: 9157 RVA: 0x000A78B7 File Offset: 0x000A5AB7
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.HeaderPresenter = base.GetTemplateChild("HeaderPresenter") as ContentPresenter;
			ToolbarButtonHelper.UpdateContentStates(this);
			ToolbarButtonHelper.UpdateHeaderStates(this);
		}

		// Token: 0x060023C6 RID: 9158 RVA: 0x000A78E4 File Offset: 0x000A5AE4
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

		// Token: 0x060023C7 RID: 9159 RVA: 0x000A796C File Offset: 0x000A5B6C
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

		// Token: 0x060023C8 RID: 9160 RVA: 0x000A79BD File Offset: 0x000A5BBD
		private void Window_Activated(object sender, EventArgs e)
		{
			this.IsMouseOverInternal = false;
		}

		// Token: 0x060023C9 RID: 9161 RVA: 0x000A79C6 File Offset: 0x000A5BC6
		private void Window_Deactivated(object sender, EventArgs e)
		{
			this.IsMouseOverInternal = false;
		}

		// Token: 0x060023CA RID: 9162 RVA: 0x000A79CF File Offset: 0x000A5BCF
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			this.IsMouseOverInternal = ToolbarButtonHelper.IsContentMouseOver(this, e);
		}

		// Token: 0x060023CB RID: 9163 RVA: 0x000A79E5 File Offset: 0x000A5BE5
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			this.IsMouseOverInternal = ToolbarButtonHelper.IsContentMouseOver(this, e);
		}

		// Token: 0x17000B63 RID: 2915
		// (get) Token: 0x060023CC RID: 9164 RVA: 0x000A79FB File Offset: 0x000A5BFB
		// (set) Token: 0x060023CD RID: 9165 RVA: 0x000A7A08 File Offset: 0x000A5C08
		public object Header
		{
			get
			{
				return base.GetValue(ToolbarRadioButton.HeaderProperty);
			}
			set
			{
				base.SetValue(ToolbarRadioButton.HeaderProperty, value);
			}
		}

		// Token: 0x17000B64 RID: 2916
		// (get) Token: 0x060023CE RID: 9166 RVA: 0x000A7A16 File Offset: 0x000A5C16
		// (set) Token: 0x060023CF RID: 9167 RVA: 0x000A7A28 File Offset: 0x000A5C28
		public DataTemplate HeaderTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(ToolbarRadioButton.HeaderTemplateProperty);
			}
			set
			{
				base.SetValue(ToolbarRadioButton.HeaderTemplateProperty, value);
			}
		}

		// Token: 0x17000B65 RID: 2917
		// (get) Token: 0x060023D0 RID: 9168 RVA: 0x000A7A36 File Offset: 0x000A5C36
		// (set) Token: 0x060023D1 RID: 9169 RVA: 0x000A7A48 File Offset: 0x000A5C48
		public Orientation Orientation
		{
			get
			{
				return (Orientation)base.GetValue(ToolbarRadioButton.OrientationProperty);
			}
			set
			{
				base.SetValue(ToolbarRadioButton.OrientationProperty, value);
			}
		}

		// Token: 0x060023D2 RID: 9170 RVA: 0x000A7A5C File Offset: 0x000A5C5C
		private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((Orientation)e.NewValue != (Orientation)e.OldValue)
			{
				ToolbarRadioButton toolbarRadioButton = d as ToolbarRadioButton;
				if (toolbarRadioButton != null)
				{
					ToolbarButtonHelper.UpdateHeaderStates(toolbarRadioButton);
				}
			}
		}

		// Token: 0x17000B66 RID: 2918
		// (get) Token: 0x060023D3 RID: 9171 RVA: 0x000A7A93 File Offset: 0x000A5C93
		// (set) Token: 0x060023D4 RID: 9172 RVA: 0x000A7AA5 File Offset: 0x000A5CA5
		public bool IsToggleEnabled
		{
			get
			{
				return (bool)base.GetValue(ToolbarRadioButton.IsToggleEnabledProperty);
			}
			set
			{
				base.SetValue(ToolbarRadioButton.IsToggleEnabledProperty, value);
			}
		}

		// Token: 0x17000B67 RID: 2919
		// (get) Token: 0x060023D5 RID: 9173 RVA: 0x000A7AB8 File Offset: 0x000A5CB8
		// (set) Token: 0x060023D6 RID: 9174 RVA: 0x000A7ACA File Offset: 0x000A5CCA
		public bool IsCheckable
		{
			get
			{
				return (bool)base.GetValue(ToolbarRadioButton.IsCheckableProperty);
			}
			set
			{
				base.SetValue(ToolbarRadioButton.IsCheckableProperty, value);
			}
		}

		// Token: 0x060023D7 RID: 9175 RVA: 0x000A7AE0 File Offset: 0x000A5CE0
		private static void OnIsCheckablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue != (bool)e.OldValue)
			{
				ToolbarRadioButton toolbarRadioButton = d as ToolbarRadioButton;
				if (toolbarRadioButton != null && !(bool)e.NewValue)
				{
					bool? isChecked = toolbarRadioButton.IsChecked;
					bool flag = false;
					if (!((isChecked.GetValueOrDefault() == flag) & (isChecked != null)))
					{
						toolbarRadioButton.IsChecked = new bool?(false);
					}
				}
			}
		}

		// Token: 0x17000B68 RID: 2920
		// (get) Token: 0x060023D8 RID: 9176 RVA: 0x000A7B48 File Offset: 0x000A5D48
		// (set) Token: 0x060023D9 RID: 9177 RVA: 0x000A7B5A File Offset: 0x000A5D5A
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

		// Token: 0x060023DA RID: 9178 RVA: 0x000A7B6D File Offset: 0x000A5D6D
		protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
		{
			base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);
			ToolbarButtonHelper.UpdateContentStates(this);
		}

		// Token: 0x060023DB RID: 9179 RVA: 0x000A7B7D File Offset: 0x000A5D7D
		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
		{
			base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);
			ToolbarButtonHelper.UpdateContentStates(this);
		}

		// Token: 0x060023DC RID: 9180 RVA: 0x000A7B8D File Offset: 0x000A5D8D
		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);
			ToolbarButtonHelper.UpdateContentStates(this);
			ToolbarButtonHelper.UpdateHeaderStates(this);
		}

		// Token: 0x060023DD RID: 9181 RVA: 0x000A7BA3 File Offset: 0x000A5DA3
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
		}

		// Token: 0x060023DE RID: 9182 RVA: 0x000A7BAC File Offset: 0x000A5DAC
		protected override void OnClick()
		{
			base.OnClick();
		}

		// Token: 0x060023DF RID: 9183 RVA: 0x000A7BB4 File Offset: 0x000A5DB4
		protected override void OnToggle()
		{
			if (this.IsCheckable && this.IsToggleEnabled)
			{
				base.IsChecked = new bool?(!base.IsChecked.GetValueOrDefault());
				return;
			}
			base.OnToggle();
		}

		// Token: 0x04000F2C RID: 3884
		private ContentPresenter headerPresenter;

		// Token: 0x04000F2D RID: 3885
		private Window window;

		// Token: 0x04000F2E RID: 3886
		public static readonly DependencyProperty HeaderProperty = ToolbarButtonHelper.HeaderProperty.AddOwner(typeof(ToolbarRadioButton));

		// Token: 0x04000F2F RID: 3887
		public static readonly DependencyProperty HeaderTemplateProperty = ToolbarButtonHelper.HeaderTemplateProperty.AddOwner(typeof(ToolbarRadioButton));

		// Token: 0x04000F30 RID: 3888
		public static readonly DependencyProperty OrientationProperty = ToolbarButtonHelper.OrientationProperty.AddOwner(typeof(ToolbarRadioButton), new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(ToolbarRadioButton.OnOrientationPropertyChanged)));

		// Token: 0x04000F31 RID: 3889
		public static readonly DependencyProperty IsToggleEnabledProperty = DependencyProperty.Register("IsToggleEnabled", typeof(bool), typeof(ToolbarRadioButton), new PropertyMetadata(true));

		// Token: 0x04000F32 RID: 3890
		public static readonly DependencyProperty IsCheckableProperty = DependencyProperty.Register("IsCheckable", typeof(bool), typeof(ToolbarRadioButton), new PropertyMetadata(true, new PropertyChangedCallback(ToolbarRadioButton.OnIsCheckablePropertyChanged)));
	}
}
