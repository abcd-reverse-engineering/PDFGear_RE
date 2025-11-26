using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Net;
using pdfeditor.Models;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls
{
	// Token: 0x020001B7 RID: 439
	public partial class FilterByAnnotationsControl : UserControl
	{
		// Token: 0x060018FA RID: 6394 RVA: 0x00060844 File Offset: 0x0005EA44
		public FilterByAnnotationsControl()
		{
			this.InitializeComponent();
			base.IsVisibleChanged += this.FilterByAnnotationsControl_IsVisibleChanged;
		}

		// Token: 0x060018FB RID: 6395 RVA: 0x00060864 File Offset: 0x0005EA64
		private void FilterByAnnotationsControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Collection<AnnotationsFilterModel> collection = (ObservableCollection<AnnotationsFilterModel>)base.GetValue(FilterByAnnotationsControl.TextListProperty);
			this.AllCount = 0;
			foreach (AnnotationsFilterModel annotationsFilterModel in collection)
			{
				this.AllCount += annotationsFilterModel.Count;
			}
		}

		// Token: 0x170009AB RID: 2475
		// (get) Token: 0x060018FC RID: 6396 RVA: 0x000608D0 File Offset: 0x0005EAD0
		// (set) Token: 0x060018FD RID: 6397 RVA: 0x00060940 File Offset: 0x0005EB40
		public ObservableCollection<AnnotationsFilterModel> TextList
		{
			get
			{
				ObservableCollection<AnnotationsFilterModel> observableCollection = (ObservableCollection<AnnotationsFilterModel>)base.GetValue(FilterByAnnotationsControl.TextListProperty);
				this.AllCount = 0;
				foreach (AnnotationsFilterModel annotationsFilterModel in observableCollection)
				{
					this.AllCount += annotationsFilterModel.Count;
				}
				return observableCollection;
			}
			set
			{
				base.SetValue(FilterByAnnotationsControl.TextListProperty, value);
			}
		}

		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x060018FE RID: 6398 RVA: 0x0006094E File Offset: 0x0005EB4E
		// (set) Token: 0x060018FF RID: 6399 RVA: 0x00060960 File Offset: 0x0005EB60
		public int AllCount
		{
			get
			{
				return (int)base.GetValue(FilterByAnnotationsControl.AllCountProperty);
			}
			set
			{
				base.SetValue(FilterByAnnotationsControl.AllCountProperty, value);
			}
		}

		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x06001900 RID: 6400 RVA: 0x00060973 File Offset: 0x0005EB73
		// (set) Token: 0x06001901 RID: 6401 RVA: 0x00060985 File Offset: 0x0005EB85
		public bool? AllCheck
		{
			get
			{
				return (bool?)base.GetValue(FilterByAnnotationsControl.AllCheckProperty);
			}
			set
			{
				base.SetValue(FilterByAnnotationsControl.AllCheckProperty, value);
			}
		}

		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x06001902 RID: 6402 RVA: 0x00060998 File Offset: 0x0005EB98
		// (set) Token: 0x06001903 RID: 6403 RVA: 0x000609AA File Offset: 0x0005EBAA
		public PdfDocument Document
		{
			get
			{
				return (PdfDocument)base.GetValue(FilterByAnnotationsControl.DocumentProperty);
			}
			set
			{
				base.SetValue(FilterByAnnotationsControl.DocumentProperty, value);
			}
		}

		// Token: 0x06001904 RID: 6404 RVA: 0x000609B8 File Offset: 0x0005EBB8
		private void fileItemListSelectAll(object sender, RoutedEventArgs e)
		{
			if (this.TextList != null && this.TextList.Count > 0)
			{
				foreach (AnnotationsFilterModel annotationsFilterModel in this.TextList)
				{
					annotationsFilterModel.IsSelect = true;
				}
			}
			Ioc.Default.GetRequiredService<MainViewModel>().FilterAnnotations();
		}

		// Token: 0x06001905 RID: 6405 RVA: 0x00060A28 File Offset: 0x0005EC28
		private void fileItemListSelectNone(object sender, RoutedEventArgs e)
		{
			if (this.TextList != null && this.TextList.Count > 0)
			{
				foreach (AnnotationsFilterModel annotationsFilterModel in this.TextList)
				{
					annotationsFilterModel.IsSelect = false;
				}
			}
			Ioc.Default.GetRequiredService<MainViewModel>().FilterAnnotations();
		}

		// Token: 0x06001906 RID: 6406 RVA: 0x00060A98 File Offset: 0x0005EC98
		private void fileItemChecked(object sender, RoutedEventArgs e)
		{
			if (this.AllCheck.GetValueOrDefault())
			{
				e.Handled = true;
			}
			this.JudgeSelectall();
			Ioc.Default.GetRequiredService<MainViewModel>().FilterAnnotations();
		}

		// Token: 0x06001907 RID: 6407 RVA: 0x00060AD4 File Offset: 0x0005ECD4
		private void JudgeSelectall()
		{
			List<AnnotationsFilterModel> list = this.TextList.Where((AnnotationsFilterModel x) => x.IsSelect).ToList<AnnotationsFilterModel>();
			if (list == null)
			{
				this.AllCheck = new bool?(false);
			}
			if (list != null && list.Count == 0)
			{
				this.AllCheck = new bool?(false);
				return;
			}
			if (list != null && list.Count > 0)
			{
				if (list.Count == this.TextList.Count)
				{
					this.AllCheck = new bool?(true);
					return;
				}
				this.AllCheck = null;
			}
		}

		// Token: 0x06001908 RID: 6408 RVA: 0x00060B74 File Offset: 0x0005ED74
		private void fileItemUnchecked(object sender, RoutedEventArgs e)
		{
			bool? allCheck = this.AllCheck;
			bool flag = false;
			if ((allCheck.GetValueOrDefault() == flag) & (allCheck != null))
			{
				e.Handled = true;
			}
			this.JudgeSelectall();
			Ioc.Default.GetRequiredService<MainViewModel>().FilterAnnotations();
		}

		// Token: 0x06001909 RID: 6409 RVA: 0x00060BBA File Offset: 0x0005EDBA
		private void fileItemCB_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x0600190A RID: 6410 RVA: 0x00060BBC File Offset: 0x0005EDBC
		private void OnListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x0600190D RID: 6413 RVA: 0x00060C30 File Offset: 0x0005EE30
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 2:
			{
				EventSetter eventSetter = new EventSetter();
				eventSetter.Event = Control.MouseDoubleClickEvent;
				eventSetter.Handler = new MouseButtonEventHandler(this.OnListViewItemDoubleClick);
				((Style)target).Setters.Add(eventSetter);
				return;
			}
			case 3:
				break;
			case 4:
				((CheckBox)target).Checked += this.fileItemListSelectAll;
				((CheckBox)target).Unchecked += this.fileItemListSelectNone;
				return;
			case 5:
				((CheckBox)target).Checked += this.fileItemChecked;
				((CheckBox)target).Unchecked += this.fileItemUnchecked;
				((CheckBox)target).PreviewMouseDoubleClick += this.fileItemCB_PreviewMouseDoubleClick;
				break;
			default:
				return;
			}
		}

		// Token: 0x0400085F RID: 2143
		public static readonly DependencyProperty TextListProperty = DependencyProperty.Register("TextList", typeof(ObservableCollection<AnnotationsFilterModel>), typeof(FilterByAnnotationsControl), new PropertyMetadata(null));

		// Token: 0x04000860 RID: 2144
		public static readonly DependencyProperty AllCountProperty = DependencyProperty.Register("AllCount", typeof(int), typeof(FilterByAnnotationsControl), new PropertyMetadata(null));

		// Token: 0x04000861 RID: 2145
		public static readonly DependencyProperty AllCheckProperty = DependencyProperty.Register("AllCheck", typeof(bool?), typeof(FilterByAnnotationsControl), new PropertyMetadata(null));

		// Token: 0x04000862 RID: 2146
		public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(PdfDocument), typeof(FilterByAnnotationsControl), new PropertyMetadata(null));
	}
}
