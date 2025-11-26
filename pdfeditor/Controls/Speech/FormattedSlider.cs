using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace pdfeditor.Controls.Speech
{
	// Token: 0x020001F1 RID: 497
	public class FormattedSlider : Slider
	{
		// Token: 0x17000A23 RID: 2595
		// (get) Token: 0x06001C1D RID: 7197 RVA: 0x00075141 File Offset: 0x00073341
		// (set) Token: 0x06001C1E RID: 7198 RVA: 0x00075149 File Offset: 0x00073349
		public string AutoToolTipFormat
		{
			get
			{
				return this._autoToolTipFormat;
			}
			set
			{
				this._autoToolTipFormat = value;
			}
		}

		// Token: 0x06001C1F RID: 7199 RVA: 0x00075152 File Offset: 0x00073352
		protected override void OnThumbDragStarted(DragStartedEventArgs e)
		{
			base.OnThumbDragStarted(e);
			this.FormatAutoToolTipContent();
		}

		// Token: 0x06001C20 RID: 7200 RVA: 0x00075161 File Offset: 0x00073361
		protected override void OnThumbDragDelta(DragDeltaEventArgs e)
		{
			base.OnThumbDragDelta(e);
			this.FormatAutoToolTipContent();
		}

		// Token: 0x06001C21 RID: 7201 RVA: 0x00075170 File Offset: 0x00073370
		private void FormatAutoToolTipContent()
		{
			if (!string.IsNullOrEmpty(this.AutoToolTipFormat))
			{
				int num = Convert.ToInt32(this.AutoToolTip.Content.ToString()) - 5;
				this.AutoToolTip.Content = string.Format("{0}", num);
			}
		}

		// Token: 0x17000A24 RID: 2596
		// (get) Token: 0x06001C22 RID: 7202 RVA: 0x000751C0 File Offset: 0x000733C0
		private ToolTip AutoToolTip
		{
			get
			{
				if (this._autoToolTip == null)
				{
					FieldInfo field = typeof(Slider).GetField("_autoToolTip", BindingFlags.Instance | BindingFlags.NonPublic);
					this._autoToolTip = field.GetValue(this) as ToolTip;
					this._autoToolTip.BorderThickness = new Thickness(0.0);
					this._autoToolTip.Background = new SolidColorBrush(Colors.Transparent);
					this._autoToolTip.Margin = new Thickness(-3.5, 0.0, 0.0, 0.0);
				}
				return this._autoToolTip;
			}
		}

		// Token: 0x04000A37 RID: 2615
		private ToolTip _autoToolTip;

		// Token: 0x04000A38 RID: 2616
		private string _autoToolTipFormat;
	}
}
