using System;
using System.Linq;
using System.Windows;
using CommonLib.Common;
using pdfeditor.Controls;
using pdfeditor.Views;

namespace pdfeditor.Utils
{
	// Token: 0x020000A2 RID: 162
	internal static class SetDefaultAppUtils
	{
		// Token: 0x06000A21 RID: 2593 RVA: 0x000336E4 File Offset: 0x000318E4
		public static void TrySetDefaultApp()
		{
			if (!AppManager.GetDefaultFileExts().All((string c) => AppIdHelper.GetDefaultAppProgId(c) == "PdfGear.App.1"))
			{
				string text = ConfigManager.GetDefaultAppAction();
				bool flag = false;
				if (!text.StartsWith("Silence_"))
				{
					SetDefaultAppDialog setDefaultAppDialog = new SetDefaultAppDialog();
					setDefaultAppDialog.Owner = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
					setDefaultAppDialog.WindowStartupLocation = ((setDefaultAppDialog.Owner == null) ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner);
					setDefaultAppDialog.ShowDialog();
					text = setDefaultAppDialog.Action;
					ConfigManager.SetDefaultAppAction(text);
					flag = true;
				}
				GAManager.SendEvent("ExtDefaultApp", text, "Count", 1L);
				if (!(text == "Silence_SetDefault"))
				{
					if (!(text == "SetDefault"))
					{
						if (!(text == "Silence_Exit"))
						{
							text == "Exit";
							return;
						}
					}
					else
					{
						AppManager.RegisterFileAssociations(true);
					}
				}
				else if (flag || !AppIdHelper.HasUserChoiceLatest)
				{
					AppManager.RegisterFileAssociations(true);
					return;
				}
			}
		}
	}
}
