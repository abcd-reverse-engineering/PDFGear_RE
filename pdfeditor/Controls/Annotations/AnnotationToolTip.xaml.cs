using System;
using System.Windows;
using System.Windows.Controls;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002A2 RID: 674
	public partial class AnnotationToolTip : ToolTip
	{
		// Token: 0x060026DA RID: 9946 RVA: 0x000B7E6C File Offset: 0x000B606C
		static AnnotationToolTip()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AnnotationToolTip), new FrameworkPropertyMetadata(typeof(AnnotationToolTip)));
		}

		// Token: 0x17000BE4 RID: 3044
		// (get) Token: 0x060026DB RID: 9947 RVA: 0x000B7ECE File Offset: 0x000B60CE
		// (set) Token: 0x060026DC RID: 9948 RVA: 0x000B7EDB File Offset: 0x000B60DB
		public object Header
		{
			get
			{
				return base.GetValue(AnnotationToolTip.HeaderProperty);
			}
			set
			{
				base.SetValue(AnnotationToolTip.HeaderProperty, value);
			}
		}

		// Token: 0x17000BE5 RID: 3045
		// (get) Token: 0x060026DD RID: 9949 RVA: 0x000B7EE9 File Offset: 0x000B60E9
		// (set) Token: 0x060026DE RID: 9950 RVA: 0x000B7EFB File Offset: 0x000B60FB
		public DataTemplate HeaderTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(AnnotationToolTip.HeaderTemplateProperty);
			}
			set
			{
				base.SetValue(AnnotationToolTip.HeaderTemplateProperty, value);
			}
		}

		// Token: 0x040010D4 RID: 4308
		public static readonly DependencyProperty HeaderProperty = HeaderedContentControl.HeaderProperty.AddOwner(typeof(AnnotationToolTip));

		// Token: 0x040010D5 RID: 4309
		public static readonly DependencyProperty HeaderTemplateProperty = HeaderedContentControl.HeaderTemplateProperty.AddOwner(typeof(AnnotationToolTip));
	}
}
