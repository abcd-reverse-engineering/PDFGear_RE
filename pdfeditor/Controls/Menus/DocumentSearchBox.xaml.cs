using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using pdfeditor.Models.Menus;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x0200025C RID: 604
	public partial class DocumentSearchBox : UserControl
	{
		// Token: 0x060022DE RID: 8926 RVA: 0x000A47B3 File Offset: 0x000A29B3
		public DocumentSearchBox()
		{
			this.InitializeComponent();
			this.UpdateVisibleState(true);
			this.UpdateRecordCountVisibility();
			this.UpdateProgress();
		}

		// Token: 0x17000B39 RID: 2873
		// (get) Token: 0x060022DF RID: 8927 RVA: 0x000A47D4 File Offset: 0x000A29D4
		// (set) Token: 0x060022E0 RID: 8928 RVA: 0x000A47E6 File Offset: 0x000A29E6
		public SearchModel SearchModel
		{
			get
			{
				return (SearchModel)base.GetValue(DocumentSearchBox.SearchModelProperty);
			}
			set
			{
				base.SetValue(DocumentSearchBox.SearchModelProperty, value);
			}
		}

		// Token: 0x060022E1 RID: 8929 RVA: 0x000A47F4 File Offset: 0x000A29F4
		private static void OnSearchModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				DocumentSearchBox documentSearchBox = d as DocumentSearchBox;
				if (documentSearchBox != null)
				{
					INotifyPropertyChanged notifyPropertyChanged = e.OldValue as INotifyPropertyChanged;
					if (notifyPropertyChanged != null)
					{
						WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(notifyPropertyChanged, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(documentSearchBox.OnSearchModelPropertyChanged));
					}
					INotifyPropertyChanged notifyPropertyChanged2 = e.NewValue as INotifyPropertyChanged;
					if (notifyPropertyChanged2 != null)
					{
						WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(notifyPropertyChanged2, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(documentSearchBox.OnSearchModelPropertyChanged));
					}
					documentSearchBox.UpdateVisibleState(false);
					documentSearchBox.UpdateRecordCountVisibility();
					documentSearchBox.UpdateProgress();
				}
			}
		}

		// Token: 0x060022E2 RID: 8930 RVA: 0x000A487C File Offset: 0x000A2A7C
		private void OnSearchModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsSearchVisible")
			{
				this.UpdateVisibleState(false);
				return;
			}
			if (e.PropertyName == "SearchText")
			{
				this.UpdateRecordCountVisibility();
				return;
			}
			if (e.PropertyName == "TotalRecords")
			{
				this.UpdateRecordCountVisibility();
				return;
			}
			if (e.PropertyName == "Progress")
			{
				this.UpdateProgress();
			}
		}

		// Token: 0x060022E3 RID: 8931 RVA: 0x000A48F0 File Offset: 0x000A2AF0
		private void UpdateVisibleState(bool disableAnimation = false)
		{
			bool flag = false;
			if (this.SearchModel != null && this.SearchModel.IsSearchEnabled && this.SearchModel.IsSearchVisible)
			{
				flag = true;
			}
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				flag = true;
			}
			if (VisualStateManager.GoToElementState((FrameworkElement)base.Content, flag ? "SearchVisible" : "SearchInvisible", !disableAnimation) && flag)
			{
				this.FocusTextBox(true);
			}
		}

		// Token: 0x060022E4 RID: 8932 RVA: 0x000A495C File Offset: 0x000A2B5C
		private void UpdateRecordCountVisibility()
		{
			Visibility visibility = Visibility.Collapsed;
			if (this.SearchModel != null)
			{
				if (this.SearchModel.TotalRecords > 0)
				{
					visibility = Visibility.Visible;
				}
				else if (this.SearchModel.TotalRecords == 0 && !string.IsNullOrEmpty(this.SearchModel.SearchText))
				{
					visibility = Visibility.Visible;
				}
			}
			this.RecordCountContainer.Visibility = visibility;
		}

		// Token: 0x060022E5 RID: 8933 RVA: 0x000A49B4 File Offset: 0x000A2BB4
		private void SearchContainer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ShowSearchAnimation.From = new double?(-e.NewSize.Height);
			this.HideSearchAnimation.To = new double?(-e.NewSize.Height);
			this.HideSearchAnimation2.To = new double?(-e.NewSize.Height);
		}

		// Token: 0x060022E6 RID: 8934 RVA: 0x000A4A20 File Offset: 0x000A2C20
		private void SearchCommandPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Thickness padding = this.SearchTextBox.Padding;
			this.SearchTextBox.Padding = new Thickness(padding.Left, padding.Top, e.NewSize.Width, padding.Bottom);
		}

		// Token: 0x060022E7 RID: 8935 RVA: 0x000A4A6C File Offset: 0x000A2C6C
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.Hide();
		}

		// Token: 0x060022E8 RID: 8936 RVA: 0x000A4A74 File Offset: 0x000A2C74
		private void Hide()
		{
			if (this.SearchModel != null)
			{
				this.SearchModel.SearchText = "";
				this.SearchModel.IsSearchVisible = false;
			}
		}

		// Token: 0x060022E9 RID: 8937 RVA: 0x000A4A9A File Offset: 0x000A2C9A
		private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				e.Handled = true;
				if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.None)
				{
					this.InvokeButton(this.NextBtn);
					return;
				}
				this.InvokeButton(this.PrevBtn);
			}
		}

		// Token: 0x060022EA RID: 8938 RVA: 0x000A4ACE File Offset: 0x000A2CCE
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				this.Hide();
			}
		}

		// Token: 0x060022EB RID: 8939 RVA: 0x000A4AEE File Offset: 0x000A2CEE
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			if (e.OriginalSource == this)
			{
				e.Handled = true;
				this.FocusTextBox(true);
			}
		}

		// Token: 0x060022EC RID: 8940 RVA: 0x000A4B10 File Offset: 0x000A2D10
		private void FocusTextBox(bool selectAll = true)
		{
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				this.SearchTextBox.Focus();
				Keyboard.Focus(this.SearchTextBox);
				if (selectAll && !string.IsNullOrEmpty(this.SearchTextBox.Text))
				{
					this.SearchTextBox.SelectAll();
				}
			}));
		}

		// Token: 0x060022ED RID: 8941 RVA: 0x000A4B4A File Offset: 0x000A2D4A
		private void InvokeButton(Button button)
		{
			if (button == null)
			{
				return;
			}
			DocumentSearchBox.InvokeCore(button);
		}

		// Token: 0x060022EE RID: 8942 RVA: 0x000A4B58 File Offset: 0x000A2D58
		private static void InvokeCore(Button button)
		{
			if (button == null)
			{
				return;
			}
			if (DocumentSearchBox.invokeFunc == null)
			{
				Type typeFromHandle = typeof(ButtonAutomationPeer);
				lock (typeFromHandle)
				{
					if (DocumentSearchBox.invokeFunc == null)
					{
						Type typeFromHandle2 = typeof(ButtonAutomationPeer);
						Type @interface = typeFromHandle2.GetInterface("IInvokeProvider");
						MethodInfo methodInfo = ((@interface != null) ? @interface.GetMethod("Invoke") : null);
						if (methodInfo == null)
						{
							DocumentSearchBox.invokeFunc = delegate(Button p)
							{
								if (p != null && p.IsEnabled)
								{
									if (p.Command != null)
									{
										p.Command.Execute(p.CommandParameter);
									}
									p.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
								}
							};
						}
						else
						{
							ParameterExpression parameterExpression = global::System.Linq.Expressions.Expression.Parameter(typeFromHandle2, "p");
							Expression<Action<ButtonAutomationPeer>> expression = global::System.Linq.Expressions.Expression.Lambda<Action<ButtonAutomationPeer>>(global::System.Linq.Expressions.Expression.Call(global::System.Linq.Expressions.Expression.Convert(parameterExpression, @interface), methodInfo), new ParameterExpression[] { parameterExpression });
							Action<ButtonAutomationPeer> func = expression.Compile();
							DocumentSearchBox.invokeFunc = delegate(Button btn)
							{
								if (btn != null && btn.IsEnabled)
								{
									ButtonAutomationPeer buttonAutomationPeer = UIElementAutomationPeer.CreatePeerForElement(btn) as ButtonAutomationPeer;
									if (buttonAutomationPeer != null)
									{
										try
										{
											func(buttonAutomationPeer);
										}
										catch
										{
										}
									}
								}
							};
						}
					}
				}
			}
			DocumentSearchBox.invokeFunc(button);
		}

		// Token: 0x060022EF RID: 8943 RVA: 0x000A4C68 File Offset: 0x000A2E68
		private void SearchContentLayoutRoot_MouseUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			this.FocusTextBox(false);
		}

		// Token: 0x060022F0 RID: 8944 RVA: 0x000A4C78 File Offset: 0x000A2E78
		private void UpdateProgress()
		{
			double num = 0.0;
			if (this.SearchModel != null)
			{
				num = this.SearchModel.Progress;
			}
			this.ProgressClip.Rect = new Rect(0.0, 0.0, num * this.ProgressBorder.ActualWidth, this.ProgressBorder.ActualHeight);
		}

		// Token: 0x060022F1 RID: 8945 RVA: 0x000A4CE0 File Offset: 0x000A2EE0
		private void CloseRecognitionTipBtn_Click(object sender, RoutedEventArgs e)
		{
			SearchModel searchModel = this.SearchModel;
			if (searchModel == null)
			{
				return;
			}
			searchModel.CloseRecognitionTip(this.DoNotShowRecognitionTipCheckBox.IsChecked ?? false);
		}

		// Token: 0x060022F2 RID: 8946 RVA: 0x000A4D1C File Offset: 0x000A2F1C
		private void PerformOcrBtn_Click(object sender, RoutedEventArgs e)
		{
			EventHandler recognitionRequested = this.RecognitionRequested;
			if (recognitionRequested == null)
			{
				return;
			}
			recognitionRequested(this, EventArgs.Empty);
		}

		// Token: 0x14000040 RID: 64
		// (add) Token: 0x060022F3 RID: 8947 RVA: 0x000A4D34 File Offset: 0x000A2F34
		// (remove) Token: 0x060022F4 RID: 8948 RVA: 0x000A4D6C File Offset: 0x000A2F6C
		public event EventHandler RecognitionRequested;

		// Token: 0x060022F5 RID: 8949 RVA: 0x000A4DA1 File Offset: 0x000A2FA1
		public void CloseSearchBox()
		{
			if (this.SearchModel.IsSearchVisible)
			{
				this.Hide();
			}
		}

		// Token: 0x04000EDA RID: 3802
		public static readonly DependencyProperty SearchModelProperty = DependencyProperty.Register("SearchModel", typeof(SearchModel), typeof(DocumentSearchBox), new PropertyMetadata(null, new PropertyChangedCallback(DocumentSearchBox.OnSearchModelPropertyChanged)));

		// Token: 0x04000EDB RID: 3803
		private static Action<Button> invokeFunc;
	}
}
