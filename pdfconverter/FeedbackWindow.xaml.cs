using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using CommonLib.Common;
using CommonLib.Controls;
using Ionic.Zip;
using pdfconverter.Properties;

namespace pdfconverter
{
	// Token: 0x02000019 RID: 25
	public partial class FeedbackWindow : Window
	{
		// Token: 0x060000B2 RID: 178 RVA: 0x00003011 File Offset: 0x00001211
		public FeedbackWindow()
		{
			this.InitializeComponent();
			GAManager.SendEvent("ConvertFeedback", "Show", "Count", 1L);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000304C File Offset: 0x0000124C
		public void showAttachmentCB(bool isShowing)
		{
			if (isShowing)
			{
				string text = "";
				foreach (string text2 in this.flist)
				{
					string fileName = Path.GetFileName(text2);
					if (text.Length > 0)
					{
						text = text + ", " + fileName;
					}
					else
					{
						text = fileName;
					}
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					this.filesTB.Text = text;
					this.sendSampleGrid.Visibility = Visibility.Visible;
					this.sendSampleCB.IsChecked = new bool?(false);
					return;
				}
			}
			this.sendSampleGrid.Visibility = Visibility.Collapsed;
			this.sendSampleCB.IsChecked = new bool?(false);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00003114 File Offset: 0x00001314
		private string CreateZipFile()
		{
			if (this.flist == null || this.flist.Count<string>() <= 0)
			{
				return "";
			}
			string localCacheFolder = AppDataHelper.LocalCacheFolder;
			string text = Path.Combine(localCacheFolder, "files");
			try
			{
				if (Directory.Exists(text))
				{
					Directory.Delete(text, true);
				}
			}
			catch
			{
			}
			Directory.CreateDirectory(text);
			if (!Directory.Exists(text))
			{
				return "";
			}
			foreach (string text2 in this.flist)
			{
				LongPathFile longPathFile = text2;
				if (longPathFile.IsExists)
				{
					FileInfo fileInfo = longPathFile.FileInfo;
					if (fileInfo.Length <= 15728640L)
					{
						string fileName = Path.GetFileName(longPathFile);
						string text3 = Path.Combine(text, fileName);
						if (!File.Exists(text3))
						{
							fileInfo.CopyTo(text3, false);
						}
					}
				}
			}
			string text4 = "";
			using (ZipFile zipFile = new ZipFile())
			{
				text4 = Path.Combine(localCacheFolder, "debug.zip");
				if (File.Exists(text4))
				{
					File.Delete(text4);
				}
				zipFile.Encryption = EncryptionAlgorithm.WinZipAes256;
				zipFile.Password = "pdfis~pdf";
				zipFile.AddDirectory(text);
				zipFile.Save(text4);
			}
			if (text4.Length > 0 && File.Exists(text4) && new FileInfo(text4).Length < 15728640L)
			{
				return text4;
			}
			return "";
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000032A8 File Offset: 0x000014A8
		private void TxtContent_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.txtContent.Text.Trim().Length > 0)
			{
				this.sendBtn.IsEnabled = true;
				return;
			}
			this.sendBtn.IsEnabled = false;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000032DB File Offset: 0x000014DB
		private void CloseBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ConvertFeedback", "CloseBtn", "Count", 1L);
			base.Close();
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000032FC File Offset: 0x000014FC
		private async void SendBtn_Click(object sender, RoutedEventArgs e)
		{
			bool sendOk = false;
			string strPrio = "3";
			string text = this.txtEmail.Text.Trim();
			if (text.Length > 0 && !this.IsEmailValid(text))
			{
				ModernMessageBox.Show(pdfconverter.Properties.Resources.FeedbackWindowInvalidEmailMsgContent, pdfconverter.Properties.Resources.FeedbackWindowInvalidEmailMsgTitle, MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else
			{
				string strSubject = string.Concat(new string[]
				{
					"[User Feedback][Win] ",
					UtilManager.GetProductName(),
					" (",
					UtilManager.GetAppVersion(),
					")"
				});
				string strDescription = this.txtContent.Text;
				string strEmail_r = ((this.txtEmail.Text.Trim().Length > 0) ? this.txtEmail.Text : "noreply@pdfgear.com");
				bool shoudSendAttachment = false;
				if (this.sendSampleCB.IsChecked.GetValueOrDefault())
				{
					shoudSendAttachment = true;
				}
				GAManager.SendEvent("ConvertFeedback", "ClickBtn", "Count", 1L);
				Action <>9__3;
				Action <>9__1;
				Action <>9__2;
				await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
				{
					try
					{
						Dispatcher dispatcher = this.Dispatcher;
						Action action;
						if ((action = <>9__3) == null)
						{
							action = (<>9__3 = delegate
							{
								this.updateSendStatus(true);
							});
						}
						dispatcher.Invoke(action);
						string text2 = "";
						if (shoudSendAttachment)
						{
							try
							{
								text2 = this.CreateZipFile();
							}
							catch
							{
							}
						}
						string text3 = "----------------------------" + DateTime.Now.Ticks.ToString("x");
						HttpWebRequest httpWebRequest = RequestHelper.CreateRequest("feedback", text3);
						using (Stream requestStream = httpWebRequest.GetRequestStream())
						{
							RequestHelper.WriteBoundaryBytes(requestStream, text3, false);
							RequestHelper.WriteContentDispositionFormDataHeader(requestStream, "email");
							RequestHelper.WriteString(requestStream, strEmail_r);
							RequestHelper.WriteCRLF(requestStream);
							RequestHelper.WriteBoundaryBytes(requestStream, text3, false);
							RequestHelper.WriteContentDispositionFormDataHeader(requestStream, "subject");
							RequestHelper.WriteString(requestStream, strSubject);
							RequestHelper.WriteCRLF(requestStream);
							RequestHelper.WriteBoundaryBytes(requestStream, text3, false);
							RequestHelper.WriteContentDispositionFormDataHeader(requestStream, "description");
							RequestHelper.WriteString(requestStream, strDescription);
							RequestHelper.WriteCRLF(requestStream);
							RequestHelper.WriteBoundaryBytes(requestStream, text3, false);
							RequestHelper.WriteContentDispositionFormDataHeader(requestStream, "status");
							RequestHelper.WriteString(requestStream, "2");
							RequestHelper.WriteCRLF(requestStream);
							RequestHelper.WriteBoundaryBytes(requestStream, text3, false);
							RequestHelper.WriteContentDispositionFormDataHeader(requestStream, "priority");
							RequestHelper.WriteString(requestStream, strPrio);
							RequestHelper.WriteCRLF(requestStream);
							RequestHelper.WriteBoundaryBytes(requestStream, text3, false);
							RequestHelper.WriteContentDispositionFormDataHeader(requestStream, "source");
							RequestHelper.WriteString(requestStream, "1");
							RequestHelper.WriteCRLF(requestStream);
							RequestHelper.WriteBoundaryBytes(requestStream, text3, false);
							RequestHelper.WriteContentDispositionFormDataHeader(requestStream, "app_id");
							RequestHelper.WriteString(requestStream, UtilManager.GetProductID());
							RequestHelper.WriteCRLF(requestStream);
							if (shoudSendAttachment)
							{
								GAManager.SendEvent("FeedbackWin", "SendAttach", "Click", 1L);
								string text4 = "log.zip";
								RequestHelper.WriteBoundaryBytes(requestStream, text3, false);
								RequestHelper.WriteContentDispositionFileHeader(requestStream, "attachment", text4, "text/plain");
								FileStream fileStream = new FileStream(text2, FileMode.Open, FileAccess.Read);
								byte[] array = new byte[fileStream.Length];
								fileStream.Read(array, 0, array.Length);
								fileStream.Close();
								requestStream.Write(array, 0, array.Length);
								RequestHelper.WriteCRLF(requestStream);
							}
							RequestHelper.WriteBoundaryBytes(requestStream, text3, true);
							requestStream.Close();
							try
							{
								HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
								Console.WriteLine("Status Code: {1} {0}", httpWebResponse.StatusCode, (int)httpWebResponse.StatusCode);
								if (httpWebResponse.StatusCode == HttpStatusCode.OK)
								{
									sendOk = true;
								}
							}
							catch (WebException)
							{
								Console.Write("Error WebException.");
							}
							catch (Exception)
							{
								Console.WriteLine("ERROR Exception.");
							}
						}
					}
					catch (Exception)
					{
					}
					finally
					{
						Dispatcher dispatcher2 = this.Dispatcher;
						Action action2;
						if ((action2 = <>9__1) == null)
						{
							action2 = (<>9__1 = delegate
							{
								this.updateSendStatus(false);
							});
						}
						dispatcher2.Invoke(action2);
						if (sendOk)
						{
							MessageBox.Show(pdfconverter.Properties.Resources.FeedbackWindowSendSuccMsgContent, pdfconverter.Properties.Resources.FeedbackWindowSendSuccMsgTitle, MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
							Dispatcher dispatcher3 = this.Dispatcher;
							Action action3;
							if ((action3 = <>9__2) == null)
							{
								action3 = (<>9__2 = delegate
								{
									this.Close();
								});
							}
							dispatcher3.Invoke(action3);
						}
						else
						{
							MessageBox.Show(pdfconverter.Properties.Resources.WinFeedBackSendTryagainMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
						}
					}
				})).ConfigureAwait(false);
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00003333 File Offset: 0x00001533
		private bool IsEmailValid(string strEmail)
		{
			return Regex.Match(strEmail, "[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2}").Success;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00003348 File Offset: 0x00001548
		private void updateSendStatus(bool showSending)
		{
			if (showSending)
			{
				this.txtEmail.IsEnabled = false;
				this.txtContent.IsEnabled = false;
				this.sendBtn.IsEnabled = false;
				this.sendingProgessBar.IsActive = true;
				return;
			}
			this.txtEmail.IsEnabled = true;
			this.txtContent.IsEnabled = true;
			this.sendBtn.IsEnabled = true;
			this.sendingProgessBar.IsActive = false;
		}

		// Token: 0x040000EA RID: 234
		public string source = "";

		// Token: 0x040000EB RID: 235
		public List<string> flist = new List<string>();
	}
}
