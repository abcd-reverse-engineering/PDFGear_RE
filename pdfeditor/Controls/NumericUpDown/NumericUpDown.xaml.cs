using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace pdfeditor.Controls.NumericUpDown
{
	// Token: 0x0200025B RID: 603
	public partial class NumericUpDown : UserControl
	{
		// Token: 0x060022D0 RID: 8912 RVA: 0x000A42BC File Offset: 0x000A24BC
		public void Selection()
		{
			this.tbox.SelectAll();
			this.tbox.Focus();
		}

		// Token: 0x17000B38 RID: 2872
		// (get) Token: 0x060022D1 RID: 8913 RVA: 0x000A42D5 File Offset: 0x000A24D5
		// (set) Token: 0x060022D2 RID: 8914 RVA: 0x000A42E7 File Offset: 0x000A24E7
		public int PrintCopies
		{
			get
			{
				return (int)base.GetValue(NumericUpDown.PrintCopiesProperty);
			}
			set
			{
				base.SetValue(NumericUpDown.PrintCopiesProperty, value);
			}
		}

		// Token: 0x060022D3 RID: 8915 RVA: 0x000A42FC File Offset: 0x000A24FC
		public NumericUpDown()
		{
			this.InitializeComponent();
			this.DowbBtn.IsEnabled = false;
			this.DowbBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D2D2D2"));
			this.UpBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"));
		}

		// Token: 0x060022D4 RID: 8916 RVA: 0x000A4360 File Offset: 0x000A2560
		private void UpBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				int num = int.Parse(this.tbox.Text);
				if (num < 1000)
				{
					num++;
					if (num == 1000)
					{
						this.UpBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D2D2D2"));
						this.UpBtn.IsEnabled = false;
					}
					this.PrintCopies = num;
					this.tbox.Text = num.ToString();
					this.DowbBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"));
					this.DowbBtn.IsEnabled = true;
				}
			}
			catch
			{
			}
		}

		// Token: 0x060022D5 RID: 8917 RVA: 0x000A4418 File Offset: 0x000A2618
		private void DowbBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				int num = int.Parse(this.tbox.Text);
				if (num > 1)
				{
					num--;
					if (num == 1)
					{
						this.DowbBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D2D2D2"));
						this.DowbBtn.IsEnabled = false;
					}
					this.PrintCopies = num;
					this.tbox.Text = num.ToString();
					this.UpBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"));
					this.UpBtn.IsEnabled = true;
				}
			}
			catch
			{
			}
		}

		// Token: 0x060022D6 RID: 8918 RVA: 0x000A44C8 File Offset: 0x000A26C8
		private void tbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		// Token: 0x060022D7 RID: 8919 RVA: 0x000A44F4 File Offset: 0x000A26F4
		private void tbox_LostFocus(object sender, RoutedEventArgs e)
		{
			int num;
			int.TryParse(this.tbox.Text, out num);
			if (num == 0)
			{
				this.DowbBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D2D2D2"));
				this.DowbBtn.IsEnabled = false;
			}
			this.PrintCopies = num;
			this.tbox.Text = num.ToString();
		}

		// Token: 0x060022D8 RID: 8920 RVA: 0x000A455C File Offset: 0x000A275C
		private void tbox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!base.IsLoaded)
			{
				return;
			}
			int num;
			int.TryParse(this.tbox.Text, out num);
			if (num <= 1)
			{
				this.DowbBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D2D2D2"));
				this.DowbBtn.IsEnabled = false;
				return;
			}
			this.DowbBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"));
			this.DowbBtn.IsEnabled = true;
			if (num < 1000)
			{
				this.UpBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"));
				this.UpBtn.IsEnabled = true;
				return;
			}
			this.UpBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D2D2D2"));
			this.UpBtn.IsEnabled = false;
		}

		// Token: 0x060022D9 RID: 8921 RVA: 0x000A4642 File Offset: 0x000A2842
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x060022DA RID: 8922 RVA: 0x000A4644 File Offset: 0x000A2844
		private void CommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = false;
			e.Handled = true;
		}

		// Token: 0x04000ED5 RID: 3797
		public static readonly DependencyProperty PrintCopiesProperty = DependencyProperty.Register("PrintCopies", typeof(int), typeof(NumericUpDown), new PropertyMetadata(1));
	}
}
