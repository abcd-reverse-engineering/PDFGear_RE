using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
using Newtonsoft.Json;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.AcroForms;
using Patagames.Pdf.Net.Actions;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Controls;
using pdfeditor.Controls.Annotations;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Controls.DigitalSignatures;
using pdfeditor.Controls.Menus;
using pdfeditor.Controls.Signature;
using pdfeditor.Controls.Stamp;
using pdfeditor.Controls.Watermark;
using pdfeditor.Models;
using pdfeditor.Models.Annotations;
using pdfeditor.Models.LeftNavigations;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.Models.Protection;
using pdfeditor.Models.Viewer;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.Utils.DigitalSignatures;
using pdfeditor.Utils.Enums;
using pdfeditor.Views;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.DigitalSignatures;
using PDFKit.Utils.PageContents;
using PDFKit.Utils.StampUtils;
using PDFKit.Utils.WatermarkUtils;

namespace pdfeditor.ViewModels
{
	// Token: 0x02000053 RID: 83
	public class AnnotationToolbarViewModel : ObservableObject
	{
		// Token: 0x06000422 RID: 1058 RVA: 0x00015736 File Offset: 0x00013936
		public AnnotationToolbarViewModel(MainViewModel mainViewModel)
		{
			this.mainViewModel = mainViewModel;
			this.InitToolbarAnnotationButtonModel();
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000423 RID: 1059 RVA: 0x0001574C File Offset: 0x0001394C
		public AnnotationMenuPropertyAccessor AnnotationMenuPropertyAccessor
		{
			get
			{
				AnnotationMenuPropertyAccessor annotationMenuPropertyAccessor;
				if ((annotationMenuPropertyAccessor = this.annotationMenuPropertyAccessor) == null)
				{
					annotationMenuPropertyAccessor = (this.annotationMenuPropertyAccessor = new AnnotationMenuPropertyAccessor(this));
				}
				return annotationMenuPropertyAccessor;
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000424 RID: 1060 RVA: 0x00015772 File Offset: 0x00013972
		// (set) Token: 0x06000425 RID: 1061 RVA: 0x0001577A File Offset: 0x0001397A
		public TypedContextMenuModel StampMenuItems
		{
			get
			{
				return this.stampMenuItems;
			}
			set
			{
				base.SetProperty<TypedContextMenuModel>(ref this.stampMenuItems, value, "StampMenuItems");
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000426 RID: 1062 RVA: 0x0001578F File Offset: 0x0001398F
		// (set) Token: 0x06000427 RID: 1063 RVA: 0x00015797 File Offset: 0x00013997
		public TypedContextMenuModel SignatureMenuItems
		{
			get
			{
				return this.signatureMenuItems;
			}
			set
			{
				base.SetProperty<TypedContextMenuModel>(ref this.signatureMenuItems, value, "SignatureMenuItems");
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000428 RID: 1064 RVA: 0x000157AC File Offset: 0x000139AC
		// (set) Token: 0x06000429 RID: 1065 RVA: 0x000157B4 File Offset: 0x000139B4
		public TypedContextMenuModel WatermakMenuItems
		{
			get
			{
				return this.waterMenuItems;
			}
			set
			{
				base.SetProperty<TypedContextMenuModel>(ref this.waterMenuItems, value, "WatermakMenuItems");
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x0600042A RID: 1066 RVA: 0x000157C9 File Offset: 0x000139C9
		// (set) Token: 0x0600042B RID: 1067 RVA: 0x000157D1 File Offset: 0x000139D1
		public WatermarkAnnonationModel WatermarkModel
		{
			get
			{
				return this.watermarkModel;
			}
			set
			{
				base.SetProperty<WatermarkAnnonationModel>(ref this.watermarkModel, value, "WatermarkModel");
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600042C RID: 1068 RVA: 0x000157E6 File Offset: 0x000139E6
		// (set) Token: 0x0600042D RID: 1069 RVA: 0x000157EE File Offset: 0x000139EE
		public WatermarkParam WatermarkParam
		{
			get
			{
				return this.watermarkParam;
			}
			set
			{
				base.SetProperty<WatermarkParam>(ref this.watermarkParam, value, "WatermarkParam");
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x0600042E RID: 1070 RVA: 0x00015803 File Offset: 0x00013A03
		// (set) Token: 0x0600042F RID: 1071 RVA: 0x0001580B File Offset: 0x00013A0B
		public WatermarkImageModel ImageWatermarkModel
		{
			get
			{
				return this.imageWatermarkModel;
			}
			set
			{
				base.SetProperty<WatermarkImageModel>(ref this.imageWatermarkModel, value, "ImageWatermarkModel");
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000430 RID: 1072 RVA: 0x00015820 File Offset: 0x00013A20
		// (set) Token: 0x06000431 RID: 1073 RVA: 0x00015828 File Offset: 0x00013A28
		public WatermarkTextModel TextWatermarkModel
		{
			get
			{
				return this.textWatermarkModel;
			}
			set
			{
				base.SetProperty<WatermarkTextModel>(ref this.textWatermarkModel, value, "TextWatermarkModel");
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000432 RID: 1074 RVA: 0x0001583D File Offset: 0x00013A3D
		// (set) Token: 0x06000433 RID: 1075 RVA: 0x00015845 File Offset: 0x00013A45
		public ToolbarAnnotationButtonModel UnderlineButtonModel
		{
			get
			{
				return this.underlineButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.underlineButtonModel, value, "UnderlineButtonModel");
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x0001585A File Offset: 0x00013A5A
		// (set) Token: 0x06000435 RID: 1077 RVA: 0x00015862 File Offset: 0x00013A62
		public ToolbarAnnotationButtonModel StrikeButtonModel
		{
			get
			{
				return this.strikeButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.strikeButtonModel, value, "StrikeButtonModel");
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x00015877 File Offset: 0x00013A77
		// (set) Token: 0x06000437 RID: 1079 RVA: 0x0001587F File Offset: 0x00013A7F
		public ToolbarAnnotationButtonModel HighlightButtonModel
		{
			get
			{
				return this.highlightButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.highlightButtonModel, value, "HighlightButtonModel");
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000438 RID: 1080 RVA: 0x00015894 File Offset: 0x00013A94
		// (set) Token: 0x06000439 RID: 1081 RVA: 0x0001589C File Offset: 0x00013A9C
		public ToolbarAnnotationButtonModel LineButtonModel
		{
			get
			{
				return this.lineButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.lineButtonModel, value, "LineButtonModel");
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x000158B1 File Offset: 0x00013AB1
		// (set) Token: 0x0600043B RID: 1083 RVA: 0x000158B9 File Offset: 0x00013AB9
		public ToolbarAnnotationButtonModel InkButtonModel
		{
			get
			{
				return this.inkButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.inkButtonModel, value, "InkButtonModel");
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x000158CE File Offset: 0x00013ACE
		// (set) Token: 0x0600043D RID: 1085 RVA: 0x000158D6 File Offset: 0x00013AD6
		public ToolbarAnnotationButtonModel SquareButtonModel
		{
			get
			{
				return this.squareButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.squareButtonModel, value, "SquareButtonModel");
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x000158EB File Offset: 0x00013AEB
		// (set) Token: 0x0600043F RID: 1087 RVA: 0x000158F3 File Offset: 0x00013AF3
		public ToolbarAnnotationButtonModel CircleButtonModel
		{
			get
			{
				return this.circleButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.circleButtonModel, value, "CircleButtonModel");
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x00015908 File Offset: 0x00013B08
		// (set) Token: 0x06000441 RID: 1089 RVA: 0x00015910 File Offset: 0x00013B10
		public ToolbarAnnotationButtonModel HighlightAreaButtonModel
		{
			get
			{
				return this.highlightareaButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.highlightareaButtonModel, value, "HighlightAreaButtonModel");
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x00015925 File Offset: 0x00013B25
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x0001592D File Offset: 0x00013B2D
		public ToolbarAnnotationButtonModel TextBoxButtonModel
		{
			get
			{
				return this.textBoxButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.textBoxButtonModel, value, "TextBoxButtonModel");
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x00015942 File Offset: 0x00013B42
		// (set) Token: 0x06000445 RID: 1093 RVA: 0x0001594A File Offset: 0x00013B4A
		public ToolbarAnnotationButtonModel TextButtonModel
		{
			get
			{
				return this.textButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.textButtonModel, value, "TextButtonModel");
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x0001595F File Offset: 0x00013B5F
		// (set) Token: 0x06000447 RID: 1095 RVA: 0x00015967 File Offset: 0x00013B67
		public ToolbarAnnotationButtonModel NoteButtonModel
		{
			get
			{
				return this.noteButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.noteButtonModel, value, "NoteButtonModel");
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x0001597C File Offset: 0x00013B7C
		// (set) Token: 0x06000449 RID: 1097 RVA: 0x00015984 File Offset: 0x00013B84
		public ToolbarAnnotationButtonModel StampButtonModel
		{
			get
			{
				return this.stampButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.stampButtonModel, value, "StampButtonModel");
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600044A RID: 1098 RVA: 0x00015999 File Offset: 0x00013B99
		// (set) Token: 0x0600044B RID: 1099 RVA: 0x000159A1 File Offset: 0x00013BA1
		public ToolbarAnnotationButtonModel ShareButtonModel
		{
			get
			{
				return this.shareButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.shareButtonModel, value, "ShareButtonModel");
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x0600044C RID: 1100 RVA: 0x000159B6 File Offset: 0x00013BB6
		// (set) Token: 0x0600044D RID: 1101 RVA: 0x000159BE File Offset: 0x00013BBE
		public ToolbarAnnotationButtonModel ImageButtonModel
		{
			get
			{
				return this.imageButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.imageButtonModel, value, "ImageButtonModel");
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600044E RID: 1102 RVA: 0x000159D3 File Offset: 0x00013BD3
		// (set) Token: 0x0600044F RID: 1103 RVA: 0x000159DB File Offset: 0x00013BDB
		public ToolbarAnnotationButtonModel SignatureButtonModel
		{
			get
			{
				return this.signatureButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.signatureButtonModel, value, "SignatureButtonModel");
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000450 RID: 1104 RVA: 0x000159F0 File Offset: 0x00013BF0
		// (set) Token: 0x06000451 RID: 1105 RVA: 0x000159F8 File Offset: 0x00013BF8
		public ToolbarAnnotationButtonModel WatermarkButtonModel
		{
			get
			{
				return this.watermarkButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.watermarkButtonModel, value, "WatermarkButtonModel");
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000452 RID: 1106 RVA: 0x00015A0D File Offset: 0x00013C0D
		// (set) Token: 0x06000453 RID: 1107 RVA: 0x00015A15 File Offset: 0x00013C15
		public TypedContextMenuModel LinkMenuItems
		{
			get
			{
				return this.linkMenuItems;
			}
			set
			{
				base.SetProperty<TypedContextMenuModel>(ref this.linkMenuItems, value, "LinkMenuItems");
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x00015A2A File Offset: 0x00013C2A
		// (set) Token: 0x06000455 RID: 1109 RVA: 0x00015A32 File Offset: 0x00013C32
		public TypedContextMenuModel TranslateMenuItems
		{
			get
			{
				return this.translateMenuItems;
			}
			set
			{
				base.SetProperty<TypedContextMenuModel>(ref this.translateMenuItems, value, "TranslateMenuItems");
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000456 RID: 1110 RVA: 0x00015A47 File Offset: 0x00013C47
		// (set) Token: 0x06000457 RID: 1111 RVA: 0x00015A4F File Offset: 0x00013C4F
		public ToolbarAnnotationButtonModel LinkButtonModel
		{
			get
			{
				return this.linkButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.linkButtonModel, value, "LinkButtonModel");
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000458 RID: 1112 RVA: 0x00015A64 File Offset: 0x00013C64
		// (set) Token: 0x06000459 RID: 1113 RVA: 0x00015A6C File Offset: 0x00013C6C
		public ToolbarAnnotationButtonModel TranslateButtonModel
		{
			get
			{
				return this.translateButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.translateButtonModel, value, "TranslateButtonModel");
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x0600045A RID: 1114 RVA: 0x00015A81 File Offset: 0x00013C81
		// (set) Token: 0x0600045B RID: 1115 RVA: 0x00015A89 File Offset: 0x00013C89
		public TypedContextMenuModel AttachmentMenuItems
		{
			get
			{
				return this.attachmentMenuItems;
			}
			set
			{
				base.SetProperty<TypedContextMenuModel>(ref this.attachmentMenuItems, value, "AttachmentMenuItems");
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600045C RID: 1116 RVA: 0x00015A9E File Offset: 0x00013C9E
		// (set) Token: 0x0600045D RID: 1117 RVA: 0x00015AA6 File Offset: 0x00013CA6
		public ToolbarAnnotationButtonModel AttachmentButtonModel
		{
			get
			{
				return this.attachmentButtonModel;
			}
			set
			{
				this.SetButtonProperty(ref this.attachmentButtonModel, value, "AttachmentButtonModel");
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x0600045E RID: 1118 RVA: 0x00015ABB File Offset: 0x00013CBB
		// (set) Token: 0x0600045F RID: 1119 RVA: 0x00015AC3 File Offset: 0x00013CC3
		public ToolbarAnnotationButtonModel DigitalSignatureButtonModel
		{
			get
			{
				return this.digitalSignatureButtonModel;
			}
			set
			{
				base.SetProperty<ToolbarAnnotationButtonModel>(ref this.digitalSignatureButtonModel, value, "DigitalSignatureButtonModel");
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000460 RID: 1120 RVA: 0x00015AD8 File Offset: 0x00013CD8
		// (set) Token: 0x06000461 RID: 1121 RVA: 0x00015AE0 File Offset: 0x00013CE0
		public ToolbarAnnotationButtonModel AddDeferredDigitalSignatureButtonModel
		{
			get
			{
				return this.addDeferredDigitalSignatureButtonModel;
			}
			set
			{
				base.SetProperty<ToolbarAnnotationButtonModel>(ref this.addDeferredDigitalSignatureButtonModel, value, "AddDeferredDigitalSignatureButtonModel");
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x00015AF5 File Offset: 0x00013CF5
		// (set) Token: 0x06000463 RID: 1123 RVA: 0x00015AFD File Offset: 0x00013CFD
		public ToolbarAnnotationButtonModel CertificateManagerButtonModel
		{
			get
			{
				return this.certificateManagerButtonModel;
			}
			set
			{
				base.SetProperty<ToolbarAnnotationButtonModel>(ref this.certificateManagerButtonModel, value, "CertificateManagerButtonModel");
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x00015B12 File Offset: 0x00013D12
		// (set) Token: 0x06000465 RID: 1125 RVA: 0x00015B1A File Offset: 0x00013D1A
		public ToolbarAnnotationButtonModel RedactionButtonModel
		{
			get
			{
				return this.redactionButtonModel;
			}
			set
			{
				base.SetProperty<ToolbarAnnotationButtonModel>(ref this.redactionButtonModel, value, "RedactionButtonModel");
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x00015B30 File Offset: 0x00013D30
		public AsyncRelayCommand AddFormControlCheckCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.addFormControlCheckCmd) == null)
				{
					asyncRelayCommand = (this.addFormControlCheckCmd = new AsyncRelayCommand(async delegate
					{
						await this.AddFormControlAsync("Check");
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000467 RID: 1127 RVA: 0x00015B64 File Offset: 0x00013D64
		public AsyncRelayCommand AddFormControlCancelCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.addFormControlCancelCmd) == null)
				{
					asyncRelayCommand = (this.addFormControlCancelCmd = new AsyncRelayCommand(async delegate
					{
						await this.AddFormControlAsync("Cancel");
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x00015B98 File Offset: 0x00013D98
		public AsyncRelayCommand AddFormControlRadioCheckCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.addFormControlRadioCheckCmd) == null)
				{
					asyncRelayCommand = (this.addFormControlRadioCheckCmd = new AsyncRelayCommand(async delegate
					{
						await this.AddFormControlAsync("RadioCheck");
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x00015BCC File Offset: 0x00013DCC
		public AsyncRelayCommand AddFormControlCheckBoxCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.addFormControlCheckBoxCmd) == null)
				{
					asyncRelayCommand = (this.addFormControlCheckBoxCmd = new AsyncRelayCommand(async delegate
					{
						await this.AddFormControlAsync("CheckBox");
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x00015C00 File Offset: 0x00013E00
		public AsyncRelayCommand AddFormControlIndeterminateCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.addFormControlIndeterminateCmd) == null)
				{
					asyncRelayCommand = (this.addFormControlIndeterminateCmd = new AsyncRelayCommand(async delegate
					{
						await this.AddFormControlAsync("Indeterminate");
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600046B RID: 1131 RVA: 0x00015C34 File Offset: 0x00013E34
		public AsyncRelayCommand AddFormControlIndeterminateFillCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.addFormControlIndeterminateFillCmd) == null)
				{
					asyncRelayCommand = (this.addFormControlIndeterminateFillCmd = new AsyncRelayCommand(async delegate
					{
						await this.AddFormControlAsync("Indeterminate Fill");
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x00015C68 File Offset: 0x00013E68
		private async Task AddFormControlAsync(string name)
		{
			global::PDFKit.PdfControl pdfEditor = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			AnnotationHolderManager holder = PdfObjectExtensions.GetAnnotationHolderManager(pdfEditor);
			if (holder != null)
			{
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				this.mainViewModel.AnnotationMode = AnnotationMode.None;
				holder.CancelAll();
				await holder.WaitForCancelCompletedAsync();
				StampFormControlModel stampFormControlModel = new StampFormControlModel
				{
					Name = name
				};
				global::System.Windows.Size size;
				Geometry geometry = StampUtil.CreateStampFormControlPreviewGrometry(name, out size);
				global::System.Windows.Shapes.Path path = new global::System.Windows.Shapes.Path
				{
					Data = geometry,
					Fill = global::System.Windows.Media.Brushes.Black,
					Width = size.Width,
					Height = size.Height,
					UseLayoutRounding = false,
					SnapsToDevicePixels = false
				};
				MainViewModel mainViewModel = this.mainViewModel;
				global::PDFKit.PdfControl pdfControl = pdfEditor;
				mainViewModel.ViewerOperationModel = new HoverOperationModel((pdfControl != null) ? pdfControl.Viewer : null)
				{
					Data = stampFormControlModel,
					SizeInDocument = new FS_SIZEF(stampFormControlModel.Width, stampFormControlModel.Height),
					PreviewElement = new Grid
					{
						Children = 
						{
							new Viewbox
							{
								Child = path
							},
							new global::System.Windows.Shapes.Rectangle
							{
								Stroke = global::System.Windows.Media.Brushes.Blue,
								StrokeThickness = 2.0,
								StrokeDashArray = { 2.5, 1.5 },
								Opacity = 0.6,
								UseLayoutRounding = false,
								SnapsToDevicePixels = false
							}
						}
					}
				};
				ViewOperationResult<bool> viewOperationResult = await this.mainViewModel.ViewerOperationModel.Task;
				DataOperationModel viewerOperationModel = this.mainViewModel.ViewerOperationModel;
				if (viewOperationResult != null && viewOperationResult.Value)
				{
					holder.CancelAll();
					holder.Stamp.StartCreateNew(this.mainViewModel.Document.Pages[viewerOperationModel.CurrentPage], viewerOperationModel.PositionFromDocument);
					global::System.Collections.Generic.IReadOnlyList<PdfStampAnnotation> readOnlyList = await holder.Stamp.CompleteCreateNewAsync();
					if (readOnlyList != null && readOnlyList.Count > 0)
					{
						holder.Select(readOnlyList[0], true);
					}
				}
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600046D RID: 1133 RVA: 0x00015CB3 File Offset: 0x00013EB3
		// (set) Token: 0x0600046E RID: 1134 RVA: 0x00015CBB File Offset: 0x00013EBB
		public DateTime StampImgFileOkTime { get; set; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600046F RID: 1135 RVA: 0x00015CC4 File Offset: 0x00013EC4
		public global::System.Collections.Generic.IReadOnlyList<ToolbarAnnotationButtonModel> AllAnnotationButton
		{
			get
			{
				return this.allAnnotationButton;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x00015CCC File Offset: 0x00013ECC
		public ToolbarSettingModel CheckedButtonToolbarSetting
		{
			get
			{
				if (this.mainViewModel.ViewToolbar.EditDocumentToolbarSetting != null)
				{
					return this.mainViewModel.ViewToolbar.EditDocumentToolbarSetting;
				}
				if (this.mainViewModel.SelectedAnnotation != null)
				{
					List<AnnotationMode> modes = AnnotationFactory.GetAnnotationModes(this.mainViewModel.SelectedAnnotation).ToList<AnnotationMode>();
					if (modes.Count > 0)
					{
						ToolbarSettingModel[] array = (from c in this.AllAnnotationButton
							where modes.Contains(c.Mode) && c.ToolbarSettingModel != null
							select c.ToolbarSettingModel into c
							orderby modes.IndexOf(c.Id.AnnotationMode)
							select c).ToArray<ToolbarSettingModel>();
						if (array.Length != 0)
						{
							return array[0];
						}
					}
				}
				ToolbarAnnotationButtonModel toolbarAnnotationButtonModel = this.AllAnnotationButton.FirstOrDefault((ToolbarAnnotationButtonModel c) => c.IsCheckable && c.IsChecked);
				if (toolbarAnnotationButtonModel == null)
				{
					return null;
				}
				return toolbarAnnotationButtonModel.ToolbarSettingModel;
			}
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x00015DD0 File Offset: 0x00013FD0
		public void UpdateViewerToobarPadding()
		{
			PdfDocument document = this.mainViewModel.Document;
			if (document == null)
			{
				return;
			}
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(document);
			ScrollViewer scrollViewer = ((pdfControl != null) ? pdfControl.ScrollViewer : null);
			FrameworkElement frameworkElement = ((pdfControl != null) ? pdfControl.Parent : null) as FrameworkElement;
			if (frameworkElement == null || scrollViewer == null)
			{
				return;
			}
			TranslateTransform translateTransform = frameworkElement.RenderTransform as TranslateTransform;
			if (translateTransform == null)
			{
				translateTransform = new TranslateTransform();
				frameworkElement.RenderTransform = translateTransform;
			}
			if (this.mainViewModel.ViewToolbar.SubViewModeContinuous == SubViewModeContinuous.Discontinuous)
			{
				translateTransform.Y = 0.0;
				if (this.CheckedButtonToolbarSetting != null && scrollViewer.VerticalOffset < 1.0)
				{
					double num = 43.0;
					double? num2 = AnnotationToolbarViewModel.<UpdateViewerToobarPadding>g__GetToolbarSettingHeight|172_0(frameworkElement);
					if (num2 != null)
					{
						num = 10.0 + num2.Value;
					}
					pdfControl.PagePadding = new Thickness(10.0, num, 10.0, 10.0);
					scrollViewer.ScrollToTop();
					scrollViewer.UpdateLayout();
					return;
				}
				pdfControl.PagePadding = new Thickness(10.0, 10.0, 10.0, 10.0);
				return;
			}
			else
			{
				pdfControl.PagePadding = new Thickness(10.0, 10.0, 10.0, 10.0);
				if (this.CheckedButtonToolbarSetting == null || scrollViewer.VerticalOffset >= 1.0)
				{
					translateTransform.Y = 0.0;
					return;
				}
				double? num3 = AnnotationToolbarViewModel.<UpdateViewerToobarPadding>g__GetToolbarSettingHeight|172_0(frameworkElement);
				if (num3 != null)
				{
					translateTransform.Y = num3.Value;
					return;
				}
				translateTransform.Y = 33.0;
				return;
			}
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x00015F98 File Offset: 0x00014198
		private void DoMenuItemCmd(ContextMenuItemModel model)
		{
			TypedContextMenuItemModel typedContextMenuItemModel = model.Parent as TypedContextMenuItemModel;
			if (typedContextMenuItemModel != null)
			{
				TypedContextMenuModel typedContextMenuModel = typedContextMenuItemModel.Parent as TypedContextMenuModel;
				if (typedContextMenuModel != null)
				{
					try
					{
						GAManager.SendEvent("AnnotationMenuClick", typedContextMenuModel.Mode.ToString(), typedContextMenuItemModel.Type.ToString(), 1L);
					}
					catch
					{
					}
					if (model is ColorMoreItemContextMenuItemModel)
					{
						this.ClearAdditionalMenuItem();
						ContextMenuItemModel contextMenuItemModel = ToolbarContextMenuHelper.CreateContextMenuItem(typedContextMenuModel.Mode, typedContextMenuItemModel.Type, model.TagData.MenuItemValue, true, new Action<ContextMenuItemModel>(this.DoMenuItemCmd));
						if (contextMenuItemModel != null)
						{
							int num = typedContextMenuItemModel.IndexOf(model);
							typedContextMenuItemModel.Insert(num, contextMenuItemModel);
							contextMenuItemModel.IsChecked = true;
						}
					}
					string text = AnnotationMenuPropertyAccessor.BuildPropertyName(typedContextMenuModel.Mode, typedContextMenuItemModel.Type);
					global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
					if (pdfControl != null)
					{
						AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
						int num2 = -1;
						if (!((annotationHolderManager != null) ? new bool?(annotationHolderManager.OnPropertyChanged(text, out num2)) : null).GetValueOrDefault() && typedContextMenuModel != null)
						{
							ToolbarChildCheckableButtonModel owner = typedContextMenuModel.Owner;
							if (owner != null)
							{
								ToolbarButtonModel parent = owner.Parent;
								if (parent != null)
								{
									parent.Tap();
								}
							}
						}
					}
					TagDataModel tagData = model.TagData;
					if (tagData != null && tagData.IsTransient)
					{
						int num3 = typedContextMenuItemModel.IndexOf(model);
						ContextMenuItemModel contextMenuItemModel2 = ToolbarContextMenuHelper.CreateContextMenuItem(typedContextMenuModel.Mode, typedContextMenuItemModel.Type, model.TagData.MenuItemValue, false, new Action<ContextMenuItemModel>(this.DoMenuItemCmd));
						if (contextMenuItemModel2 != null)
						{
							typedContextMenuItemModel[num3] = contextMenuItemModel2;
							contextMenuItemModel2.IsChecked = true;
						}
					}
				}
			}
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x00016140 File Offset: 0x00014340
		private void InitMenu()
		{
			List<DynamicStampTextModel> list = ToolbarContextMenuHelper.ReadDynamicStamp(true);
			this.stampMenuItems = new TypedContextMenuModel(AnnotationMode.Stamp)
			{
				ToolbarContextMenuHelper.CreteStampMenu(AnnotationMode.Stamp, new Action<ContextMenuItemModel>(this.DoStampCmd)),
				ToolbarContextMenuHelper.CreteManageStampMenu(AnnotationMode.Stamp, new Action<ContextMenuItemModel>(this.DoManageStampCmd))
			};
			if (list != null && list.Count > 0)
			{
				this.stampMenuItems.Add(new ContextMenuSeparator());
				List<string> list2 = new List<string>();
				for (int i = 0; i < list.Count; i++)
				{
					DynamicStampTextModel dynamicStampTextModel = list[i];
					if (!list2.Contains(dynamicStampTextModel.GroupName))
					{
						list2.Add(dynamicStampTextModel.GroupName);
						this.StampMenuItems.Add(ToolbarContextMenuHelper.ManageStampMenu(dynamicStampTextModel.GroupName, AnnotationMode.Stamp, new Action<ContextMenuItemModel>(this.DoStampCmd)));
					}
					if (i == list.Count - 1)
					{
						this.stampMenuItems.Add(new ContextMenuSeparator());
						this.StampMenuItems.Add(ToolbarContextMenuHelper.CreatePresetsMenu(AnnotationMode.Stamp, new Action<ContextMenuItemModel>(this.DoStampPresetsCmd)));
					}
				}
			}
			else
			{
				this.stampMenuItems.Add(new ContextMenuSeparator());
				this.StampMenuItems.Add(ToolbarContextMenuHelper.CreatePresetsMenu(AnnotationMode.Stamp, new Action<ContextMenuItemModel>(this.DoStampPresetsCmd)));
			}
			this.stampMenuItems2 = new TypedContextMenuModel(AnnotationMode.Stamp)
			{
				ToolbarContextMenuHelper.CreatePresetsMenu(AnnotationMode.Stamp, new Action<ContextMenuItemModel>(this.DoStampPresetsCmd)),
				ToolbarContextMenuHelper.CreteStampMenu(AnnotationMode.Stamp, new Action<ContextMenuItemModel>(this.DoStampCmd))
			};
			this.shareMenuItem = new TypedContextMenuModel(AnnotationMode.None)
			{
				ToolbarContextMenuHelper.CreateShareEmailMenu(AnnotationMode.None, new Action<ContextMenuItemModel>(this.SharebyEmailCmd)),
				ToolbarContextMenuHelper.CreateShareFileMenu(AnnotationMode.None, new Action<ContextMenuItemModel>(this.SharebyFileCmd))
			};
			this.signatureMenuItems = new TypedContextMenuModel(AnnotationMode.Signature) { ToolbarContextMenuHelper.CreateSignatureMenu(AnnotationMode.Signature, new Action<ContextMenuItemModel>(this.DoSignatureMenuCmd)) };
			this.signatureMenuItems2 = new TypedContextMenuModel(AnnotationMode.Signature) { ToolbarContextMenuHelper.CreateSignatureMenu(AnnotationMode.Signature, new Action<ContextMenuItemModel>(this.DoSignatureMenuCmd)) };
			this.signatureMenuItems3 = new TypedContextMenuModel(AnnotationMode.Signature) { ToolbarContextMenuHelper.CreateSignatureMenu(AnnotationMode.Signature, new Action<ContextMenuItemModel>(this.DoSignatureMenuCmd)) };
			this.waterMenuItems = new TypedContextMenuModel(AnnotationMode.Watermark)
			{
				ToolbarContextMenuHelper.CreateAddWatermarkMenu(AnnotationMode.Watermark, new Action<ContextMenuItemModel>(this.DoWatermarkInsertCmd)),
				ToolbarContextMenuHelper.CreateDeleteAllWatermarkMenu(AnnotationMode.Watermark, delegate(ContextMenuItemModel m)
				{
					this.DoWatermarkDelCmd(m, true);
				})
			};
			TypedContextMenuModel typedContextMenuModel = new TypedContextMenuModel(AnnotationMode.None);
			ContextMenuItemModel contextMenuItemModel = new ContextMenuItemModel();
			contextMenuItemModel.Name = "SignatureCertificates";
			contextMenuItemModel.Caption = Resources.ResourceManager.GetString("CertificateManager_DigitalIDs_TabName");
			contextMenuItemModel.IsCheckable = false;
			contextMenuItemModel.Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/DigitalSignature/SignCertMenuIcon.png"), new Uri("pack://application:,,,/Style/DarkModeResources/DigitalSignature/SignCertMenuIcon.png"));
			contextMenuItemModel.Command = new RelayCommand(delegate
			{
				GAManager.SendEvent("DigitalSignature", "ShowDigitalIdWindow", "Toolbar", 1L);
				new CertificateManagerWindow(CertificateManagerWindow.DefaultSelectedTab.SignatureCertificates)
				{
					Owner = App.Current.MainWindow,
					WindowStartupLocation = WindowStartupLocation.CenterOwner
				}.ShowDialog();
			});
			typedContextMenuModel.Add(contextMenuItemModel);
			ContextMenuItemModel contextMenuItemModel2 = new ContextMenuItemModel();
			contextMenuItemModel2.Name = "VerificationCertificates";
			contextMenuItemModel2.Caption = Resources.ResourceManager.GetString("CertificateManager_Trust_TabName");
			contextMenuItemModel2.IsCheckable = false;
			contextMenuItemModel2.Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/DigitalSignature/ValidCertMenuIcon.png"), new Uri("pack://application:,,,/Style/DarkModeResources/DigitalSignature/ValidCertMenuIcon.png"));
			contextMenuItemModel2.Command = new RelayCommand(delegate
			{
				GAManager.SendEvent("DigitalSignature", "ShowTrustCertWindow", "Toolbar", 1L);
				new CertificateManagerWindow(CertificateManagerWindow.DefaultSelectedTab.VerificationCertificates)
				{
					Owner = App.Current.MainWindow,
					WindowStartupLocation = WindowStartupLocation.CenterOwner
				}.ShowDialog();
			});
			typedContextMenuModel.Add(contextMenuItemModel2);
			this.certManagerItems = typedContextMenuModel;
			this.linkMenuItems = new TypedContextMenuModel(AnnotationMode.Link)
			{
				ToolbarContextMenuHelper.CreateAddLinkMenu(AnnotationMode.Link, delegate(ContextMenuItemModel m)
				{
					this.LinkCmd(m, true);
				}),
				ToolbarContextMenuHelper.CreateDeleteAllLinkMenu(AnnotationMode.Link, delegate(ContextMenuItemModel m)
				{
					this.DoLinkDelCmd(m);
				})
			};
			this.translateMenuItems = new TypedContextMenuModel(AnnotationMode.None)
			{
				ToolbarContextMenuHelper.TranslateForWordsMenu(AnnotationMode.None, delegate(ContextMenuItemModel m)
				{
					this.DoTranslateForWordsCmd(m);
				}),
				ToolbarContextMenuHelper.OpenTranslateMenu(AnnotationMode.None, delegate(ContextMenuItemModel m)
				{
					this.DoOpenTranslationCmd(m);
				})
			};
			this.attachmentMenuItems = new TypedContextMenuModel(AnnotationMode.Attachment)
			{
				ToolbarContextMenuHelper.CreateAddAttachmentMenu(AnnotationMode.Attachment, new Action<ContextMenuItemModel>(this.DoAttachmentInsertCmd)),
				ToolbarContextMenuHelper.OpenAttachmentManagerMenu(AnnotationMode.Attachment, new Action<ContextMenuItemModel>(this.OpenAttachmentManagerCmd))
			};
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x0001656C File Offset: 0x0001476C
		public async void DoSignatureMenuCmd(ContextMenuItemModel model)
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (this.mainViewModel.Document != null)
			{
				SignatureCreateWin signatureCreateWin = new SignatureCreateWin();
				bool? flag = signatureCreateWin.ShowDialog();
				bool flag2 = false;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					this.mainViewModel.AnnotationMode = AnnotationMode.None;
				}
				else
				{
					this.mainViewModel.AnnotationMode = AnnotationMode.Signature;
					StampImageModel stampImageModel = new StampImageModel
					{
						ImageFilePath = signatureCreateWin.ResultModel.ImageFilePath,
						RemoveBackground = signatureCreateWin.ResultModel.RemoveBackground,
						IsSignature = true
					};
					await this.ProcessStampImageModelAsync(stampImageModel);
					this.mainViewModel.AnnotationMode = AnnotationMode.None;
				}
			}
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x000165A4 File Offset: 0x000147A4
		private async void DoStampAddImgCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document != null)
			{
				this.mainViewModel.AnnotationMode = AnnotationMode.Stamp;
				DataOperationModel viewerOperationModel = this.mainViewModel.ViewerOperationModel;
				if (viewerOperationModel != null)
				{
					viewerOperationModel.Dispose();
				}
				OpenFileDialog openFileDialog = new OpenFileDialog
				{
					Filter = "All Image Files|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;*.tif;*.tiff|Windows Bitmap(*.bmp)|*.bmp|Windows Icon(*.ico)|*.ico|Graphics Interchange Format (*.gif)|(*.gif)|JPEG File Interchange Format (*.jpg)|*.jpg;*.jpeg|Portable Network Graphics (*.png)|*.png|Tag Image File Format (*.tif)|*.tif;*.tiff",
					ShowReadOnly = false,
					ReadOnlyChecked = true
				};
				if (openFileDialog.ShowDialog(Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>()).GetValueOrDefault() && !string.IsNullOrEmpty(openFileDialog.FileName))
				{
					this.StampImgFileOkTime = DateTime.Now;
					this.mainViewModel.AnnotationMode = AnnotationMode.None;
					StampImageModel stampImageModel = new StampImageModel
					{
						ImageFilePath = openFileDialog.FileName
					};
					await this.ProcessImageModelAsync(stampImageModel);
				}
				else
				{
					this.mainViewModel.AnnotationMode = AnnotationMode.None;
				}
			}
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x000165DC File Offset: 0x000147DC
		public async Task ProcessImageModelAsync(StampImageModel model)
		{
			AnnotationToolbarViewModel.<>c__DisplayClass177_0 CS$<>8__locals1 = new AnnotationToolbarViewModel.<>c__DisplayClass177_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.model = model;
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			PdfViewer viewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			AnnotationHolderManager holder = PdfObjectExtensions.GetAnnotationHolderManager(viewer);
			if (holder != null)
			{
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				holder.CancelAll();
				await holder.WaitForCancelCompletedAsync();
				ImageStampModel imageStampModel = new ImageStampModel();
				WriteableBitmap bitmapImage = null;
				try
				{
					using (FileStream fileStream = File.OpenRead(CS$<>8__locals1.model.ImageFilePath))
					{
						BitmapImage bitmapImage2 = new BitmapImage();
						bitmapImage2.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage2.BeginInit();
						bitmapImage2.StreamSource = fileStream;
						bitmapImage2.EndInit();
						bitmapImage = new WriteableBitmap(bitmapImage2);
					}
				}
				catch
				{
					DrawUtils.ShowUnsupportedImageMessage();
					return;
				}
				imageStampModel.StampImageSource = bitmapImage;
				global::System.Windows.Size size = default(global::System.Windows.Size);
				PdfPage pdfPage = this.mainViewModel.Document.Pages.CurrentPage;
				FS_RECTF effectiveBox = pdfPage.GetEffectiveBox(pdfPage.Rotation, false);
				size = StampAnnotationHolder.GetStampPageSize((double)bitmapImage.PixelWidth, (double)bitmapImage.PixelHeight, effectiveBox);
				imageStampModel.ImageWidth = size.Width;
				imageStampModel.ImageHeight = size.Height;
				imageStampModel.PageSize = new FS_SIZEF(size.Width, size.Height);
				StampAnnotationMoveControl stampAnnotationMoveControl = new StampAnnotationMoveControl(imageStampModel)
				{
					Width = size.Width,
					Height = size.Height
				};
				CS$<>8__locals1.model.ImageStampControlModel = imageStampModel;
				this.mainViewModel.ViewerOperationModel = new HoverOperationModel(viewer)
				{
					Data = CS$<>8__locals1.model,
					SizeInDocument = new FS_SIZEF(size.Width, size.Height),
					PreviewElement = new Viewbox
					{
						Child = stampAnnotationMoveControl
					}
				};
				ViewOperationResult<bool> viewOperationResult = await this.mainViewModel.ViewerOperationModel.Task;
				DataOperationModel viewerOperationModel = this.mainViewModel.ViewerOperationModel;
				if (viewOperationResult != null && viewOperationResult.Value)
				{
					pdfPage = this.mainViewModel.Document.Pages[viewerOperationModel.CurrentPage];
					holder.CancelAll();
					CS$<>8__locals1.rect = this.InsertImageSizeMethod(viewerOperationModel, bitmapImage, pdfPage);
					CS$<>8__locals1.pageindex = pdfPage.PageIndex;
					global::System.Drawing.Image image = global::System.Drawing.Image.FromFile(CS$<>8__locals1.model.ImageFilePath);
					PdfBitmap pdfBitmap = ImageToPDFBitmapHelper.CreatePdfBitmapFromFile((Bitmap)image, pdfPage.Rotation);
					if (pdfBitmap == null)
					{
						DrawUtils.ShowUnsupportedImageMessage();
					}
					else
					{
						if (pdfPage.Rotation == PageRotate.Rotate90)
						{
							image.RotateFlip(RotateFlipType.Rotate270FlipNone);
						}
						else if (pdfPage.Rotation == PageRotate.Rotate270)
						{
							image.RotateFlip(RotateFlipType.Rotate90FlipNone);
						}
						else if (pdfPage.Rotation == PageRotate.Rotate180)
						{
							image.RotateFlip(RotateFlipType.Rotate180FlipNone);
						}
						try
						{
							if (new FileInfo(CS$<>8__locals1.model.ImageFilePath).Length == 0L)
							{
								ModernMessageBox.Show(Resources.UnsupportedImageMsg, "PDFgear", MessageBoxButton.OK, MessageBoxResult.None, null, false);
								return;
							}
							CS$<>8__locals1.pageIndex = viewerOperationModel.CurrentPage;
							float width = this.mainViewModel.Document.Pages[CS$<>8__locals1.pageIndex].Width;
							float height = this.mainViewModel.Document.Pages[CS$<>8__locals1.pageIndex].Height;
							PdfImageObject pdfImageObject = PdfImageObject.Create(this.mainViewModel.Document, pdfBitmap, CS$<>8__locals1.rect.left, CS$<>8__locals1.rect.bottom);
							float num = CS$<>8__locals1.rect.Width / pdfImageObject.BoundingBox.Width;
							float num2 = CS$<>8__locals1.rect.Height / pdfImageObject.BoundingBox.Height;
							FS_MATRIX matrix = pdfImageObject.Matrix;
							float num3 = pdfImageObject.BoundingBox.right - pdfImageObject.BoundingBox.Width / 2f;
							float num4 = pdfImageObject.BoundingBox.top - pdfImageObject.BoundingBox.Height / 2f;
							matrix.Translate(-num3, -num4, false);
							matrix.Scale(num, num2, false);
							matrix.Translate(CS$<>8__locals1.rect.right - CS$<>8__locals1.rect.Width / 2f, CS$<>8__locals1.rect.top - CS$<>8__locals1.rect.Height / 2f, false);
							pdfImageObject.Matrix = matrix;
							this.mainViewModel.Document.Pages[CS$<>8__locals1.pageIndex].PageObjects.Add(pdfImageObject);
							CS$<>8__locals1.imageIndex = this.mainViewModel.Document.Pages[CS$<>8__locals1.pageIndex].PageObjects.Count - 1;
							CS$<>8__locals1.annotCanvas = PdfObjectExtensions.GetAnnotationCanvas(viewer);
							if (CS$<>8__locals1.annotCanvas != null)
							{
								CS$<>8__locals1.annotCanvas.ImageControl.CreateImageborder(CS$<>8__locals1.annotCanvas, this.mainViewModel.Document, CS$<>8__locals1.pageIndex, CS$<>8__locals1.imageIndex, CS$<>8__locals1.annotCanvas.PdfViewer, true);
								CS$<>8__locals1.annotCanvas.ImageControl.editorImageControl();
								CS$<>8__locals1.annotCanvas.ImageControl.Visibility = Visibility.Visible;
								CS$<>8__locals1.annotCanvas.ImageControl.UpdateImageborder();
							}
							DataOperationModel viewerOperationModel2 = this.mainViewModel.ViewerOperationModel;
							if (viewerOperationModel2 != null)
							{
								viewerOperationModel2.Dispose();
							}
							this.mainViewModel.ViewerOperationModel = null;
							this.mainViewModel.Document.Pages[CS$<>8__locals1.pageIndex].GenerateContentAdvance(false);
							await this.mainViewModel.Document.Pages[CS$<>8__locals1.pageIndex].TryRedrawPageAsync(default(CancellationToken));
							await this.mainViewModel.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
							{
								AnnotationToolbarViewModel.<>c__DisplayClass177_0.<<ProcessImageModelAsync>b__0>d <<ProcessImageModelAsync>b__0>d;
								<<ProcessImageModelAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
								<<ProcessImageModelAsync>b__0>d.<>4__this = CS$<>8__locals1;
								<<ProcessImageModelAsync>b__0>d.doc = doc;
								<<ProcessImageModelAsync>b__0>d.<>1__state = -1;
								<<ProcessImageModelAsync>b__0>d.<>t__builder.Start<AnnotationToolbarViewModel.<>c__DisplayClass177_0.<<ProcessImageModelAsync>b__0>d>(ref <<ProcessImageModelAsync>b__0>d);
								return <<ProcessImageModelAsync>b__0>d.<>t__builder.Task;
							}, delegate(PdfDocument doc)
							{
								AnnotationToolbarViewModel.<>c__DisplayClass177_0.<<ProcessImageModelAsync>b__1>d <<ProcessImageModelAsync>b__1>d;
								<<ProcessImageModelAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
								<<ProcessImageModelAsync>b__1>d.<>4__this = CS$<>8__locals1;
								<<ProcessImageModelAsync>b__1>d.doc = doc;
								<<ProcessImageModelAsync>b__1>d.<>1__state = -1;
								<<ProcessImageModelAsync>b__1>d.<>t__builder.Start<AnnotationToolbarViewModel.<>c__DisplayClass177_0.<<ProcessImageModelAsync>b__1>d>(ref <<ProcessImageModelAsync>b__1>d);
								return <<ProcessImageModelAsync>b__1>d.<>t__builder.Task;
							}, "");
						}
						catch
						{
							DrawUtils.ShowUnsupportedImageMessage();
							return;
						}
						finally
						{
							global::System.Drawing.Image image2 = image;
							if (image2 != null)
							{
								image2.Dispose();
							}
							PdfBitmap pdfBitmap2 = pdfBitmap;
							if (pdfBitmap2 != null)
							{
								pdfBitmap2.Dispose();
							}
						}
						image = null;
						pdfBitmap = null;
					}
				}
			}
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x00016628 File Offset: 0x00014828
		private FS_RECTF InsertImageSizeMethod(DataOperationModel dataOperation, WriteableBitmap bitmapImage, PdfPage currentPage)
		{
			float num = currentPage.MediaBox.Width - dataOperation.PositionFromDocument.X - 10f;
			float num2 = dataOperation.PositionFromDocument.Y - 10f;
			if (currentPage.Rotation == PageRotate.Rotate90)
			{
				num = currentPage.MediaBox.Height - dataOperation.PositionFromDocument.Y - 10f;
				num2 = currentPage.MediaBox.Width - dataOperation.PositionFromDocument.X - 10f;
			}
			else if (currentPage.Rotation == PageRotate.Rotate180)
			{
				num = dataOperation.PositionFromDocument.X - 10f;
				num2 = currentPage.MediaBox.Height - dataOperation.PositionFromDocument.Y - 10f;
			}
			else if (currentPage.Rotation == PageRotate.Rotate270)
			{
				num = dataOperation.PositionFromDocument.Y - 10f;
				num2 = dataOperation.PositionFromDocument.X - 10f;
			}
			FS_POINTF positionFromDocument = dataOperation.PositionFromDocument;
			double num3 = bitmapImage.Width / bitmapImage.Height;
			double num4 = 1.0;
			if ((float)bitmapImage.PixelWidth > num && (float)bitmapImage.PixelHeight > num2)
			{
				if (num3 > 1.0)
				{
					num4 = (double)(num / (float)bitmapImage.PixelWidth);
					if ((double)num2 / num4 < (double)bitmapImage.PixelHeight)
					{
						num4 = (double)(num2 / (float)bitmapImage.PixelHeight);
					}
				}
				else
				{
					num4 = (double)(num2 / (float)bitmapImage.PixelHeight);
					if ((double)num / num4 < (double)bitmapImage.PixelWidth)
					{
						num4 = (double)(num / (float)bitmapImage.PixelWidth);
					}
				}
			}
			else if ((float)bitmapImage.PixelWidth > num)
			{
				num4 = (double)(num / (float)bitmapImage.PixelWidth);
			}
			else if ((float)bitmapImage.PixelHeight > num2)
			{
				num4 = (double)(num2 / (float)bitmapImage.PixelHeight);
			}
			double num5 = (double)bitmapImage.PixelWidth * num4;
			double num6 = (double)bitmapImage.PixelHeight * num4;
			FS_RECTF fs_RECTF = new FS_RECTF((double)positionFromDocument.X, (double)positionFromDocument.Y, (double)positionFromDocument.X + num5, (double)positionFromDocument.Y - num6);
			if (currentPage.Rotation == PageRotate.Rotate90)
			{
				fs_RECTF = new FS_RECTF((double)positionFromDocument.X, (double)positionFromDocument.Y + num5, (double)positionFromDocument.X + num6, (double)positionFromDocument.Y);
			}
			else if (currentPage.Rotation == PageRotate.Rotate180)
			{
				fs_RECTF = new FS_RECTF((double)positionFromDocument.X - num5, (double)positionFromDocument.Y + num6, (double)positionFromDocument.X, (double)positionFromDocument.Y);
			}
			else if (currentPage.Rotation == PageRotate.Rotate270)
			{
				fs_RECTF = new FS_RECTF((double)positionFromDocument.X - num6, (double)positionFromDocument.Y, (double)positionFromDocument.X, (double)positionFromDocument.Y - num5);
			}
			return fs_RECTF;
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x000168F0 File Offset: 0x00014AF0
		public async void SharebyEmailCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document != null)
			{
				if (this.mainViewModel.CanSave)
				{
					ModernMessageBox.Show(Resources.SaveDocBeforeSendMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else
				{
					await ShareUtils.SendMailAsync(this.mainViewModel.DocumentWrapper.DocumentPath);
				}
			}
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00016928 File Offset: 0x00014B28
		public async void ShareCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document != null)
			{
				if (this.mainViewModel.CanSave)
				{
					ModernMessageBox.Show(Resources.SaveDocBeforeSendMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else
				{
					await ShareUtils.WindowsShareAsync(this.mainViewModel.DocumentWrapper.DocumentPath);
				}
			}
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x00016960 File Offset: 0x00014B60
		private void LinkCmd(ContextMenuItemModel model, bool Isbutton = true)
		{
			try
			{
				if (this.mainViewModel.Document != null)
				{
					if (AnnotationToolbarViewModel.<>o__181.<>p__2 == null)
					{
						AnnotationToolbarViewModel.<>o__181.<>p__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(AnnotationToolbarViewModel), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					Func<CallSite, object, bool> target = AnnotationToolbarViewModel.<>o__181.<>p__2.Target;
					CallSite <>p__ = AnnotationToolbarViewModel.<>o__181.<>p__2;
					if (AnnotationToolbarViewModel.<>o__181.<>p__1 == null)
					{
						AnnotationToolbarViewModel.<>o__181.<>p__1 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof(AnnotationToolbarViewModel), new CSharpArgumentInfo[]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
						}));
					}
					Func<CallSite, object, MouseModes, object> target2 = AnnotationToolbarViewModel.<>o__181.<>p__1.Target;
					CallSite <>p__2 = AnnotationToolbarViewModel.<>o__181.<>p__1;
					if (AnnotationToolbarViewModel.<>o__181.<>p__0 == null)
					{
						AnnotationToolbarViewModel.<>o__181.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(AnnotationToolbarViewModel), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					if (target(<>p__, target2(<>p__2, AnnotationToolbarViewModel.<>o__181.<>p__0.Target(AnnotationToolbarViewModel.<>o__181.<>p__0, this.mainViewModel.ViewerMouseMode), MouseModes.Default)))
					{
						if (AnnotationToolbarViewModel.<>o__181.<>p__3 == null)
						{
							AnnotationToolbarViewModel.<>o__181.<>p__3 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Value", typeof(AnnotationToolbarViewModel), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
							}));
						}
						AnnotationToolbarViewModel.<>o__181.<>p__3.Target(AnnotationToolbarViewModel.<>o__181.<>p__3, this.mainViewModel.ViewerMouseMode, MouseModes.Default);
					}
					ToolbarAnnotationButtonModel toolbarAnnotationButtonModel = (model.Parent as TypedContextMenuModel).Owner.Parent as ToolbarAnnotationButtonModel;
					if (Isbutton)
					{
						this.LinkButtonModel.IsChecked = !this.LinkButtonModel.IsChecked;
					}
					if (!this.LinkButtonModel.IsChecked)
					{
						this.mainViewModel.AnnotationMode = AnnotationMode.None;
						this.mainViewModel.SelectedAnnotation = null;
						this.NotifyCheckedButtonToolbarSettingChanged();
					}
					PdfAnnotation selectedAnnotation = this.mainViewModel.SelectedAnnotation;
					this.mainViewModel.ExitTransientMode(false, false, false, true, false);
					global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
					if (toolbarAnnotationButtonModel != null)
					{
						if (toolbarAnnotationButtonModel.IsChecked)
						{
							pdfControl.Viewer.IsLinkAnnotationHighlighted = true;
						}
						else
						{
							pdfControl.Viewer.IsLinkAnnotationHighlighted = false;
							if (selectedAnnotation != null)
							{
								this.mainViewModel.SelectedAnnotation = null;
							}
						}
					}
					AnnotationMode annotationMode = this.mainViewModel.AnnotationMode;
					if (pdfControl != null)
					{
						AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
						if (((annotationHolderManager != null) ? annotationHolderManager.CurrentHolder : null) is LinkAnnotationHolder && annotationHolderManager != null && annotationHolderManager.CurrentHolder.State == AnnotationHolderState.CreatingNew)
						{
							AnnotationMode annotationMode2 = this.mainViewModel.AnnotationMode;
							annotationHolderManager.CancelAll();
							this.mainViewModel.AnnotationMode = annotationMode2;
						}
					}
					this.mainViewModel.RaiseAnnotationModePropertyChanged();
					if (selectedAnnotation != null)
					{
						if (annotationMode == AnnotationMode.None)
						{
							this.mainViewModel.ReleaseViewerFocusAsync(false);
						}
						else if (annotationMode == AnnotationMode.Ink)
						{
							this.mainViewModel.ReleaseViewerFocusAsync(false);
						}
						else if (annotationMode != AnnotationMode.None)
						{
							global::System.Collections.Generic.IReadOnlyList<AnnotationMode> annotationModes = AnnotationFactory.GetAnnotationModes(selectedAnnotation);
							if (annotationModes.Count == 0 || annotationModes[0] != annotationMode)
							{
								this.mainViewModel.ReleaseViewerFocusAsync(true);
								this.NotifyCheckedButtonToolbarSettingChanged();
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x00016C88 File Offset: 0x00014E88
		public async void AddLinkDirectly()
		{
			FS_RECTF selectedDestination = this.GetSelectedDestination();
			if (selectedDestination.Width > 5f || selectedDestination.Height > 5f)
			{
				LinkEditWindows linkEditWindows = new LinkEditWindows(this.mainViewModel.Document);
				linkEditWindows.Owner = App.Current.MainWindow;
				linkEditWindows.WindowStartupLocation = ((linkEditWindows.Owner != null) ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen);
				bool? flag = linkEditWindows.ShowDialog();
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				PdfPage page = this.mainViewModel.Document.Pages.CurrentPage;
				if (page.Annots == null)
				{
					page.CreateAnnotations();
				}
				if (flag.GetValueOrDefault())
				{
					PdfLinkAnnotation LinkAnnot = new PdfLinkAnnotation(page);
					if (linkEditWindows.SelectedType == LinkSelect.ToPage)
					{
						int num = linkEditWindows.Page - 1;
						PdfDestination pdfDestination = PdfDestination.CreateXYZ(this.mainViewModel.Document, num, null, new float?(this.mainViewModel.Document.Pages[num].Height), null);
						LinkAnnot.Link.Action = new PdfGoToAction(this.mainViewModel.Document, pdfDestination);
					}
					else if (linkEditWindows.SelectedType == LinkSelect.ToWeb)
					{
						LinkAnnot.Link.Action = new PdfUriAction(this.mainViewModel.Document, linkEditWindows.UrlFilePath);
					}
					else if (linkEditWindows.SelectedType == LinkSelect.ToFile)
					{
						PdfFileSpecification pdfFileSpecification = new PdfFileSpecification(this.mainViewModel.Document);
						pdfFileSpecification.FileName = linkEditWindows.FileDiaoligFiePath;
						LinkAnnot.Link.Action = new PdfLaunchAction(this.mainViewModel.Document, pdfFileSpecification);
					}
					global::System.Windows.Media.Color color = (global::System.Windows.Media.Color)global::System.Windows.Media.ColorConverter.ConvertFromString(linkEditWindows.SelectedFontground);
					FS_COLOR fs_COLOR = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
					float num2;
					if (!linkEditWindows.rectangleVis)
					{
						num2 = 0f;
					}
					else
					{
						LinkAnnot.Color = fs_COLOR;
						num2 = linkEditWindows.BorderWidth;
					}
					LinkAnnot.Rectangle = selectedDestination;
					LinkAnnot.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
					LinkAnnot.Flags |= AnnotationFlags.Print;
					PdfBorderStyle pdfBorderStyle = new PdfBorderStyle
					{
						Width = num2,
						Style = linkEditWindows.BorderStyles,
						DashPattern = new float[] { 2f, 4f }
					};
					LinkAnnot.SetBorderStyle(pdfBorderStyle);
					page.Annots.Add(LinkAnnot);
					await requiredService.OperationManager.TraceAnnotationInsertAsync(LinkAnnot, "");
					await page.TryRedrawPageAsync(default(CancellationToken));
					if (LinkAnnot != null)
					{
						GAManager.SendEvent("AnnotationAction", "PdfLinkAnnotation", "New", 1L);
					}
					LinkAnnot = null;
				}
				page = null;
			}
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x00016CC0 File Offset: 0x00014EC0
		private FS_RECTF GetSelectedDestination()
		{
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			PdfViewer pdfViewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			TextInfo[] array = PdfTextMarkupAnnotationUtils.GetTextInfos(this.mainViewModel.Document, pdfViewer.SelectInfo, false).ToArray<TextInfo>();
			List<FS_RECTF> list = new List<FS_RECTF>();
			foreach (TextInfo textInfo in array)
			{
				list.AddRange(PdfTextMarkupAnnotationUtils.GetNormalizedRects(pdfViewer, textInfo, true, true));
			}
			if (list.Count == 1)
			{
				return list.FirstOrDefault<FS_RECTF>();
			}
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			foreach (FS_RECTF fs_RECTF in list)
			{
				num = Math.Min(num, Math.Min(fs_RECTF.left, fs_RECTF.right));
				num2 = Math.Min(num2, Math.Min(fs_RECTF.top, fs_RECTF.bottom));
				num3 = Math.Max(num3, Math.Max(fs_RECTF.left, fs_RECTF.right));
				num4 = Math.Max(num4, Math.Max(fs_RECTF.top, fs_RECTF.bottom));
			}
			return new FS_RECTF(num, num2, num3, num4);
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x00016E18 File Offset: 0x00015018
		public void SharebyFileCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document == null)
			{
				return;
			}
			if (this.mainViewModel.CanSave)
			{
				ModernMessageBox.Show(Resources.SaveDocBeforeSendMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			MainView mainView = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
			new ShareSendFileDialog(this.mainViewModel.DocumentWrapper.DocumentPath)
			{
				Owner = mainView,
				WindowStartupLocation = ((mainView == null) ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner)
			}.ShowDialog();
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x00016E9C File Offset: 0x0001509C
		public void DoManageStampCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document == null)
			{
				return;
			}
			GAManager.SendEvent("MainWindow_Stamp", "ManageCustomStampBtn", "Count", 1L);
			this.mainViewModel.AnnotationMode = AnnotationMode.None;
			new StampManageWindow
			{
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				Owner = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>()
			}.ShowDialog();
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x00016F08 File Offset: 0x00015108
		public async void DoStampCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document != null)
			{
				GAManager.SendEvent("MainWindow_Stamp", "NewCustomStampBtn", "Count", 1L);
				this.mainViewModel.AnnotationMode = AnnotationMode.Stamp;
				EditStampWindow editStampWindow = new EditStampWindow();
				editStampWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				editStampWindow.Owner = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
				if (!editStampWindow.ShowDialog().GetValueOrDefault())
				{
					DataOperationModel viewerOperationModel = this.mainViewModel.ViewerOperationModel;
					if (viewerOperationModel != null)
					{
						viewerOperationModel.Dispose();
					}
					this.mainViewModel.AnnotationMode = AnnotationMode.None;
				}
				else
				{
					if (this.mainViewModel.Document != null)
					{
						if (editStampWindow.isSave)
						{
							this.SaveStamp(editStampWindow.StampTextModel);
							if (editStampWindow.isText)
							{
								await this.ProcessStampTextModelAsync(editStampWindow.StampTextModel);
							}
							else
							{
								await this.ProcessStampImageModelAsync(new StampImageModel
								{
									ImageFilePath = editStampWindow.ResultModel.ImageFilePath,
									RemoveBackground = editStampWindow.ResultModel.RemoveBackground
								});
							}
						}
						else if (editStampWindow.isText)
						{
							await this.ProcessStampTextModelAsync(editStampWindow.StampTextModel);
						}
						else
						{
							await this.ProcessStampImageModelAsync(new StampImageModel
							{
								ImageFilePath = editStampWindow.FileDiaoligFiePath,
								RemoveBackground = editStampWindow.ResultModel.RemoveBackground
							});
						}
					}
					this.mainViewModel.AnnotationMode = AnnotationMode.None;
				}
			}
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x00016F40 File Offset: 0x00015140
		private async void DoStampPresetsCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document != null)
			{
				GAManager.SendEvent("PdfStampAnnotation", "DoStamp", "Presets", 1L);
				this.mainViewModel.AnnotationMode = AnnotationMode.Stamp;
				IStampTextModel stampTextModel = model.TagData.MenuItemValue as IStampTextModel;
				if (stampTextModel != null)
				{
					await this.ProcessStampTextModelAsync(stampTextModel);
				}
				this.mainViewModel.AnnotationMode = AnnotationMode.None;
			}
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x00016F80 File Offset: 0x00015180
		internal async Task ProcessStampTextModelAsync(IStampTextModel model)
		{
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			PdfViewer viewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			AnnotationHolderManager holder = PdfObjectExtensions.GetAnnotationHolderManager(viewer);
			if (holder != null)
			{
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				holder.CancelAll();
				await holder.WaitForCancelCompletedAsync();
				if (model != null)
				{
					string textContent = model.TextContent;
					string text = "Helvetica-BoldOblique";
					global::System.Windows.FontStyle fontStyle = FontStyles.Italic;
					global::System.Windows.FontWeight fontWeight = FontWeights.Bold;
					if (!model.IsPreset && !PdfFontUtils.CheckStockFontSupport(FontStockNames.HelveticaBoldOblique.ToString(), textContent))
					{
						text = "#GLOBAL USER INTERFACE";
						fontStyle = FontStyles.Normal;
						fontWeight = FontWeights.Normal;
					}
					StampAnnotationMoveControl stampAnnotationMoveControl = new StampAnnotationMoveControl(new TextStampModel
					{
						TextFontSize = 12.0,
						TextFontFamily = text,
						TextFontStyle = fontStyle,
						TextFontWeight = fontWeight,
						Text = textContent,
						BorderBrush = model.FontColor,
						Foreground = model.FontColor,
						TextWidth = 120.0,
						TextHeight = 30.0,
						PageScale = 1.0
					});
					stampAnnotationMoveControl.Width = 120.0;
					stampAnnotationMoveControl.Height = 30.0;
					this.mainViewModel.ViewerOperationModel = new HoverOperationModel(viewer)
					{
						Data = model,
						SizeInDocument = new FS_SIZEF(120f, 30f),
						PreviewElement = new Viewbox
						{
							Child = stampAnnotationMoveControl
						}
					};
					ViewOperationResult<bool> viewOperationResult = await this.mainViewModel.ViewerOperationModel.Task;
					DataOperationModel viewerOperationModel = this.mainViewModel.ViewerOperationModel;
					if (viewOperationResult != null && viewOperationResult.Value)
					{
						holder.CancelAll();
						holder.Stamp.StartCreateNew(this.mainViewModel.Document.Pages[viewerOperationModel.CurrentPage], viewerOperationModel.PositionFromDocument);
						global::System.Collections.Generic.IReadOnlyList<PdfStampAnnotation> readOnlyList = await holder.Stamp.CompleteCreateNewAsync();
						if (readOnlyList != null && readOnlyList.Count > 0)
						{
							holder.Select(readOnlyList[0], true);
						}
					}
				}
			}
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x00016FCC File Offset: 0x000151CC
		internal async Task ProcessStampTextModelAsync(DynamicStampTextModel model)
		{
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			PdfViewer viewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			AnnotationHolderManager holder = PdfObjectExtensions.GetAnnotationHolderManager(viewer);
			if (holder != null)
			{
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				holder.CancelAll();
				await holder.WaitForCancelCompletedAsync();
				if (model != null)
				{
					int num = 60;
					if (model.TemplateName == "Chop1")
					{
						num = 200;
					}
					Rect stampSize = this.GetStampSize(viewer.CurrentIndex, (double)num);
					StampAnnotationMoveControl stampAnnotationMoveControl = new StampAnnotationMoveControl(model, stampSize);
					stampAnnotationMoveControl.Width = stampSize.Width;
					stampAnnotationMoveControl.Height = stampSize.Height;
					this.mainViewModel.ViewerOperationModel = new HoverOperationModel(viewer)
					{
						Data = model,
						SizeInDocument = new FS_SIZEF(stampSize.Width, stampSize.Height),
						PreviewElement = new Viewbox
						{
							Child = stampAnnotationMoveControl
						}
					};
					ViewOperationResult<bool> viewOperationResult = await this.mainViewModel.ViewerOperationModel.Task;
					DataOperationModel viewerOperationModel = this.mainViewModel.ViewerOperationModel;
					if (viewOperationResult != null && viewOperationResult.Value)
					{
						holder.CancelAll();
						holder.Stamp.StartCreateNew(this.mainViewModel.Document.Pages[viewerOperationModel.CurrentPage], viewerOperationModel.PositionFromDocument);
						global::System.Collections.Generic.IReadOnlyList<PdfStampAnnotation> readOnlyList = await holder.Stamp.CompleteCreateNewAsync();
						if (readOnlyList != null && readOnlyList.Count > 0)
						{
							holder.Select(readOnlyList[0], true);
						}
					}
				}
			}
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x00017018 File Offset: 0x00015218
		private Rect GetStampSize(int currentPageindex, double defaultHeight)
		{
			Rect rect = new Rect
			{
				Height = defaultHeight,
				Width = 200.0
			};
			PdfPage pdfPage = this.mainViewModel.Document.Pages[currentPageindex];
			if (pdfPage.Width <= 1000f)
			{
				if (pdfPage.Width <= 250f)
				{
					rect.Width = 50.0;
					rect.Height = defaultHeight / 4.0;
				}
				else
				{
					rect.Width = (double)(pdfPage.Width / 5f);
					rect.Height = defaultHeight / 200.0 * (double)pdfPage.Width / 5.0;
				}
			}
			return rect;
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x000170D8 File Offset: 0x000152D8
		internal async Task ProcessStampImageModelAsync(StampImageModel model)
		{
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			PdfViewer viewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			AnnotationHolderManager holder = PdfObjectExtensions.GetAnnotationHolderManager(viewer);
			if (holder != null)
			{
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				holder.CancelAll();
				await holder.WaitForCancelCompletedAsync();
				ImageStampModel imageStampModel = new ImageStampModel();
				WriteableBitmap writeableBitmap = null;
				try
				{
					using (FileStream fileStream = File.OpenRead(model.ImageFilePath))
					{
						BitmapImage bitmapImage = new BitmapImage();
						bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage.BeginInit();
						bitmapImage.StreamSource = fileStream;
						bitmapImage.EndInit();
						writeableBitmap = new WriteableBitmap(bitmapImage);
					}
				}
				catch
				{
					DrawUtils.ShowUnsupportedImageMessage();
					return;
				}
				imageStampModel.StampImageSource = writeableBitmap;
				global::System.Windows.Size size = default(global::System.Windows.Size);
				PdfPage currentPage = this.mainViewModel.Document.Pages.CurrentPage;
				FS_RECTF effectiveBox = currentPage.GetEffectiveBox(currentPage.Rotation, false);
				if (this.mainViewModel.AnnotationMode == AnnotationMode.Signature)
				{
					size = StampAnnotationHolder.GetSignaturePageSize(writeableBitmap.Width, writeableBitmap.Height, effectiveBox);
				}
				else
				{
					size = StampAnnotationHolder.GetStampPageSize(writeableBitmap.Width, writeableBitmap.Height, effectiveBox);
				}
				imageStampModel.ImageWidth = size.Width;
				imageStampModel.ImageHeight = size.Height;
				imageStampModel.PageSize = new FS_SIZEF(size.Width, size.Height);
				StampAnnotationMoveControl stampAnnotationMoveControl = new StampAnnotationMoveControl(imageStampModel)
				{
					Width = size.Width,
					Height = size.Height
				};
				model.ImageStampControlModel = imageStampModel;
				this.mainViewModel.ViewerOperationModel = new HoverOperationModel(viewer)
				{
					Data = model,
					SizeInDocument = new FS_SIZEF(size.Width, size.Height),
					PreviewElement = new Viewbox
					{
						Child = stampAnnotationMoveControl
					}
				};
				ViewOperationResult<bool> viewOperationResult = await this.mainViewModel.ViewerOperationModel.Task;
				DataOperationModel viewerOperationModel = this.mainViewModel.ViewerOperationModel;
				if (viewOperationResult != null && viewOperationResult.Value)
				{
					holder.CancelAll();
					holder.Stamp.StartCreateNew(this.mainViewModel.Document.Pages[viewerOperationModel.CurrentPage], viewerOperationModel.PositionFromDocument);
					global::System.Collections.Generic.IReadOnlyList<PdfStampAnnotation> readOnlyList = await holder.Stamp.CompleteCreateNewAsync();
					if (readOnlyList != null && readOnlyList.Count > 0)
					{
						holder.Select(readOnlyList[0], true);
					}
				}
			}
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x00017124 File Offset: 0x00015324
		public async Task<FS_RECTF?> CreateDigitalSignatureRectAsync()
		{
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			PdfViewer viewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(viewer);
			FS_RECTF? fs_RECTF;
			if (annotationHolderManager == null)
			{
				fs_RECTF = null;
			}
			else
			{
				annotationHolderManager.CancelAll();
				await annotationHolderManager.WaitForCancelCompletedAsync();
				DragOperationModel operationModel = new DragOperationModel(viewer);
				this.mainViewModel.ViewerOperationModel = operationModel;
				ViewOperationResult<bool> viewOperationResult = await operationModel.Task;
				if (viewOperationResult != null && viewOperationResult.Value)
				{
					FS_SIZEF sizeInDocument = operationModel.SizeInDocument;
					FS_RECTF fs_RECTF2 = new FS_RECTF(operationModel.PositionFromDocument.X, operationModel.PositionFromDocument.Y, operationModel.PositionFromDocument.X + sizeInDocument.Width, operationModel.PositionFromDocument.Y - sizeInDocument.Height);
					fs_RECTF = new FS_RECTF?(fs_RECTF2);
				}
				else
				{
					fs_RECTF = null;
				}
			}
			return fs_RECTF;
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x00017168 File Offset: 0x00015368
		public async Task DoDigitalSignatureAsync()
		{
			if (this.mainViewModel.CanSave)
			{
				MessageBox.Show(Resources.SaveDocBeforeSendMsg, UtilManager.GetProductName());
			}
			else
			{
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				this.mainViewModel.AnnotationMode = AnnotationMode.None;
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
				PdfViewer pdfViewer = ((pdfControl != null) ? pdfControl.Viewer : null);
				if (PdfObjectExtensions.GetAnnotationHolderManager(pdfViewer) != null)
				{
					MainView mainView = Window.GetWindow(pdfViewer) as MainView;
					if (mainView != null)
					{
						mainView.ShowPdfViewerView();
					}
					if (!ConfigManager.GetDoNotShowFlag("NotShowDigitalSignatureCreateTipFlag", false))
					{
						bool? checkboxResult = MessageBoxHelper.Show(new MessageBoxHelper.RichMessageBoxContent
						{
							Content = Resources.DigitalSignatureCreateTip,
							ShowLeftBottomCheckbox = true,
							LeftBottomCheckboxContent = Resources.WinPwdPasswordSaveTipNotshowagainContent
						}, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false).CheckboxResult;
						if (checkboxResult != null && checkboxResult.GetValueOrDefault())
						{
							ConfigManager.SetDoNotShowFlag("NotShowDigitalSignatureCreateTipFlag", true);
						}
					}
					FS_RECTF? fs_RECTF = await this.CreateDigitalSignatureRectAsync();
					if (fs_RECTF != null)
					{
						await this.CreateDigitalSignature(null, fs_RECTF.Value);
					}
				}
			}
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x000171AC File Offset: 0x000153AC
		public async Task CreateDeferredDigitalSignatureFieldAsync()
		{
			GAManager.SendEvent("DigitalSignatureFiled", "AddDSFiledInPDF", "Begin", 1L);
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			this.mainViewModel.AnnotationMode = AnnotationMode.None;
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			PdfViewer viewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			MainView mainView = Window.GetWindow(viewer) as MainView;
			if (mainView != null)
			{
				mainView.ShowPdfViewerView();
			}
			if (!ConfigManager.GetDoNotShowFlag("NotShowDigitalSignatureCreateTipFlag", false))
			{
				bool? checkboxResult = MessageBoxHelper.Show(new MessageBoxHelper.RichMessageBoxContent
				{
					Content = Resources.ResourceManager.GetString("DeferredDigitSignCreateTip"),
					ShowLeftBottomCheckbox = true,
					LeftBottomCheckboxContent = Resources.WinPwdPasswordSaveTipNotshowagainContent
				}, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false).CheckboxResult;
				if (checkboxResult != null && checkboxResult.GetValueOrDefault())
				{
					ConfigManager.SetDoNotShowFlag("NotShowDigitalSignatureCreateTipFlag", true);
				}
			}
			FS_RECTF? fs_RECTF = await this.CreateDigitalSignatureRectAsync();
			if (fs_RECTF != null)
			{
				if (CreateDigitalSignatureDialog.CanCreate(new global::System.Windows.Size((double)fs_RECTF.Value.Width, (double)fs_RECTF.Value.Height), AnnotationToolbarViewModel.DefaultDigitalSignatureThickness, AnnotationToolbarViewModel.DefaultDigitalSignatureThickness))
				{
					PdfSignatureField pdfSignatureField = new PdfSignatureField(this.mainViewModel.Document.FormFill.InterForm, string.Format("{0:D}", Guid.NewGuid()), null);
					Patagames.Pdf.Net.PdfControl pdfControl2 = new PdfSignatureControl(pdfSignatureField, this.mainViewModel.Document.Pages.CurrentPage, fs_RECTF.Value, new FS_COLOR?(new FS_COLOR(0)), PageRotate.Normal, 1f, null);
					if (!this.mainViewModel.Document.FormFill.InterForm.HasDefaultAppearance())
					{
						this.mainViewModel.Document.FormFill.InterForm.SetDefaultAppearance(null, 0f, null);
					}
					this.mainViewModel.Document.FormFill.InterForm.Fields.Add(pdfSignatureField);
					PdfWidgetAnnotation widget = pdfControl2.GetWidget(null);
					widget.CreateEmptyAppearance(AppearanceStreamModes.Normal);
					FS_RECTF rectangle = widget.Rectangle;
					widget.GenerateAppearance(AppearanceStreamModes.Normal);
					widget.Rectangle = rectangle;
					this.mainViewModel.SetCanSaveFlag("AddDeferredDigitalSignature", true);
					this.mainViewModel.DocumentWrapper.ReloadDigitalSignatureHelper();
					GAManager.SendEvent("DigitalSignatureFiled", "AddDSFiledInPDF", "Done", 1L);
					await viewer.TryRedrawVisiblePageAsync(default(CancellationToken));
				}
				else
				{
					ModernMessageBox.Show(Resources.DigSigCreateMessage_AreaTooSmall, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
			}
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x000171F0 File Offset: 0x000153F0
		public async Task SignDeferredDigitalSignature(PdfDigitalSignatureLocation pdfDigitalSignatureLocation)
		{
			await this.CreateDigitalSignature(pdfDigitalSignatureLocation, default(FS_RECTF));
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x0001723C File Offset: 0x0001543C
		public async Task RemoveDeferredDigitalSignature(PdfDigitalSignatureLocation pdfDigitalSignatureLocation)
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			this.mainViewModel.AnnotationMode = AnnotationMode.None;
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			PdfViewer pdfViewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			PdfObjectExtensions.GetAnnotationHolderManager(pdfViewer);
			if (ModernMessageBox.Show(Resources.ResourceManager.GetString("RemoveDeferredDigitSignConfirmMessage").Replace("XXX", " " + pdfDigitalSignatureLocation.Name + " "), UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.No, null, false) == MessageBoxResult.Yes)
			{
				if (this.mainViewModel.Document.FormFill.InterForm.RemoveField(pdfDigitalSignatureLocation.SignatureField))
				{
					this.mainViewModel.Document.FormFill.InterForm.ReloadForms();
					this.mainViewModel.SetCanSaveFlag("RemoveDeferredDigitalSignature", true);
					this.mainViewModel.DocumentWrapper.ReloadDigitalSignatureHelper();
					await pdfViewer.TryRedrawVisiblePageAsync(default(CancellationToken));
				}
				else
				{
					ModernMessageBox.Show(Resources.ResourceManager.GetString("RemoveDeferredDigitSignFailedMessage"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
			}
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x00017288 File Offset: 0x00015488
		private async Task CreateDigitalSignature(PdfDigitalSignatureLocation pdfDigitalSignatureLocation, FS_RECTF rect)
		{
			bool flag;
			if (pdfDigitalSignatureLocation != null)
			{
				flag = CreateDigitalSignatureDialog.CanCreate(pdfDigitalSignatureLocation, AnnotationToolbarViewModel.DefaultDigitalSignatureThickness, AnnotationToolbarViewModel.DefaultDigitalSignatureThickness);
			}
			else
			{
				flag = CreateDigitalSignatureDialog.CanCreate(new global::System.Windows.Size((double)rect.Width, (double)rect.Height), AnnotationToolbarViewModel.DefaultDigitalSignatureThickness, AnnotationToolbarViewModel.DefaultDigitalSignatureThickness);
			}
			if (flag)
			{
				CertificateManagerWindow.X509CertificateModel[] certs = CertificateManagerWindow.GetSignatureCertificate().ToArray<CertificateManagerWindow.X509CertificateModel>();
				try
				{
					CreateDigitalSignatureDialog createDigitalSignatureDialog;
					if (pdfDigitalSignatureLocation != null)
					{
						createDigitalSignatureDialog = new CreateDigitalSignatureDialog(pdfDigitalSignatureLocation, certs);
					}
					else
					{
						createDigitalSignatureDialog = new CreateDigitalSignatureDialog(new global::System.Windows.Size((double)rect.Width, (double)rect.Height), certs);
					}
					createDigitalSignatureDialog.DrawingLeftElementMargin = AnnotationToolbarViewModel.DefaultDigitalSignatureThickness;
					createDigitalSignatureDialog.DrawingRightElementMargin = AnnotationToolbarViewModel.DefaultDigitalSignatureThickness;
					createDigitalSignatureDialog.Owner = App.Current.MainWindow;
					createDigitalSignatureDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
					if (pdfDigitalSignatureLocation != null)
					{
						GAManager.SendEvent("CreateDigitalSignatureDialog", "Show", "SignFromSignatureField", 1L);
					}
					else
					{
						GAManager.SendEvent("CreateDigitalSignatureDialog", "Show", "SignWithDigitalSignature", 1L);
					}
					if (createDigitalSignatureDialog.ShowDialog().GetValueOrDefault())
					{
						DigitalSignatureDrawingHelper digitalSignatureDrawingHelper = createDigitalSignatureDialog.CreateDrawingHelper();
						if (digitalSignatureDrawingHelper != null)
						{
							DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
							string text;
							if (documentWrapper == null)
							{
								text = null;
							}
							else
							{
								EncryptManage encryptManage = documentWrapper.EncryptManage;
								text = ((encryptManage != null) ? encryptManage.UserPassword : null);
							}
							string password = text;
							if (string.IsNullOrEmpty(password))
							{
								password = null;
							}
							FS_RECTF? fs_RECTF = null;
							if (pdfDigitalSignatureLocation == null)
							{
								fs_RECTF = new FS_RECTF?(rect);
							}
							MainViewModel.SaveResult saveResult = await this.mainViewModel.DocumentWrapper.DigitalSignatureHelper.AddDigitalSignatureAndSaveAsAsync(createDigitalSignatureDialog.X509Certificate, digitalSignatureDrawingHelper, fs_RECTF, createDigitalSignatureDialog.Certificated, this.mainViewModel.CanSave, (pdfDigitalSignatureLocation != null) ? pdfDigitalSignatureLocation.Name : null);
							if (saveResult.FailedResult == MainViewModel.SaveFailedResult.Successed)
							{
								await this.mainViewModel.OpenDocumentCoreAsync(saveResult.File.FullName, password, true);
							}
							else
							{
								ModernMessageBox.Show(Resources.DigSigCreateMessage_Failed, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							}
							password = null;
						}
					}
				}
				finally
				{
					foreach (CertificateManagerWindow.X509CertificateModel x509CertificateModel in certs)
					{
						try
						{
							x509CertificateModel.Certificate.Dispose();
						}
						catch
						{
						}
					}
				}
				certs = null;
			}
			else
			{
				ModernMessageBox.Show(Resources.DigSigCreateMessage_AreaTooSmall, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x000172DC File Offset: 0x000154DC
		private async Task DoRedactionCommand()
		{
			if (this.RedactionButtonModel.IsChecked)
			{
				if (!ConfigManager.GetDoNotShowFlag("NotShowRedactTipFlag", false))
				{
					bool? checkboxResult = MessageBoxHelper.Show(new MessageBoxHelper.RichMessageBoxContent
					{
						Title = Resources.RedactStartMessageTitle,
						Content = Resources.RedactStartMessageContent,
						ShowLeftBottomCheckbox = true,
						LeftBottomCheckboxContent = Resources.WinPwdPasswordSaveTipNotshowagainContent
					}, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false).CheckboxResult;
					if (checkboxResult != null && checkboxResult.GetValueOrDefault())
					{
						ConfigManager.SetDoNotShowFlag("NotShowRedactTipFlag", true);
					}
				}
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
				PdfViewer viewer = ((pdfControl != null) ? pdfControl.Viewer : null);
				AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(viewer);
				if (annotationHolderManager != null)
				{
					MainView mainView = Window.GetWindow(viewer) as MainView;
					if (mainView != null)
					{
						mainView.ShowPdfViewerView();
					}
					this.mainViewModel.ExitTransientMode(false, false, false, true, false);
					annotationHolderManager.CancelAll();
					await annotationHolderManager.WaitForCancelCompletedAsync();
					this.mainViewModel.AnnotationMode = AnnotationMode.None;
					this.RedactionButtonModel.IsChecked = true;
					while (this.RedactionButtonModel.IsChecked)
					{
						DragOperationModel operationModel = new DragOperationModel(viewer);
						this.mainViewModel.ViewerOperationModel = operationModel;
						ViewOperationResult<bool> viewOperationResult = await operationModel.Task;
						if (this.mainViewModel.ViewerOperationModel != operationModel)
						{
							break;
						}
						if (viewOperationResult != null && viewOperationResult.Value)
						{
							FS_SIZEF sizeInDocument = operationModel.SizeInDocument;
							if (sizeInDocument.Width > 2f && sizeInDocument.Height > 2f)
							{
								GAManager.SendEvent("Redaction", "DoRedaction", "Begin", 1L);
								new global::System.Windows.Size((double)sizeInDocument.Width, (double)sizeInDocument.Height);
								FS_RECTF fs_RECTF = new FS_RECTF(operationModel.PositionFromDocument.X, operationModel.PositionFromDocument.Y, operationModel.PositionFromDocument.X + sizeInDocument.Width, operationModel.PositionFromDocument.Y - sizeInDocument.Height);
								PdfPage pdfPage = this.mainViewModel.Document.Pages[operationModel.CurrentPage];
								object selectedValue = this.RedactionButtonModel.ToolbarSettingModel.FirstOrDefault((ToolbarSettingItemModel c) => c.Type == ContextMenuItemType.FillColor).SelectedValue;
								FS_COLOR? fs_COLOR = new FS_COLOR?(FS_COLOR.Black);
								try
								{
									global::System.Windows.Media.Color color = (global::System.Windows.Media.Color)global::System.Windows.Media.ColorConverter.ConvertFromString(selectedValue as string);
									fs_COLOR = ((color.A != 0) ? new FS_COLOR?(color.ToPdfColor()) : null);
								}
								catch
								{
								}
								await this.mainViewModel.OperationManager.RedactionTextObjectAsync(pdfPage, fs_RECTF, fs_COLOR, "Redact");
								GAManager.SendEvent("Redaction", "DoRedaction", "End", 1L);
							}
						}
						operationModel = null;
					}
				}
			}
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x00017320 File Offset: 0x00015520
		private async void DoWatermarkInsertCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document != null)
			{
				GAManager.SendEvent("MainWindow", "Watermark", "DoWatermarkInsertCmd", 1L);
				await this.mainViewModel.ReleaseViewerFocusAsync(true);
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				bool? flag = new WatermarkEditWin().ShowDialog();
				bool flag2 = false;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					this.mainViewModel.AnnotationMode = AnnotationMode.None;
					this.WatermarkParam = null;
					this.TextWatermarkModel = null;
					this.ImageWatermarkModel = null;
				}
				else
				{
					this.mainViewModel.SetCanSaveFlag();
					this.mainViewModel.AnnotationMode = AnnotationMode.Watermark;
					if (this.mainViewModel.AnnotationToolbar.LinkButtonModel.IsChecked)
					{
						this.mainViewModel.AnnotationMode = AnnotationMode.None;
					}
				}
			}
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x00017358 File Offset: 0x00015558
		public async void DoWatermarkInsertCmd2()
		{
			if (this.mainViewModel.Document != null)
			{
				GAManager.SendEvent("MainWindow", "Watermark", "DoWatermarkInsertCmd", 1L);
				await this.mainViewModel.ReleaseViewerFocusAsync(true);
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				bool? flag = new WatermarkEditWin().ShowDialog();
				bool flag2 = false;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					this.mainViewModel.AnnotationMode = AnnotationMode.None;
					this.WatermarkParam = null;
					this.TextWatermarkModel = null;
					this.ImageWatermarkModel = null;
				}
				else
				{
					this.mainViewModel.SetCanSaveFlag();
					this.mainViewModel.AnnotationMode = AnnotationMode.Watermark;
					if (this.mainViewModel.AnnotationToolbar.LinkButtonModel.IsChecked)
					{
						this.mainViewModel.AnnotationMode = AnnotationMode.None;
					}
				}
			}
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x00017390 File Offset: 0x00015590
		private async void DoWatermarkDelCmd(ContextMenuItemModel model, bool allPage)
		{
			if (this.mainViewModel.Document != null && this.mainViewModel.Document.Pages != null)
			{
				GAManager.SendEvent("MainWindow", "Watermark", "DoWatermarkDelCmd", 1L);
				await this.mainViewModel.ReleaseViewerFocusAsync(true);
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				if (MessageBox.Show(Resources.DeleteWatermarkAskMsg, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.No)
				{
					int num = 0;
					int num2 = 0;
					if (allPage)
					{
						num = 0;
						num2 = this.mainViewModel.Document.Pages.Count;
					}
					else
					{
						num = this.mainViewModel.Document.Pages.CurrentIndex;
						num2 = 1;
					}
					bool flag = false;
					global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
					global::System.ValueTuple<int, int> valueTuple = ((pdfControl != null) ? pdfControl.GetVisiblePageRange() : new global::System.ValueTuple<int, int>(-1, -1));
					int item = valueTuple.Item1;
					int item2 = valueTuple.Item2;
					for (int i = num; i < num + num2; i++)
					{
						IntPtr intPtr = IntPtr.Zero;
						PdfPage pdfPage = null;
						try
						{
							intPtr = Pdfium.FPDF_LoadPage(this.mainViewModel.Document.Handle, i);
							if (intPtr != IntPtr.Zero)
							{
								pdfPage = PdfPage.FromHandle(this.mainViewModel.Document, intPtr, i, true);
								if (pdfPage.Annots != null && pdfPage.Annots.Count > 0)
								{
									for (int j = pdfPage.Annots.Count - 1; j >= 0; j--)
									{
										if (pdfPage.Annots[j] is PdfWatermarkAnnotation)
										{
											flag = true;
											pdfPage.Annots.RemoveAt(j);
										}
									}
								}
							}
						}
						finally
						{
							if (pdfPage != null && (pdfPage.PageIndex > item2 || pdfPage.PageIndex < item))
							{
								PageDisposeHelper.DisposePage(pdfPage);
							}
							if (intPtr != IntPtr.Zero)
							{
								Pdfium.FPDF_ClosePage(intPtr);
							}
						}
					}
					if (flag)
					{
						this.mainViewModel.SetCanSaveFlag();
					}
					if (pdfControl != null)
					{
						await pdfControl.TryRedrawVisiblePageAsync(default(CancellationToken));
					}
				}
			}
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x000173D0 File Offset: 0x000155D0
		private async void DoTranslateForWordsCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document != null && this.mainViewModel.Document.Pages != null)
			{
				if (!this.mainViewModel.TranslateWords)
				{
					this.mainViewModel.TranslateWords = true;
				}
				else
				{
					this.mainViewModel.TranslateWords = false;
				}
			}
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x00017408 File Offset: 0x00015608
		private async void DoOpenTranslationCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document != null && this.mainViewModel.Document.Pages != null)
			{
				this.mainViewModel.TranslatePanelVisible = true;
			}
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x00017440 File Offset: 0x00015640
		private async void DoLinkDelCmd(ContextMenuItemModel model)
		{
			AnnotationToolbarViewModel.<>c__DisplayClass204_0 CS$<>8__locals1 = new AnnotationToolbarViewModel.<>c__DisplayClass204_0();
			if (this.mainViewModel.Document != null && this.mainViewModel.Document.Pages != null)
			{
				GAManager.SendEvent("MainWindow", "Link", "DoLinkDelCmd", 1L);
				await this.mainViewModel.ReleaseViewerFocusAsync(true);
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				if (MessageBox.Show(Resources.LinkDeleteAll, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.No)
				{
					int count = this.mainViewModel.Document.Pages.Count;
					global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
					global::PDFKit.PdfControl pdfControl = viewer;
					if (pdfControl == null)
					{
						new global::System.ValueTuple<int, int>(-1, -1);
					}
					else
					{
						pdfControl.GetVisiblePageRange();
					}
					CS$<>8__locals1.dict = LinkOperationManagerExtensions.LinkDeleteAllRedo(this.mainViewModel.Document);
					if (CS$<>8__locals1.dict != null && CS$<>8__locals1.dict.Count > 0)
					{
						await this.mainViewModel.OperationManager.AddOperationAsync(delegate(PdfDocument _doc)
						{
							AnnotationToolbarViewModel.<>c__DisplayClass204_0.<<DoLinkDelCmd>b__0>d <<DoLinkDelCmd>b__0>d;
							<<DoLinkDelCmd>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
							<<DoLinkDelCmd>b__0>d.<>4__this = CS$<>8__locals1;
							<<DoLinkDelCmd>b__0>d._doc = _doc;
							<<DoLinkDelCmd>b__0>d.<>1__state = -1;
							<<DoLinkDelCmd>b__0>d.<>t__builder.Start<AnnotationToolbarViewModel.<>c__DisplayClass204_0.<<DoLinkDelCmd>b__0>d>(ref <<DoLinkDelCmd>b__0>d);
							return <<DoLinkDelCmd>b__0>d.<>t__builder.Task;
						}, delegate(PdfDocument _doc)
						{
							AnnotationToolbarViewModel.<>c__DisplayClass204_0.<<DoLinkDelCmd>b__1>d <<DoLinkDelCmd>b__1>d;
							<<DoLinkDelCmd>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
							<<DoLinkDelCmd>b__1>d.<>4__this = CS$<>8__locals1;
							<<DoLinkDelCmd>b__1>d._doc = _doc;
							<<DoLinkDelCmd>b__1>d.<>1__state = -1;
							<<DoLinkDelCmd>b__1>d.<>t__builder.Start<AnnotationToolbarViewModel.<>c__DisplayClass204_0.<<DoLinkDelCmd>b__1>d>(ref <<DoLinkDelCmd>b__1>d);
							return <<DoLinkDelCmd>b__1>d.<>t__builder.Task;
						}, "");
					}
					if (viewer != null)
					{
						await viewer.TryRedrawVisiblePageAsync(default(CancellationToken));
					}
				}
			}
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x00017478 File Offset: 0x00015678
		private async void DoAttachmentInsertCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document != null)
			{
				GAManager.SendEvent("MainWindow", "AttachmentToolbar", "DoAttachmentInsertCmd", 1L);
				await this.mainViewModel.ReleaseViewerFocusAsync(true);
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				this.mainViewModel.AnnotationMode = AnnotationMode.Attachment;
				await this.ProcessAttachmentAnnotationAsync();
				this.mainViewModel.AnnotationMode = AnnotationMode.None;
			}
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x000174B0 File Offset: 0x000156B0
		internal async Task ProcessAttachmentAnnotationAsync()
		{
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			PdfViewer viewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			AnnotationHolderManager holder = PdfObjectExtensions.GetAnnotationHolderManager(viewer);
			if (holder != null)
			{
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				holder.CancelAll();
				await holder.WaitForCancelCompletedAsync();
				global::System.Windows.Shapes.Rectangle rectangle = new global::System.Windows.Shapes.Rectangle
				{
					Fill = global::System.Windows.Media.Brushes.Transparent,
					Width = 1.0,
					Height = 1.0,
					Opacity = 0.01
				};
				Cursor cursor = new Cursor(Application.GetResourceStream(new Uri("/Style/Resources/attachment.cur", UriKind.Relative)).Stream);
				this.mainViewModel.ViewerOperationModel = new HoverOperationModel(viewer, cursor)
				{
					SizeInDocument = new FS_SIZEF(25f, 25f),
					PreviewElement = rectangle
				};
				ViewOperationResult<bool> viewOperationResult = await this.mainViewModel.ViewerOperationModel.Task;
				DataOperationModel viewerOperationModel = this.mainViewModel.ViewerOperationModel;
				if (viewOperationResult != null && viewOperationResult.Value)
				{
					holder.CancelAll();
					holder.Attachment.StartCreateNew(this.mainViewModel.Document.Pages[viewerOperationModel.CurrentPage], viewerOperationModel.PositionFromDocument);
					global::System.Collections.Generic.IReadOnlyList<PdfFileAttachmentAnnotation> readOnlyList = await holder.Attachment.CompleteCreateNewAsync();
					if (readOnlyList != null && readOnlyList.Count > 0)
					{
						holder.Select(readOnlyList[0], true);
					}
				}
			}
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x000174F4 File Offset: 0x000156F4
		private async void OpenAttachmentManagerCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document != null)
			{
				GAManager.SendEvent("MainWindow", "AttachmentToolbar", "OpenAttachmentManagerCmd", 1L);
				this.mainViewModel.Menus.SelectedLeftNavItem = this.mainViewModel.Menus.LeftNavList.First((NavigationModel x) => x.Name == "Attachment");
			}
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x0001752C File Offset: 0x0001572C
		private void DoToolbarSettingsItemCmd(ToolbarSettingItemModel model)
		{
			string text = AnnotationMenuPropertyAccessor.BuildPropertyName(model.Id.AnnotationMode, model.Type);
			switch (model.Id.AnnotationMode)
			{
			case AnnotationMode.Line:
				this.mainViewModel.AnnotationToolbar.LineButtonModel.IsChecked = true;
				break;
			case AnnotationMode.Ink:
				this.mainViewModel.AnnotationToolbar.InkButtonModel.IsChecked = true;
				break;
			case AnnotationMode.Shape:
				this.mainViewModel.AnnotationToolbar.SquareButtonModel.IsChecked = true;
				break;
			case AnnotationMode.Highlight:
				this.mainViewModel.AnnotationToolbar.HighlightButtonModel.IsChecked = true;
				break;
			case AnnotationMode.Underline:
				this.mainViewModel.AnnotationToolbar.UnderlineButtonModel.IsChecked = true;
				break;
			case AnnotationMode.Strike:
				this.mainViewModel.AnnotationToolbar.StrikeButtonModel.IsChecked = true;
				break;
			case AnnotationMode.HighlightArea:
				this.mainViewModel.AnnotationToolbar.HighlightAreaButtonModel.IsChecked = true;
				break;
			case AnnotationMode.Ellipse:
				this.mainViewModel.AnnotationToolbar.CircleButtonModel.IsChecked = true;
				break;
			case AnnotationMode.Text:
				this.mainViewModel.AnnotationToolbar.TextButtonModel.IsChecked = true;
				break;
			case AnnotationMode.TextBox:
				this.mainViewModel.AnnotationToolbar.TextBoxButtonModel.IsChecked = true;
				break;
			}
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			if (pdfControl != null)
			{
				AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
				int num = -1;
				if (annotationHolderManager == null)
				{
					return;
				}
				annotationHolderManager.OnPropertyChanged(text, out num);
			}
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x000176C3 File Offset: 0x000158C3
		private void DoToolbarSettingItemExitCmd(ToolbarSettingItemModel model)
		{
			this.mainViewModel.SelectedAnnotation = null;
			this.mainViewModel.AnnotationMode = AnnotationMode.None;
			this.NotifyCheckedButtonToolbarSettingChanged();
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x000176E4 File Offset: 0x000158E4
		private void DoToolbarSettingItemLinkExitCmd(ToolbarSettingItemModel model)
		{
			this.mainViewModel.SelectedAnnotation = null;
			this.mainViewModel.AnnotationMode = AnnotationMode.None;
			this.NotifyCheckedButtonToolbarSettingChanged();
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			if (pdfControl != null)
			{
				pdfControl.Viewer.IsLinkAnnotationHighlighted = false;
			}
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x00017730 File Offset: 0x00015930
		private void DoToolbarSettingItemImageExitCmd(ToolbarSettingItemModel model)
		{
			this.mainViewModel.SelectedAnnotation = null;
			this.mainViewModel.AnnotationMode = AnnotationMode.None;
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			AnnotationCanvas annotationCanvas = PdfObjectExtensions.GetAnnotationCanvas((pdfControl != null) ? pdfControl.Viewer : null);
			if (annotationCanvas != null)
			{
				annotationCanvas.ImageControl.quitImageControl();
			}
			this.NotifyCheckedButtonToolbarSettingChanged();
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0001778C File Offset: 0x0001598C
		private void InkToolbarSettingItemExitCmd(ToolbarSettingItemModel model)
		{
			(this.mainViewModel.AnnotationToolbar.inkButtonModel.ToolbarSettingModel[3] as ToolbarSettingInkEraserModel).IsChecked = false;
			this.mainViewModel.SelectedAnnotation = null;
			this.mainViewModel.AnnotationMode = AnnotationMode.None;
			this.NotifyCheckedButtonToolbarSettingChanged();
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x000177DD File Offset: 0x000159DD
		private void NotifyCheckedButtonToolbarSettingChanged()
		{
			base.OnPropertyChanged("CheckedButtonToolbarSetting");
			this.UpdateViewerToobarPadding();
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x000177F0 File Offset: 0x000159F0
		private void InitToolbarAnnotationButtonModel()
		{
			this.InitMenu();
			this.HighlightButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Highlight)
			{
				Caption = Resources.MenuAnnotateHighlightContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/highlighttext.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/highlighttext.png")),
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__TextMarkupCommandFunc|214_4)),
				ToolbarSettingModel = new ToolbarSettingModel(AnnotationMode.Highlight)
				{
					ToolbarSettingsHelper.CreateAnnotationModeIcon(AnnotationMode.Highlight),
					ToolbarSettingsHelper.CreateColor(AnnotationMode.Highlight, ContextMenuItemType.StrokeColor, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), null),
					ToolbarSettingsHelper.CreateExitEdit(new Action<ToolbarSettingItemModel>(this.DoToolbarSettingItemExitCmd)),
					ToolbarSettingsHelper.CreateApplyToDefault()
				}
			};
			this.UnderlineButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Underline)
			{
				Caption = Resources.MenuAnnotateUnderlineContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/underline.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/underline.png")),
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__TextMarkupCommandFunc|214_4)),
				ToolbarSettingModel = new ToolbarSettingModel(AnnotationMode.Underline)
				{
					ToolbarSettingsHelper.CreateAnnotationModeIcon(AnnotationMode.Underline),
					ToolbarSettingsHelper.CreateColor(AnnotationMode.Underline, ContextMenuItemType.StrokeColor, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), null),
					ToolbarSettingsHelper.CreateExitEdit(new Action<ToolbarSettingItemModel>(this.DoToolbarSettingItemExitCmd)),
					ToolbarSettingsHelper.CreateApplyToDefault()
				}
			};
			this.StrikeButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Strike)
			{
				Caption = Resources.MenuAnnotateStrikeContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/strike.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/strike.png")),
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__TextMarkupCommandFunc|214_4)),
				ToolbarSettingModel = new ToolbarSettingModel(AnnotationMode.Strike)
				{
					ToolbarSettingsHelper.CreateAnnotationModeIcon(AnnotationMode.Strike),
					ToolbarSettingsHelper.CreateColor(AnnotationMode.Strike, ContextMenuItemType.StrokeColor, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), null),
					ToolbarSettingsHelper.CreateExitEdit(new Action<ToolbarSettingItemModel>(this.DoToolbarSettingItemExitCmd)),
					ToolbarSettingsHelper.CreateApplyToDefault()
				}
			};
			this.LineButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Line)
			{
				Caption = Resources.MenuAnnotateLineContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/line.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/line.png")),
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0)),
				ToolbarSettingModel = new ToolbarSettingModel(AnnotationMode.Line)
				{
					ToolbarSettingsHelper.CreateAnnotationModeIcon(AnnotationMode.Line),
					ToolbarSettingsHelper.CreateStrokeThickness(AnnotationMode.Line, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), null),
					ToolbarSettingsHelper.CreateCollapsedColor(AnnotationMode.Line, ContextMenuItemType.StrokeColor, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), ToolbarSettingsHelper.CreateIcon(ContextMenuItemType.StrokeColor)),
					ToolbarSettingsHelper.CreateExitEdit(new Action<ToolbarSettingItemModel>(this.DoToolbarSettingItemExitCmd)),
					ToolbarSettingsHelper.CreateApplyToDefault()
				}
			};
			this.SquareButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Shape)
			{
				Caption = Resources.MenuAnnotateShapeContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/shape.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/shape.png")),
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0)),
				ToolbarSettingModel = new ToolbarSettingModel(AnnotationMode.Shape)
				{
					ToolbarSettingsHelper.CreateAnnotationModeIcon(AnnotationMode.Shape),
					ToolbarSettingsHelper.CreateStrokeThickness(AnnotationMode.Shape, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), null),
					ToolbarSettingsHelper.CreateCollapsedColor(AnnotationMode.Shape, ContextMenuItemType.StrokeColor, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), ToolbarSettingsHelper.CreateIcon(ContextMenuItemType.StrokeColor)),
					ToolbarSettingsHelper.CreateCollapsedColor(AnnotationMode.Shape, ContextMenuItemType.FillColor, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), ToolbarSettingsHelper.CreateIcon(ContextMenuItemType.FillColor)),
					ToolbarSettingsHelper.CreateExitEdit(new Action<ToolbarSettingItemModel>(this.DoToolbarSettingItemExitCmd)),
					ToolbarSettingsHelper.CreateApplyToDefault()
				}
			};
			this.CircleButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Ellipse)
			{
				Caption = Resources.MenuAnnotateEllipseContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/ellipse.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/ellipse.png")),
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0)),
				ToolbarSettingModel = new ToolbarSettingModel(AnnotationMode.Ellipse)
				{
					ToolbarSettingsHelper.CreateAnnotationModeIcon(AnnotationMode.Ellipse),
					ToolbarSettingsHelper.CreateStrokeThickness(AnnotationMode.Ellipse, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), null),
					ToolbarSettingsHelper.CreateCollapsedColor(AnnotationMode.Ellipse, ContextMenuItemType.StrokeColor, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), ToolbarSettingsHelper.CreateIcon(ContextMenuItemType.StrokeColor)),
					ToolbarSettingsHelper.CreateCollapsedColor(AnnotationMode.Ellipse, ContextMenuItemType.FillColor, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), ToolbarSettingsHelper.CreateIcon(ContextMenuItemType.FillColor)),
					ToolbarSettingsHelper.CreateExitEdit(new Action<ToolbarSettingItemModel>(this.DoToolbarSettingItemExitCmd)),
					ToolbarSettingsHelper.CreateApplyToDefault()
				}
			};
			this.InkButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Ink)
			{
				Caption = Resources.MenuAnnotateInkContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/Ink.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/Ink.png")),
				Tooltip = Resources.MenuAnnotateInkContent,
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__InkCommandFunc|214_9)),
				ToolbarSettingModel = new ToolbarSettingModel(AnnotationMode.Ink)
				{
					ToolbarSettingsHelper.CreateAnnotationModeIcon(AnnotationMode.Ink),
					ToolbarSettingsHelper.CreateStrokeThickness(AnnotationMode.Ink, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), null),
					ToolbarSettingsHelper.CreateColor(AnnotationMode.Ink, ContextMenuItemType.StrokeColor, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), null),
					ToolbarSettingsHelper.CreteEreserState(AnnotationMode.Ink, "Eraser", false, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd)),
					ToolbarSettingsHelper.CreateExitEdit(new Action<ToolbarSettingItemModel>(this.InkToolbarSettingItemExitCmd)),
					ToolbarSettingsHelper.CreateApplyToDefault()
				}
			};
			this.HighlightAreaButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.HighlightArea)
			{
				Caption = Resources.WinToolBarBtnHighlightContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/highlightarea2.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/highlightarea2.png")),
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0)),
				ToolbarSettingModel = new ToolbarSettingModel(AnnotationMode.HighlightArea)
				{
					ToolbarSettingsHelper.CreateAnnotationModeIcon(AnnotationMode.HighlightArea),
					ToolbarSettingsHelper.CreateColor(AnnotationMode.HighlightArea, ContextMenuItemType.StrokeColor, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), null),
					ToolbarSettingsHelper.CreateExitEdit(new Action<ToolbarSettingItemModel>(this.DoToolbarSettingItemExitCmd)),
					ToolbarSettingsHelper.CreateApplyToDefault()
				}
			};
			this.TextBoxButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.TextBox)
			{
				Caption = Resources.MenuAnnotateTextBoxContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/textbox.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/textbox.png")),
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0)),
				ToolbarSettingModel = new ToolbarSettingModel(AnnotationMode.TextBox)
				{
					ToolbarSettingsHelper.CreateAnnotationModeIcon(AnnotationMode.TextBox),
					ToolbarSettingsHelper.CreateFontSize(AnnotationMode.TextBox, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), null),
					ToolbarSettingsHelper.CreateCollapsedColor(AnnotationMode.TextBox, ContextMenuItemType.FontColor, "Test1", new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), ToolbarSettingsHelper.CreateIcon(ContextMenuItemType.FontColor)),
					ToolbarSettingsHelper.CreateStrokeThickness(AnnotationMode.TextBox, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), null),
					ToolbarSettingsHelper.CreateCollapsedColor(AnnotationMode.TextBox, ContextMenuItemType.StrokeColor, "Test1", new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), ToolbarSettingsHelper.CreateIcon(ContextMenuItemType.StrokeColor)),
					ToolbarSettingsHelper.CreateCollapsedColor(AnnotationMode.TextBox, ContextMenuItemType.FillColor, "Test1", new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), ToolbarSettingsHelper.CreateIcon(ContextMenuItemType.FillColor)),
					ToolbarSettingsHelper.CreateExitEdit(new Action<ToolbarSettingItemModel>(this.DoToolbarSettingItemExitCmd)),
					ToolbarSettingsHelper.CreateApplyToDefault()
				}
			};
			this.TextButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Text)
			{
				Caption = Resources.MenuAnnotateTypeWriterContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/insertText.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/insertText.png")),
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__TextCommandFunc|214_1)),
				ToolbarSettingModel = new ToolbarSettingModel(AnnotationMode.Text)
				{
					ToolbarSettingsHelper.CreateAnnotationModeIcon(AnnotationMode.Text),
					ToolbarSettingsHelper.CreateFontSize(AnnotationMode.Text, new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), null),
					ToolbarSettingsHelper.CreateCollapsedColor(AnnotationMode.Text, ContextMenuItemType.FontColor, "Test1", new Action<ToolbarSettingItemModel>(this.DoToolbarSettingsItemCmd), ToolbarSettingsHelper.CreateIcon(ContextMenuItemType.FontColor)),
					ToolbarSettingsHelper.CreateExitEdit(new Action<ToolbarSettingItemModel>(this.DoToolbarSettingItemExitCmd)),
					ToolbarSettingsHelper.CreateApplyToDefault()
				}
			};
			this.NoteButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Note)
			{
				Caption = Resources.MenuAnnotateNoteContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/note.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/note.png")),
				ChildButtonModel = null,
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__NoteCommandFunc|214_2))
			};
			this.StampButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Stamp)
			{
				Caption = Resources.MenuAnnotateStampContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/stamp.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/stamp.png")),
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					ContextMenu = this.stampMenuItems
				},
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__StampCommandFunc|214_5))
			};
			this.ShareButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.None)
			{
				Icon = new BitmapImage(new Uri("/Style/Resources/ShareBtn.png", UriKind.Relative)),
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					ContextMenu = this.shareMenuItem
				},
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__StampCommandFunc|214_5))
			};
			this.ImageButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Image)
			{
				Caption = Resources.MenuInsertImageContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/InsertPicture.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/InsertPicture.png")),
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__InsertImageCommandFunc|214_6)),
				ToolbarSettingModel = new ToolbarSettingModel(AnnotationMode.Image)
				{
					ToolbarSettingsHelper.CreateText(Resources.ImageToolbarTitle),
					ToolbarSettingsHelper.CreateImageExitEdit(new Action<ToolbarSettingItemModel>(this.DoToolbarSettingItemImageExitCmd))
				}
			};
			this.SignatureButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Signature)
			{
				Caption = Resources.MenuAnnotateSignatureContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/signature.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/signature.png")),
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					ContextMenu = this.signatureMenuItems
				},
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__SignatureCommandFunc|214_7))
			};
			this.WatermarkButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Watermark)
			{
				Caption = Resources.MenuAnnotateWatermarkContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/Watermark.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/Watermark.png")),
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					ContextMenu = this.waterMenuItems
				},
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__WatermarkCommandFunc|214_10))
			};
			this.LinkButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Link)
			{
				Caption = Resources.LinkBtn,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/Link.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/Link.png")),
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					ContextMenu = this.linkMenuItems
				},
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__LinkCommandFunc|214_8)),
				ToolbarSettingModel = new ToolbarSettingModel(AnnotationMode.Link)
				{
					ToolbarSettingsHelper.CreateText(Resources.LinkToolbarTitle),
					ToolbarSettingsHelper.CreateImageExitEdit(new Action<ToolbarSettingItemModel>(this.DoToolbarSettingItemLinkExitCmd))
				}
			};
			this.DigitalSignatureButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.None)
			{
				Caption = Resources.SigDialogDigSignBtn2,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Protect/DigialSign.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Protect/DigialSign.png")),
				Command = new AsyncRelayCommand<ToolbarAnnotationButtonModel>(async delegate([Nullable(2)] ToolbarAnnotationButtonModel model)
				{
					GAManager.SendEvent("DigitalSignature", "AddDigitalSignature", "Toolbar", 1L);
					await this.DoDigitalSignatureAsync();
				})
			};
			this.AddDeferredDigitalSignatureButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.None)
			{
				Caption = Resources.ResourceManager.GetString("AddDeferredDigitSignBtn"),
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Protect/DeferredDigialSign.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Protect/DeferredDigialSign.png")),
				Command = new AsyncRelayCommand<ToolbarAnnotationButtonModel>(async delegate([Nullable(2)] ToolbarAnnotationButtonModel model)
				{
					GAManager.SendEvent("DigitalSignature", "AddDigitalSignatureFiled", "Toolbar", 1L);
					await this.CreateDeferredDigitalSignatureFieldAsync();
				})
			};
			this.CertificateManagerButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.None)
			{
				Caption = Resources.ResourceManager.GetString("CertificateManagerBtn"),
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Protect/CertManager.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Protect/CertManager.png")),
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					ContextMenu = this.certManagerItems
				},
				Command = new AsyncRelayCommand<ToolbarAnnotationButtonModel>(async delegate([Nullable(2)] ToolbarAnnotationButtonModel model)
				{
					this.CertificateManagerButtonModel.ChildButtonModel.Tap();
				})
			};
			ToolbarAnnotationButtonModel toolbarAnnotationButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.None);
			toolbarAnnotationButtonModel.Caption = "Translate";
			toolbarAnnotationButtonModel.Icon = new BitmapImage(new Uri("/Style/Resources/Annonate/Translate.png", UriKind.Relative));
			toolbarAnnotationButtonModel.ChildButtonModel = new ToolbarChildCheckableButtonModel
			{
				ContextMenu = this.translateMenuItems
			};
			toolbarAnnotationButtonModel.Command = new AsyncRelayCommand<ToolbarAnnotationButtonModel>(async delegate([Nullable(2)] ToolbarAnnotationButtonModel model)
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				requiredService.TranslateWords = !requiredService.TranslateWords;
			});
			this.TranslateButtonModel = toolbarAnnotationButtonModel;
			ToolbarAnnotationButtonModel toolbarAnnotationButtonModel2 = new ToolbarAnnotationButtonModel(AnnotationMode.None);
			toolbarAnnotationButtonModel2.Caption = Resources.RedactBtn;
			toolbarAnnotationButtonModel2.Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Protect/Redact.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Protect/Redact.png"));
			toolbarAnnotationButtonModel2.IsCheckable = true;
			toolbarAnnotationButtonModel2.Command = new AsyncRelayCommand<ToolbarAnnotationButtonModel>(async delegate([Nullable(2)] ToolbarAnnotationButtonModel model)
			{
				this.NotifyCheckedButtonToolbarSettingChanged();
				if (this.RedactionButtonModel.IsChecked)
				{
					GAManager.SendEvent("Redaction", "RedactionBtn", "Toolbar", 1L);
					await this.DoRedactionCommand();
				}
			}, AsyncRelayCommandOptions.AllowConcurrentExecutions);
			ToolbarSettingModel toolbarSettingModel = new ToolbarSettingModel(ToolbarSettingId.CreateRedact());
			toolbarSettingModel.Add(ToolbarSettingsHelper.CreateColor(AnnotationMode.Redact, ContextMenuItemType.FillColor, delegate(ToolbarSettingItemModel model)
			{
			}, ToolbarSettingsHelper.CreateIcon(ContextMenuItemType.FillColor)));
			toolbarSettingModel.Add(ToolbarSettingsHelper.CreateImageExitEdit(Resources.RedactToolbarExitText, delegate(ToolbarSettingItemModel model)
			{
				this.RedactionButtonModel.IsChecked = false;
				DataOperationModel viewerOperationModel = this.mainViewModel.ViewerOperationModel;
				if (viewerOperationModel != null)
				{
					viewerOperationModel.Dispose();
				}
				this.mainViewModel.ViewerOperationModel = null;
				this.NotifyCheckedButtonToolbarSettingChanged();
			}));
			toolbarAnnotationButtonModel2.ToolbarSettingModel = toolbarSettingModel;
			this.RedactionButtonModel = toolbarAnnotationButtonModel2;
			this.AttachmentButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.Attachment)
			{
				Caption = Resources.AnnotationFileAttachment,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/Attachment.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/Attachment.png")),
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					ContextMenu = this.attachmentMenuItems
				},
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.<InitToolbarAnnotationButtonModel>g__AttachmentCommandFunc|214_11))
			};
			this.allAnnotationButton = new List<ToolbarAnnotationButtonModel>
			{
				this.UnderlineButtonModel, this.StrikeButtonModel, this.HighlightButtonModel, this.LineButtonModel, this.InkButtonModel, this.SquareButtonModel, this.CircleButtonModel, this.HighlightAreaButtonModel, this.TextButtonModel, this.TextBoxButtonModel,
				this.NoteButtonModel, this.StampButtonModel, this.SignatureButtonModel, this.WatermarkButtonModel, this.shareButtonModel, this.LinkButtonModel, this.DigitalSignatureButtonModel, this.AddDeferredDigitalSignatureButtonModel, this.CertificateManagerButtonModel, this.RedactionButtonModel,
				this.ImageButtonModel, this.TranslateButtonModel, this.AttachmentButtonModel
			};
			foreach (ToolbarAnnotationButtonModel toolbarAnnotationButtonModel3 in this.allAnnotationButton)
			{
				toolbarAnnotationButtonModel3.ContextMenuSelectionChanged += this.ToolbarButton_ContextMenuSelectionChanged;
			}
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x000186D4 File Offset: 0x000168D4
		private void ToolbarButton_ContextMenuSelectionChanged(object sender, SelectedAccessorSelectionChangedEventArgs e)
		{
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x000186D6 File Offset: 0x000168D6
		public void SetMenuItemValue()
		{
			this.ClearAdditionalMenuItem();
			this.TryBeginTransient();
			this.NotifyCheckedButtonToolbarSettingChanged();
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x000186EC File Offset: 0x000168EC
		private void TryBeginTransient()
		{
			PdfAnnotation selectedAnnotation = this.mainViewModel.SelectedAnnotation;
			if (selectedAnnotation == null)
			{
				return;
			}
			List<AnnotationMode> modes = AnnotationFactory.GetAnnotationModes(selectedAnnotation).ToList<AnnotationMode>();
			ToolbarSettingModel[] array = (from c in this.AllAnnotationButton
				where modes.Contains(c.Mode) && c.ToolbarSettingModel != null
				select c.ToolbarSettingModel into c
				orderby modes.IndexOf(c.Id.AnnotationMode)
				select c).ToArray<ToolbarSettingModel>();
			if (array.Length != 0)
			{
				PdfAnnotationPropertyAccessor pdfAnnotationPropertyAccessor = new PdfAnnotationPropertyAccessor(selectedAnnotation, array[0].Id.AnnotationMode);
				foreach (ToolbarSettingItemModel toolbarSettingItemModel in array[0])
				{
					string text;
					object obj;
					if (ToolbarContextMenuHelper.TryParseMenuValue(array[0].Id.AnnotationMode, toolbarSettingItemModel.Type, pdfAnnotationPropertyAccessor.GetValue(toolbarSettingItemModel.Type), out text, out obj))
					{
						toolbarSettingItemModel.BeginTransient(obj);
					}
					else if (toolbarSettingItemModel is ToolbarSettingItemApplyToDefaultModel)
					{
						toolbarSettingItemModel.BeginTransient(null);
					}
				}
			}
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x00018818 File Offset: 0x00016A18
		private void TryEndAllTransient()
		{
			foreach (ToolbarAnnotationButtonModel toolbarAnnotationButtonModel in this.AllAnnotationButton)
			{
				if (toolbarAnnotationButtonModel.ToolbarSettingModel != null)
				{
					foreach (ToolbarSettingItemModel toolbarSettingItemModel in toolbarAnnotationButtonModel.ToolbarSettingModel)
					{
						toolbarSettingItemModel.EndTransient();
					}
				}
			}
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x000188A0 File Offset: 0x00016AA0
		private void ClearAdditionalMenuItem()
		{
			this.TryEndAllTransient();
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x000188A8 File Offset: 0x00016AA8
		private void TrySetDefaultValueCore(TypedContextMenuItemModel menu)
		{
			TypedContextMenuModel typedContextMenuModel = menu.Parent as TypedContextMenuModel;
			if (typedContextMenuModel != null)
			{
				ContextMenuItemModel defaultMenuItem = ToolbarContextMenuHelper.GetDefaultMenuItem(typedContextMenuModel.Mode, menu);
				if (defaultMenuItem != null)
				{
					defaultMenuItem.IsChecked = true;
				}
			}
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x000188DC File Offset: 0x00016ADC
		private bool SetButtonProperty(ref ToolbarAnnotationButtonModel field, ToolbarAnnotationButtonModel value, [CallerMemberName] string propName = "")
		{
			ToolbarAnnotationButtonModel toolbarAnnotationButtonModel = field;
			if (base.SetProperty<ToolbarAnnotationButtonModel>(ref field, value, propName))
			{
				if (toolbarAnnotationButtonModel != null)
				{
					toolbarAnnotationButtonModel.PropertyChanged -= this.ToolbarButtonModel_PropertyChanged;
				}
				if (value != null)
				{
					value.PropertyChanged += this.ToolbarButtonModel_PropertyChanged;
				}
				return true;
			}
			return false;
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x00018924 File Offset: 0x00016B24
		private void ToolbarButtonModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.NotifyCheckedButtonToolbarSettingChanged();
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0001892C File Offset: 0x00016B2C
		public async Task SaveToolbarSettingsConfigAsync()
		{
			await Task.WhenAll((from c in this.AllAnnotationButton
				select c.ToolbarSettingModel into c
				where c != null
				select c).Select(delegate(ToolbarSettingModel c)
			{
				AnnotationToolbarViewModel.<>c__DisplayClass223_0 CS$<>8__locals1 = new AnnotationToolbarViewModel.<>c__DisplayClass223_0();
				CS$<>8__locals1.c = c;
				return delegate
				{
					AnnotationToolbarViewModel.<>c__DisplayClass223_0.<<SaveToolbarSettingsConfigAsync>b__3>d <<SaveToolbarSettingsConfigAsync>b__3>d;
					<<SaveToolbarSettingsConfigAsync>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<SaveToolbarSettingsConfigAsync>b__3>d.<>4__this = CS$<>8__locals1;
					<<SaveToolbarSettingsConfigAsync>b__3>d.<>1__state = -1;
					<<SaveToolbarSettingsConfigAsync>b__3>d.<>t__builder.Start<AnnotationToolbarViewModel.<>c__DisplayClass223_0.<<SaveToolbarSettingsConfigAsync>b__3>d>(ref <<SaveToolbarSettingsConfigAsync>b__3>d);
					return <<SaveToolbarSettingsConfigAsync>b__3>d.<>t__builder.Task;
				}();
			}));
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x00018970 File Offset: 0x00016B70
		public async Task LoadToolbarSettingsConfigAsync()
		{
			await Task.WhenAll((from c in this.AllAnnotationButton
				select c.ToolbarSettingModel into c
				where c != null
				select c).Select(delegate(ToolbarSettingModel c)
			{
				AnnotationToolbarViewModel.<>c__DisplayClass224_0 CS$<>8__locals1 = new AnnotationToolbarViewModel.<>c__DisplayClass224_0();
				CS$<>8__locals1.c = c;
				return delegate
				{
					AnnotationToolbarViewModel.<>c__DisplayClass224_0.<<LoadToolbarSettingsConfigAsync>b__3>d <<LoadToolbarSettingsConfigAsync>b__3>d;
					<<LoadToolbarSettingsConfigAsync>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<LoadToolbarSettingsConfigAsync>b__3>d.<>4__this = CS$<>8__locals1;
					<<LoadToolbarSettingsConfigAsync>b__3>d.<>1__state = -1;
					<<LoadToolbarSettingsConfigAsync>b__3>d.<>t__builder.Start<AnnotationToolbarViewModel.<>c__DisplayClass224_0.<<LoadToolbarSettingsConfigAsync>b__3>d>(ref <<LoadToolbarSettingsConfigAsync>b__3>d);
					return <<LoadToolbarSettingsConfigAsync>b__3>d.<>t__builder.Task;
				}();
			}));
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x000189B4 File Offset: 0x00016BB4
		public async Task RemoveSignatureAnnotionAsync(PdfDocument doc)
		{
			if (doc != null && doc.Pages != null && doc.Pages.Count != 0)
			{
				Action<PdfPage> action = this.RemoveImageStampFunc();
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(doc);
				global::System.ValueTuple<int, int> valueTuple = ((pdfControl != null) ? pdfControl.GetVisiblePageRange() : new global::System.ValueTuple<int, int>(-1, -1));
				int item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				if (action != null)
				{
					for (int i = 0; i < doc.Pages.Count; i++)
					{
						int pageIndex = doc.Pages[i].PageIndex;
						IntPtr intPtr = IntPtr.Zero;
						PdfPage pdfPage = null;
						try
						{
							intPtr = Pdfium.FPDF_LoadPage(doc.Handle, pageIndex);
							if (intPtr != IntPtr.Zero)
							{
								pdfPage = PdfPage.FromHandle(doc, intPtr, pageIndex, true);
								action(pdfPage);
							}
						}
						finally
						{
							if (pdfPage != null && (pdfPage.PageIndex > item2 || pdfPage.PageIndex < item))
							{
								PageDisposeHelper.DisposePage(pdfPage);
							}
							if (intPtr != IntPtr.Zero)
							{
								Pdfium.FPDF_ClosePage(intPtr);
							}
						}
					}
				}
				if (pdfControl != null)
				{
					await pdfControl.TryRedrawVisiblePageAsync(default(CancellationToken));
				}
			}
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x000189FF File Offset: 0x00016BFF
		private Action<PdfPage> RemoveImageStampFunc()
		{
			return delegate(PdfPage p)
			{
				PdfAnnotationCollection annots = p.Annots;
				object obj;
				if (annots == null)
				{
					obj = null;
				}
				else
				{
					obj = annots.OfType<PdfStampAnnotation>().ToList<PdfStampAnnotation>().FindAll((PdfStampAnnotation x) => x.Subject == "Signature");
				}
				object obj2 = obj;
				if (obj2 == null)
				{
					return;
				}
				obj2.ForEach(delegate(PdfStampAnnotation a)
				{
					PdfAnnotationExtensions.WaitForAnnotationGenerateAsync();
					a.DeleteAnnotation();
					PageEditorViewModel pageEditors = this.mainViewModel.PageEditors;
					if (pageEditors != null)
					{
						pageEditors.NotifyPageAnnotationChanged(p.PageIndex);
					}
					p.TryRedrawPageAsync(default(CancellationToken));
				});
			};
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x00018A10 File Offset: 0x00016C10
		public async Task ConvertSignatureObj(PdfDocument doc, IProgress<double> progress)
		{
			AnnotationToolbarViewModel.<>c__DisplayClass227_0 CS$<>8__locals1 = new AnnotationToolbarViewModel.<>c__DisplayClass227_0();
			CS$<>8__locals1.doc = doc;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.progress = progress;
			if (CS$<>8__locals1.doc != null && CS$<>8__locals1.doc.Pages.Count != 0)
			{
				IProgress<double> progress2 = CS$<>8__locals1.progress;
				if (progress2 != null)
				{
					progress2.Report(0.0);
				}
				await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
				{
					AnnotationToolbarViewModel.<>c__DisplayClass227_0.<<ConvertSignatureObj>b__0>d <<ConvertSignatureObj>b__0>d;
					<<ConvertSignatureObj>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<ConvertSignatureObj>b__0>d.<>4__this = CS$<>8__locals1;
					<<ConvertSignatureObj>b__0>d.<>1__state = -1;
					<<ConvertSignatureObj>b__0>d.<>t__builder.Start<AnnotationToolbarViewModel.<>c__DisplayClass227_0.<<ConvertSignatureObj>b__0>d>(ref <<ConvertSignatureObj>b__0>d);
					return <<ConvertSignatureObj>b__0>d.<>t__builder.Task;
				})).ConfigureAwait(false);
			}
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x00018A64 File Offset: 0x00016C64
		private async Task AddEmbedSignatureObjAsync(PdfPage page)
		{
			PdfAnnotationCollection annots = page.Annots;
			List<PdfStampAnnotation> list;
			if (annots == null)
			{
				list = null;
			}
			else
			{
				list = annots.OfType<PdfStampAnnotation>().ToList<PdfStampAnnotation>().FindAll((PdfStampAnnotation x) => x.Subject == "Signature");
			}
			List<PdfStampAnnotation> imgStamps = list;
			for (int i = 0; i < imgStamps.Count; i++)
			{
				PdfStampAnnotation annot = imgStamps[i];
				await StampUtil.FlattenAnnotationAsync(annot);
				annot.DeleteAnnotation();
				annot = null;
			}
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00018AA8 File Offset: 0x00016CA8
		public bool FindApplySignature(PdfDocument doc)
		{
			if (doc == null || doc.Pages.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < doc.Pages.Count; i++)
			{
				PdfPage pdfPage = doc.Pages[i];
				if (pdfPage.Annots != null)
				{
					PdfAnnotationCollection annots = pdfPage.Annots;
					bool flag;
					if (annots == null)
					{
						flag = false;
					}
					else
					{
						flag = annots.OfType<PdfStampAnnotation>().ToList<PdfStampAnnotation>().FindAll((PdfStampAnnotation x) => x.Subject == "Signature")
							.Count == 0;
					}
					if (!flag)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00018B3A File Offset: 0x00016D3A
		public void NotifyPropertyChanged(string propName)
		{
			base.OnPropertyChanged(propName);
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00018B44 File Offset: 0x00016D44
		public void ChageCategroyName(string oldcategroyName, string newcategroyName)
		{
			try
			{
				string localJsonPath = AnnotationToolbarViewModel.GetLocalJsonPath();
				List<DynamicStampTextModel> list = ToolbarContextMenuHelper.ReadDynamicStamp(false);
				if (list == null)
				{
					list = new List<DynamicStampTextModel>();
				}
				foreach (DynamicStampTextModel dynamicStampTextModel in list)
				{
					if (dynamicStampTextModel.GroupName == oldcategroyName)
					{
						dynamicStampTextModel.GroupName = newcategroyName;
					}
				}
				using (FileStream fileStream = new FileStream(localJsonPath, FileMode.Create, FileAccess.ReadWrite))
				{
					using (StreamWriter streamWriter = new StreamWriter(fileStream))
					{
						string text = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
						{
							TypeNameHandling = TypeNameHandling.Auto
						});
						streamWriter.Write(text);
						streamWriter.Close();
					}
					fileStream.Close();
				}
				this.ReflashStampList();
			}
			catch
			{
			}
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00018C40 File Offset: 0x00016E40
		public void SaveStamp(DynamicStampTextModel stampTextModel)
		{
			try
			{
				string localJsonPath = AnnotationToolbarViewModel.GetLocalJsonPath();
				stampTextModel.GroupId = Guid.NewGuid().ToString();
				stampTextModel.dateTime = DateTime.Now;
				List<DynamicStampTextModel> list = ToolbarContextMenuHelper.ReadDynamicStamp(false);
				if (list == null)
				{
					list = new List<DynamicStampTextModel>();
				}
				list.Add(stampTextModel);
				using (FileStream fileStream = new FileStream(localJsonPath, FileMode.Create, FileAccess.ReadWrite))
				{
					using (StreamWriter streamWriter = new StreamWriter(fileStream))
					{
						string text = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
						{
							TypeNameHandling = TypeNameHandling.Auto
						});
						streamWriter.Write(text);
						streamWriter.Close();
					}
					fileStream.Close();
				}
				this.ReflashStampList();
			}
			catch
			{
			}
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00018D10 File Offset: 0x00016F10
		public void ReflashStampList()
		{
			List<DynamicStampTextModel> list = ToolbarContextMenuHelper.ReadDynamicStamp(false);
			if (this.stampMenuItems.Count > 0)
			{
				for (int i = this.stampMenuItems.Count - 1; i > 1; i--)
				{
					this.stampMenuItems.RemoveAt(i);
				}
			}
			if (list != null && list.Count > 0)
			{
				this.stampMenuItems.Add(new ContextMenuSeparator());
				List<string> list2 = new List<string>();
				for (int j = 0; j < list.Count; j++)
				{
					DynamicStampTextModel stamp = list[j];
					if (list2.FindIndex((string x) => x == stamp.GroupName) == -1)
					{
						list2.Add(stamp.GroupName);
						this.StampMenuItems.Add(ToolbarContextMenuHelper.ManageStampMenu(stamp.GroupName, AnnotationMode.Stamp, new Action<ContextMenuItemModel>(this.DoStampCmd)));
					}
					if (j == list.Count - 1)
					{
						this.stampMenuItems.Add(new ContextMenuSeparator());
						this.StampMenuItems.Add(ToolbarContextMenuHelper.CreatePresetsMenu(AnnotationMode.Stamp, new Action<ContextMenuItemModel>(this.DoStampPresetsCmd)));
					}
				}
				return;
			}
			this.stampMenuItems.Add(new ContextMenuSeparator());
			this.StampMenuItems.Add(ToolbarContextMenuHelper.CreatePresetsMenu(AnnotationMode.Stamp, new Action<ContextMenuItemModel>(this.DoStampPresetsCmd)));
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x00018E64 File Offset: 0x00017064
		private void SaveStamp(StampTextModel stampTextModel)
		{
			try
			{
				string localJsonPath = AnnotationToolbarViewModel.GetLocalJsonPath();
				stampTextModel.GroupId = Guid.NewGuid().ToString();
				stampTextModel.dateTime = DateTime.Now;
				List<StampTextModel> list = ToolbarContextMenuHelper.ReadStamp();
				if (list == null)
				{
					list = new List<StampTextModel>();
				}
				list.Add(stampTextModel);
				using (FileStream fileStream = new FileStream(localJsonPath, FileMode.Create, FileAccess.ReadWrite))
				{
					using (StreamWriter streamWriter = new StreamWriter(fileStream))
					{
						string text = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
						{
							TypeNameHandling = TypeNameHandling.Auto
						});
						streamWriter.Write(text);
						streamWriter.Close();
					}
					fileStream.Close();
				}
			}
			catch
			{
			}
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x00018F30 File Offset: 0x00017130
		public static string GetLocalJsonPath()
		{
			string text = global::System.IO.Path.Combine(AppDataHelper.LocalCacheFolder, "Config");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return global::System.IO.Path.Combine(text, "Stamp.json");
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x0001911C File Offset: 0x0001731C
		[CompilerGenerated]
		internal static double? <UpdateViewerToobarPadding>g__GetToolbarSettingHeight|172_0(FrameworkElement _container)
		{
			UserControl userControl = _container.FindName("AnnotToolbarSettingPanel") as UserControl;
			if (userControl != null)
			{
				FrameworkElement frameworkElement = userControl.Content as FrameworkElement;
				if (frameworkElement != null)
				{
					FrameworkElement frameworkElement2 = frameworkElement.FindName("ClipBorder") as FrameworkElement;
					if (frameworkElement2 != null)
					{
						return new double?(Math.Max(frameworkElement2.ActualHeight, frameworkElement2.Height) + frameworkElement2.Margin.Top);
					}
				}
			}
			return null;
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x00019308 File Offset: 0x00017508
		[CompilerGenerated]
		private void <InitToolbarAnnotationButtonModel>g__CommandFunc|214_0(ToolbarAnnotationButtonModel model)
		{
			if (AnnotationToolbarViewModel.<>o__214.<>p__2 == null)
			{
				AnnotationToolbarViewModel.<>o__214.<>p__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(AnnotationToolbarViewModel), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target = AnnotationToolbarViewModel.<>o__214.<>p__2.Target;
			CallSite <>p__ = AnnotationToolbarViewModel.<>o__214.<>p__2;
			if (AnnotationToolbarViewModel.<>o__214.<>p__1 == null)
			{
				AnnotationToolbarViewModel.<>o__214.<>p__1 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof(AnnotationToolbarViewModel), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, MouseModes, object> target2 = AnnotationToolbarViewModel.<>o__214.<>p__1.Target;
			CallSite <>p__2 = AnnotationToolbarViewModel.<>o__214.<>p__1;
			if (AnnotationToolbarViewModel.<>o__214.<>p__0 == null)
			{
				AnnotationToolbarViewModel.<>o__214.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(AnnotationToolbarViewModel), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			if (target(<>p__, target2(<>p__2, AnnotationToolbarViewModel.<>o__214.<>p__0.Target(AnnotationToolbarViewModel.<>o__214.<>p__0, this.mainViewModel.ViewerMouseMode), MouseModes.Default)))
			{
				if (AnnotationToolbarViewModel.<>o__214.<>p__3 == null)
				{
					AnnotationToolbarViewModel.<>o__214.<>p__3 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Value", typeof(AnnotationToolbarViewModel), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				AnnotationToolbarViewModel.<>o__214.<>p__3.Target(AnnotationToolbarViewModel.<>o__214.<>p__3, this.mainViewModel.ViewerMouseMode, MouseModes.Default);
			}
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			if (this.mainViewModel.AnnotationMode != AnnotationMode.Link)
			{
				pdfControl.Viewer.IsLinkAnnotationHighlighted = false;
			}
			if (this.mainViewModel.AnnotationToolbar.imageButtonModel.IsChecked)
			{
				AnnotationCanvas annotationCanvas = PdfObjectExtensions.GetAnnotationCanvas(global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document));
				if (annotationCanvas != null && annotationCanvas.ImageControl.ImageControlState)
				{
					annotationCanvas.ImageControl.quitImageControl();
				}
			}
			if (pdfControl != null)
			{
				AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
				if (((annotationHolderManager != null) ? annotationHolderManager.CurrentHolder : null) is StampAnnotationHolder && annotationHolderManager != null && annotationHolderManager.CurrentHolder.State == AnnotationHolderState.CreatingNew)
				{
					AnnotationMode annotationMode = this.mainViewModel.AnnotationMode;
					annotationHolderManager.CancelAll();
					this.mainViewModel.AnnotationMode = annotationMode;
				}
			}
			this.mainViewModel.RaiseAnnotationModePropertyChanged();
			PdfAnnotation selectedAnnotation = this.mainViewModel.SelectedAnnotation;
			AnnotationMode annotationMode2 = this.mainViewModel.AnnotationMode;
			if (selectedAnnotation != null)
			{
				if (annotationMode2 == AnnotationMode.None)
				{
					this.mainViewModel.ReleaseViewerFocusAsync(false);
					return;
				}
				if (annotationMode2 == AnnotationMode.Ink)
				{
					this.mainViewModel.ReleaseViewerFocusAsync(false);
					return;
				}
				if (annotationMode2 != AnnotationMode.None)
				{
					global::System.Collections.Generic.IReadOnlyList<AnnotationMode> annotationModes = AnnotationFactory.GetAnnotationModes(selectedAnnotation);
					if (annotationModes.Count == 0 || annotationModes[0] != annotationMode2)
					{
						this.mainViewModel.ReleaseViewerFocusAsync(true);
						this.NotifyCheckedButtonToolbarSettingChanged();
					}
				}
			}
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x000195C4 File Offset: 0x000177C4
		[CompilerGenerated]
		private void <InitToolbarAnnotationButtonModel>g__TextCommandFunc|214_1(ToolbarAnnotationButtonModel model)
		{
			foreach (ToolbarAnnotationButtonModel toolbarAnnotationButtonModel in this.allAnnotationButton)
			{
				if (toolbarAnnotationButtonModel != model && toolbarAnnotationButtonModel.IsChecked)
				{
					toolbarAnnotationButtonModel.IsChecked = false;
				}
			}
			this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0(model);
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x00019624 File Offset: 0x00017824
		[CompilerGenerated]
		private void <InitToolbarAnnotationButtonModel>g__NoteCommandFunc|214_2(ToolbarAnnotationButtonModel model)
		{
			foreach (ToolbarAnnotationButtonModel toolbarAnnotationButtonModel in this.allAnnotationButton)
			{
				if (toolbarAnnotationButtonModel != model && toolbarAnnotationButtonModel.IsChecked)
				{
					toolbarAnnotationButtonModel.IsChecked = false;
				}
			}
			this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0(model);
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x00019684 File Offset: 0x00017884
		[CompilerGenerated]
		private void <InitToolbarAnnotationButtonModel>g__OpenContextMenuCommandFunc|214_3(ToolbarAnnotationButtonModel model)
		{
			AnnotationToolbarViewModel.<>c__DisplayClass214_0 CS$<>8__locals1 = new AnnotationToolbarViewModel.<>c__DisplayClass214_0();
			CS$<>8__locals1.model = model;
			this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0(CS$<>8__locals1.model);
			DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Render, new Action(delegate
			{
				AnnotationToolbarViewModel.<>c__DisplayClass214_0.<<InitToolbarAnnotationButtonModel>b__19>d <<InitToolbarAnnotationButtonModel>b__19>d;
				<<InitToolbarAnnotationButtonModel>b__19>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<InitToolbarAnnotationButtonModel>b__19>d.<>4__this = CS$<>8__locals1;
				<<InitToolbarAnnotationButtonModel>b__19>d.<>1__state = -1;
				<<InitToolbarAnnotationButtonModel>b__19>d.<>t__builder.Start<AnnotationToolbarViewModel.<>c__DisplayClass214_0.<<InitToolbarAnnotationButtonModel>b__19>d>(ref <<InitToolbarAnnotationButtonModel>b__19>d);
			}));
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x000196C4 File Offset: 0x000178C4
		[CompilerGenerated]
		private async void <InitToolbarAnnotationButtonModel>g__TextMarkupCommandFunc|214_4(ToolbarAnnotationButtonModel model)
		{
			foreach (ToolbarAnnotationButtonModel toolbarAnnotationButtonModel in this.allAnnotationButton)
			{
				if (toolbarAnnotationButtonModel != model && toolbarAnnotationButtonModel.IsChecked)
				{
					toolbarAnnotationButtonModel.IsChecked = false;
				}
			}
			this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0(model);
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			PdfViewer viewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			if (viewer != null && this.mainViewModel.Document != null && this.mainViewModel.Document.Pages.CurrentPage.PageIndex != -1 && viewer.SelectInfo.StartIndex >= 0)
			{
				AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(viewer);
				if (annotationHolderManager != null)
				{
					global::System.Collections.Generic.IReadOnlyList<PdfAnnotation> annots = null;
					switch (model.Mode)
					{
					case AnnotationMode.Highlight:
						annots = annotationHolderManager.Highlight.CreateAnnotation(this.mainViewModel.Document, viewer.SelectInfo);
						break;
					case AnnotationMode.Underline:
						annots = annotationHolderManager.Underline.CreateAnnotation(this.mainViewModel.Document, viewer.SelectInfo);
						break;
					case AnnotationMode.Strike:
						annots = annotationHolderManager.Strikeout.CreateAnnotation(this.mainViewModel.Document, viewer.SelectInfo);
						break;
					}
					if (annots != null && annots.Count > 0)
					{
						await this.mainViewModel.OperationManager.TraceAnnotationInsertAsync(annots, "");
						foreach (PdfPage curPage in annots.Select((PdfAnnotation c) => c.Page).Distinct<PdfPage>())
						{
							await curPage.TryRedrawPageAsync(default(CancellationToken));
							this.mainViewModel.PageEditors.NotifyPageAnnotationChanged(curPage.PageIndex);
							curPage = null;
						}
						IEnumerator<PdfPage> enumerator2 = null;
						viewer.DeselectText();
						this.mainViewModel.AnnotationMode = AnnotationMode.None;
						this.mainViewModel.SelectedAnnotation = annots[0];
					}
					annots = null;
				}
			}
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x00019703 File Offset: 0x00017903
		[CompilerGenerated]
		private void <InitToolbarAnnotationButtonModel>g__StampCommandFunc|214_5(ToolbarAnnotationButtonModel model)
		{
			this.<InitToolbarAnnotationButtonModel>g__OpenContextMenuCommandFunc|214_3(model);
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x0001970C File Offset: 0x0001790C
		[CompilerGenerated]
		private void <InitToolbarAnnotationButtonModel>g__InsertImageCommandFunc|214_6(ToolbarAnnotationButtonModel model)
		{
			GAManager.SendEvent("AnnotationAction", "InsertImageCommandFunc", "New", 1L);
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			this.DoStampAddImgCmd(null);
			try
			{
				if (this.mainViewModel.AnnotationMode != AnnotationMode.Link)
				{
					global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document).Viewer.IsLinkAnnotationHighlighted = false;
					ToolbarChildCheckableButtonModel toolbarChildCheckableButtonModel = this.mainViewModel.AnnotationToolbar.LinkButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel;
					if (toolbarChildCheckableButtonModel != null)
					{
						TypedContextMenuModel typedContextMenuModel = toolbarChildCheckableButtonModel.ContextMenu as TypedContextMenuModel;
						if (typedContextMenuModel != null)
						{
							ContextMenuItemModel contextMenuItemModel = typedContextMenuModel[0] as ContextMenuItemModel;
							if (contextMenuItemModel != null)
							{
								contextMenuItemModel.Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/LinkCE.png"));
								this.mainViewModel.AnnotationToolbar.LinkButtonModel.IsChecked = false;
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x000197F0 File Offset: 0x000179F0
		[CompilerGenerated]
		private void <InitToolbarAnnotationButtonModel>g__SignatureCommandFunc|214_7(ToolbarAnnotationButtonModel model)
		{
			AnnotationToolbarViewModel.<>c__DisplayClass214_1 CS$<>8__locals1 = new AnnotationToolbarViewModel.<>c__DisplayClass214_1();
			CS$<>8__locals1.model = model;
			this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0(CS$<>8__locals1.model);
			DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				AnnotationToolbarViewModel.<>c__DisplayClass214_1.<<InitToolbarAnnotationButtonModel>b__21>d <<InitToolbarAnnotationButtonModel>b__21>d;
				<<InitToolbarAnnotationButtonModel>b__21>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<InitToolbarAnnotationButtonModel>b__21>d.<>4__this = CS$<>8__locals1;
				<<InitToolbarAnnotationButtonModel>b__21>d.<>1__state = -1;
				<<InitToolbarAnnotationButtonModel>b__21>d.<>t__builder.Start<AnnotationToolbarViewModel.<>c__DisplayClass214_1.<<InitToolbarAnnotationButtonModel>b__21>d>(ref <<InitToolbarAnnotationButtonModel>b__21>d);
			}));
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x00019830 File Offset: 0x00017A30
		[CompilerGenerated]
		private void <InitToolbarAnnotationButtonModel>g__LinkCommandFunc|214_8(ToolbarAnnotationButtonModel model)
		{
			this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0(model);
			ToolbarChildCheckableButtonModel toolbarChildCheckableButtonModel = model.ChildButtonModel as ToolbarChildCheckableButtonModel;
			if (toolbarChildCheckableButtonModel != null)
			{
				TypedContextMenuModel typedContextMenuModel = toolbarChildCheckableButtonModel.ContextMenu as TypedContextMenuModel;
				if (typedContextMenuModel != null)
				{
					this.LinkCmd(typedContextMenuModel[0] as ContextMenuItemModel, false);
				}
			}
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x00019878 File Offset: 0x00017A78
		[CompilerGenerated]
		private void <InitToolbarAnnotationButtonModel>g__InkCommandFunc|214_9(ToolbarAnnotationButtonModel model)
		{
			this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0(model);
			ToolbarSettingInkEraserModel toolbarSettingInkEraserModel = model.ToolbarSettingModel[3] as ToolbarSettingInkEraserModel;
			if (toolbarSettingInkEraserModel != null && toolbarSettingInkEraserModel.IsChecked)
			{
				toolbarSettingInkEraserModel.IsChecked = false;
			}
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x000198B0 File Offset: 0x00017AB0
		[CompilerGenerated]
		private void <InitToolbarAnnotationButtonModel>g__WatermarkCommandFunc|214_10(ToolbarAnnotationButtonModel model)
		{
			AnnotationToolbarViewModel.<>c__DisplayClass214_2 CS$<>8__locals1 = new AnnotationToolbarViewModel.<>c__DisplayClass214_2();
			CS$<>8__locals1.model = model;
			this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0(CS$<>8__locals1.model);
			DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				AnnotationToolbarViewModel.<>c__DisplayClass214_2.<<InitToolbarAnnotationButtonModel>b__22>d <<InitToolbarAnnotationButtonModel>b__22>d;
				<<InitToolbarAnnotationButtonModel>b__22>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<InitToolbarAnnotationButtonModel>b__22>d.<>4__this = CS$<>8__locals1;
				<<InitToolbarAnnotationButtonModel>b__22>d.<>1__state = -1;
				<<InitToolbarAnnotationButtonModel>b__22>d.<>t__builder.Start<AnnotationToolbarViewModel.<>c__DisplayClass214_2.<<InitToolbarAnnotationButtonModel>b__22>d>(ref <<InitToolbarAnnotationButtonModel>b__22>d);
			}));
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x000198F0 File Offset: 0x00017AF0
		[CompilerGenerated]
		private void <InitToolbarAnnotationButtonModel>g__AttachmentCommandFunc|214_11(ToolbarAnnotationButtonModel model)
		{
			AnnotationToolbarViewModel.<>c__DisplayClass214_3 CS$<>8__locals1 = new AnnotationToolbarViewModel.<>c__DisplayClass214_3();
			CS$<>8__locals1.model = model;
			this.<InitToolbarAnnotationButtonModel>g__CommandFunc|214_0(CS$<>8__locals1.model);
			DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				AnnotationToolbarViewModel.<>c__DisplayClass214_3.<<InitToolbarAnnotationButtonModel>b__23>d <<InitToolbarAnnotationButtonModel>b__23>d;
				<<InitToolbarAnnotationButtonModel>b__23>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<InitToolbarAnnotationButtonModel>b__23>d.<>4__this = CS$<>8__locals1;
				<<InitToolbarAnnotationButtonModel>b__23>d.<>1__state = -1;
				<<InitToolbarAnnotationButtonModel>b__23>d.<>t__builder.Start<AnnotationToolbarViewModel.<>c__DisplayClass214_3.<<InitToolbarAnnotationButtonModel>b__23>d>(ref <<InitToolbarAnnotationButtonModel>b__23>d);
			}));
		}

		// Token: 0x04000257 RID: 599
		private static readonly Thickness DefaultDigitalSignatureThickness = new Thickness(4.0);

		// Token: 0x04000258 RID: 600
		private readonly MainViewModel mainViewModel;

		// Token: 0x04000259 RID: 601
		private TypedContextMenuModel stampMenuItems;

		// Token: 0x0400025A RID: 602
		private TypedContextMenuModel stampMenuItems2;

		// Token: 0x0400025B RID: 603
		private TypedContextMenuModel shareMenuItem;

		// Token: 0x0400025C RID: 604
		private TypedContextMenuModel signatureMenuItems;

		// Token: 0x0400025D RID: 605
		private TypedContextMenuModel signatureMenuItems2;

		// Token: 0x0400025E RID: 606
		private TypedContextMenuModel signatureMenuItems3;

		// Token: 0x0400025F RID: 607
		private TypedContextMenuModel waterMenuItems;

		// Token: 0x04000260 RID: 608
		private TypedContextMenuModel linkMenuItems;

		// Token: 0x04000261 RID: 609
		private TypedContextMenuModel certManagerItems;

		// Token: 0x04000262 RID: 610
		private TypedContextMenuModel translateMenuItems;

		// Token: 0x04000263 RID: 611
		private TypedContextMenuModel attachmentMenuItems;

		// Token: 0x04000264 RID: 612
		private ToolbarAnnotationButtonModel underlineButtonModel;

		// Token: 0x04000265 RID: 613
		private ToolbarAnnotationButtonModel strikeButtonModel;

		// Token: 0x04000266 RID: 614
		private ToolbarAnnotationButtonModel highlightButtonModel;

		// Token: 0x04000267 RID: 615
		private ToolbarAnnotationButtonModel lineButtonModel;

		// Token: 0x04000268 RID: 616
		private ToolbarAnnotationButtonModel inkButtonModel;

		// Token: 0x04000269 RID: 617
		private ToolbarAnnotationButtonModel squareButtonModel;

		// Token: 0x0400026A RID: 618
		private ToolbarAnnotationButtonModel circleButtonModel;

		// Token: 0x0400026B RID: 619
		private ToolbarAnnotationButtonModel highlightareaButtonModel;

		// Token: 0x0400026C RID: 620
		private ToolbarAnnotationButtonModel textBoxButtonModel;

		// Token: 0x0400026D RID: 621
		private ToolbarAnnotationButtonModel textButtonModel;

		// Token: 0x0400026E RID: 622
		private ToolbarAnnotationButtonModel noteButtonModel;

		// Token: 0x0400026F RID: 623
		private ToolbarAnnotationButtonModel shareButtonModel;

		// Token: 0x04000270 RID: 624
		private ToolbarAnnotationButtonModel stampButtonModel;

		// Token: 0x04000271 RID: 625
		private ToolbarAnnotationButtonModel imageButtonModel;

		// Token: 0x04000272 RID: 626
		private ToolbarAnnotationButtonModel signatureButtonModel;

		// Token: 0x04000273 RID: 627
		private ToolbarAnnotationButtonModel watermarkButtonModel;

		// Token: 0x04000274 RID: 628
		private ToolbarAnnotationButtonModel linkButtonModel;

		// Token: 0x04000275 RID: 629
		private ToolbarAnnotationButtonModel translateButtonModel;

		// Token: 0x04000276 RID: 630
		private ToolbarAnnotationButtonModel attachmentButtonModel;

		// Token: 0x04000277 RID: 631
		private ToolbarAnnotationButtonModel digitalSignatureButtonModel;

		// Token: 0x04000278 RID: 632
		private ToolbarAnnotationButtonModel addDeferredDigitalSignatureButtonModel;

		// Token: 0x04000279 RID: 633
		private ToolbarAnnotationButtonModel certificateManagerButtonModel;

		// Token: 0x0400027A RID: 634
		private ToolbarAnnotationButtonModel redactionButtonModel;

		// Token: 0x0400027B RID: 635
		private AnnotationMenuPropertyAccessor annotationMenuPropertyAccessor;

		// Token: 0x0400027C RID: 636
		private global::System.Collections.Generic.IReadOnlyList<ToolbarAnnotationButtonModel> allAnnotationButton;

		// Token: 0x0400027D RID: 637
		private WatermarkAnnonationModel watermarkModel;

		// Token: 0x0400027E RID: 638
		private WatermarkParam watermarkParam;

		// Token: 0x0400027F RID: 639
		private WatermarkImageModel imageWatermarkModel;

		// Token: 0x04000280 RID: 640
		private WatermarkTextModel textWatermarkModel;

		// Token: 0x04000281 RID: 641
		private AsyncRelayCommand addFormControlCheckCmd;

		// Token: 0x04000282 RID: 642
		private AsyncRelayCommand addFormControlCancelCmd;

		// Token: 0x04000283 RID: 643
		private AsyncRelayCommand addFormControlRadioCheckCmd;

		// Token: 0x04000284 RID: 644
		private AsyncRelayCommand addFormControlCheckBoxCmd;

		// Token: 0x04000285 RID: 645
		private AsyncRelayCommand addFormControlIndeterminateCmd;

		// Token: 0x04000286 RID: 646
		private AsyncRelayCommand addFormControlIndeterminateFillCmd;

		// Token: 0x04000287 RID: 647
		public static int DocSignatures = 0;
	}
}
