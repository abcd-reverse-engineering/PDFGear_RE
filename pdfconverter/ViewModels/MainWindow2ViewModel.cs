using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using pdfconverter.Models;
using pdfconverter.Properties;

namespace pdfconverter.ViewModels
{
	// Token: 0x02000033 RID: 51
	public class MainWindow2ViewModel : ObservableObject
	{
		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060003A9 RID: 937 RVA: 0x0000EA8A File Offset: 0x0000CC8A
		// (set) Token: 0x060003AA RID: 938 RVA: 0x0000EA92 File Offset: 0x0000CC92
		public ObservableCollection<ActionMenuGroup> ActionMenus { get; private set; }

		// Token: 0x060003AB RID: 939 RVA: 0x0000EA9C File Offset: 0x0000CC9C
		public MainWindow2ViewModel()
		{
			this.ActionMenus = new ObservableCollection<ActionMenuGroup>
			{
				new ActionMenuGroup
				{
					Title = Resources.WinMergeSplitLeftMergeTile,
					Tag = "mergepdf"
				},
				new ActionMenuGroup
				{
					Title = Resources.WinMergeSplitLeftSplitTile,
					Tag = "splitpdf"
				},
				new ActionMenuGroup
				{
					Title = Resources.CompressPDFText,
					Tag = "compresspdf"
				},
				new ActionMenuGroup
				{
					Title = Resources.WinListHeaderWordToPDFText,
					Tag = "wordtopdf"
				},
				new ActionMenuGroup
				{
					Title = Resources.WinListHeaderExcelToPDFText,
					Tag = "exceltopdf"
				},
				new ActionMenuGroup
				{
					Title = Resources.WinListHeaderPPTToPDFText,
					Tag = "ppttopdf"
				},
				new ActionMenuGroup
				{
					Title = Resources.WinListHeaderImageToPDFText,
					Tag = "imagetopdf"
				},
				new ActionMenuGroup
				{
					Title = Resources.WinListHeaderRTFToPDFText,
					Tag = "rtftopdf"
				},
				new ActionMenuGroup
				{
					Title = Resources.WinListHeaderTXTToPDFText,
					Tag = "txttopdf"
				}
			};
		}
	}
}
