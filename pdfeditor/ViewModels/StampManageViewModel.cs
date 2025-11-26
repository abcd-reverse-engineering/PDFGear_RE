using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using pdfeditor.Controls.Annotations;
using pdfeditor.Controls.Stamp;
using pdfeditor.Models.Stamp;
using pdfeditor.Models.Viewer;
using pdfeditor.Properties;
using pdfeditor.Utils;

namespace pdfeditor.ViewModels
{
	// Token: 0x0200006C RID: 108
	public class StampManageViewModel : ObservableObject
	{
		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x060007CA RID: 1994 RVA: 0x000256E3 File Offset: 0x000238E3
		// (set) Token: 0x060007CB RID: 1995 RVA: 0x000256EB File Offset: 0x000238EB
		public ObservableCollection<StampGroupModel> Groups
		{
			get
			{
				return this.groups;
			}
			set
			{
				base.SetProperty<ObservableCollection<StampGroupModel>>(ref this.groups, value, "Groups");
			}
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x060007CC RID: 1996 RVA: 0x00025700 File Offset: 0x00023900
		public ICommand SelectGroupCommand { get; }

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x060007CD RID: 1997 RVA: 0x00025708 File Offset: 0x00023908
		// (set) Token: 0x060007CE RID: 1998 RVA: 0x00025710 File Offset: 0x00023910
		public int AllCategoriesCount
		{
			get
			{
				return this._allCategoriesCount;
			}
			set
			{
				base.SetProperty<int>(ref this._allCategoriesCount, value, "AllCategoriesCount");
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x060007CF RID: 1999 RVA: 0x00025725 File Offset: 0x00023925
		// (set) Token: 0x060007D0 RID: 2000 RVA: 0x0002572D File Offset: 0x0002392D
		public StampGroupModel SelectedGroup
		{
			get
			{
				return this._selectedGroup;
			}
			set
			{
				if (this._selectedGroup != null)
				{
					this._selectedGroup.IsSelected = false;
				}
				base.SetProperty<StampGroupModel>(ref this._selectedGroup, value, "SelectedGroup");
				if (this._selectedGroup != null)
				{
					this._selectedGroup.IsSelected = true;
				}
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x060007D1 RID: 2001 RVA: 0x0002576A File Offset: 0x0002396A
		// (set) Token: 0x060007D2 RID: 2002 RVA: 0x00025772 File Offset: 0x00023972
		public StampGroupModel AllSelectedGroup
		{
			get
			{
				return this._allSelectedGroup;
			}
			set
			{
				if (this._allSelectedGroup != null)
				{
					this._allSelectedGroup.IsSelected = false;
				}
				base.SetProperty<StampGroupModel>(ref this._allSelectedGroup, value, "AllSelectedGroup");
				if (this._allSelectedGroup != null)
				{
					this._allSelectedGroup.IsSelected = true;
				}
			}
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x060007D3 RID: 2003 RVA: 0x000257AF File Offset: 0x000239AF
		public ICommand DeleteStampCommand { get; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x060007D4 RID: 2004 RVA: 0x000257B7 File Offset: 0x000239B7
		public ICommand EditStampCommand { get; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x060007D5 RID: 2005 RVA: 0x000257BF File Offset: 0x000239BF
		public ICommand CreateCommand { get; }

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x060007D6 RID: 2006 RVA: 0x000257C7 File Offset: 0x000239C7
		public ICommand DeleteSelectedItemsCommand { get; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x060007D7 RID: 2007 RVA: 0x000257CF File Offset: 0x000239CF
		public ICommand AddStampCommand { get; }

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x060007D8 RID: 2008 RVA: 0x000257D7 File Offset: 0x000239D7
		public ICommand EditCategoryCommand { get; }

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x060007D9 RID: 2009 RVA: 0x000257DF File Offset: 0x000239DF
		public ICommand EditCurrentCategeorCommand { get; }

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x060007DA RID: 2010 RVA: 0x000257E7 File Offset: 0x000239E7
		public ICommand DeleteCategoryCommand { get; }

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x060007DB RID: 2011 RVA: 0x000257EF File Offset: 0x000239EF
		public ICommand DeleteCurrentCategoryCommand { get; }

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x060007DC RID: 2012 RVA: 0x000257F7 File Offset: 0x000239F7
		public ICommand AddNewCategoryCommand { get; }

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x060007DD RID: 2013 RVA: 0x000257FF File Offset: 0x000239FF
		public ICommand SelectedAllCategory { get; }

		// Token: 0x060007DE RID: 2014 RVA: 0x00025808 File Offset: 0x00023A08
		private void AddStampToGroup(StampGroupModel group)
		{
			if (group != null)
			{
				GAManager.SendEvent("CustomStampManageWindow", "AddStampToGroup", "Count", 1L);
				EditStampWindow editStampWindow = new EditStampWindow(group.Name);
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				editStampWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				editStampWindow.Owner = Application.Current.Windows.OfType<StampManageWindow>().FirstOrDefault<StampManageWindow>();
				if (!editStampWindow.ShowDialog().GetValueOrDefault())
				{
					DataOperationModel viewerOperationModel = requiredService.ViewerOperationModel;
					if (viewerOperationModel != null)
					{
						viewerOperationModel.Dispose();
					}
					requiredService.AnnotationMode = AnnotationMode.None;
					return;
				}
				if (requiredService.Document != null && editStampWindow.isSave)
				{
					requiredService.AnnotationToolbar.SaveStamp(editStampWindow.StampTextModel);
					this.ReadStampList(group.Name);
				}
			}
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x000258BF File Offset: 0x00023ABF
		private void EditSelectedCategoryName()
		{
			if (this.SelectedGroup != null)
			{
				this.EditCategoryName(this.SelectedGroup);
			}
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x000258D8 File Offset: 0x00023AD8
		private void EditCategoryName(StampGroupModel group)
		{
			if (group != null)
			{
				GAManager.SendEvent("CustomStampManageWindow", "EditCategoryName", "Count", 1L);
				ChangeCategoryNameWindow changeCategoryNameWindow = new ChangeCategoryNameWindow(group.Name);
				changeCategoryNameWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				changeCategoryNameWindow.Owner = Application.Current.Windows.OfType<StampManageWindow>().FirstOrDefault<StampManageWindow>();
				if (changeCategoryNameWindow.ShowDialog().GetValueOrDefault())
				{
					Ioc.Default.GetRequiredService<MainViewModel>().AnnotationToolbar.ChageCategroyName(group.Name, changeCategoryNameWindow.NewCategoryName);
					string stampCategory = ConfigManager.GetStampCategory();
					if (!string.IsNullOrEmpty(stampCategory))
					{
						List<string> list = JsonConvert.DeserializeObject<List<string>>(stampCategory);
						list.RemoveAll((string x) => x == group.Name);
						list.Add(changeCategoryNameWindow.NewCategoryName);
						string text = JsonConvert.SerializeObject(list);
						if (!string.IsNullOrEmpty(text))
						{
							ConfigManager.SetStampCategory(text);
						}
					}
					this.ReadStampList(changeCategoryNameWindow.NewCategoryName);
				}
			}
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x000259D0 File Offset: 0x00023BD0
		private void DeleteCurrentCategory()
		{
			if (this.SelectedGroup != null)
			{
				this.DeleteCategory(this.SelectedGroup);
			}
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x000259E8 File Offset: 0x00023BE8
		private void DeleteCategory(StampGroupModel group)
		{
			GAManager.SendEvent("CustomStampManageWindow", "DeleteCategory", "Count", 1L);
			if (ModernMessageBox.Show(Resources.StampManageDeleteCategoryMessage, "PDFgear", MessageBoxButton.YesNo, MessageBoxResult.None, null, false) != MessageBoxResult.Yes)
			{
				return;
			}
			for (int i = group.Items.Count - 1; i >= 0; i--)
			{
				this.DeleteStamp(group.Items[i]);
			}
			string stampCategory = ConfigManager.GetStampCategory();
			if (!string.IsNullOrEmpty(stampCategory))
			{
				List<string> list = JsonConvert.DeserializeObject<List<string>>(stampCategory);
				list.RemoveAll((string x) => x == group.Name);
				string text = JsonConvert.SerializeObject(list);
				if (!string.IsNullOrEmpty(text))
				{
					ConfigManager.SetStampCategory(text);
				}
			}
			if (string.IsNullOrEmpty(group.Name) || !(this.SelectedGroup.Name == group.Name))
			{
				this.ReadStampList(null);
				return;
			}
			int num = this.Groups.IndexOf(group);
			if (num == this.Groups.Count - 1 && this.Groups.Count >= 2)
			{
				this.ReadStampList(this.Groups[num - 1].Name);
				return;
			}
			if (num < this.Groups.Count - 1)
			{
				this.ReadStampList(this.Groups[num + 1].Name);
				return;
			}
			this.ReadStampList(null);
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x00025B5C File Offset: 0x00023D5C
		public void DeleteSelectedItems()
		{
			if (this.SelectedGroup.Items.Count <= 0)
			{
				return;
			}
			if (ModernMessageBox.Show(Resources.StampManageDeleteSelectedMessage, "PDFgear", MessageBoxButton.YesNo, MessageBoxResult.None, null, false) != MessageBoxResult.Yes)
			{
				return;
			}
			for (int i = this.SelectedGroup.Items.Count - 1; i >= 0; i--)
			{
				if (this.SelectedGroup.Items[i].IsChecked)
				{
					this.DeleteStamp(this.SelectedGroup.Items[i]);
				}
			}
			this.ReadStampList(this.SelectedGroup.Name);
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x00025BF4 File Offset: 0x00023DF4
		private void DeleteSelectedItems(GroupItem item)
		{
			if (item != null)
			{
				GAManager.SendEvent("CustomStampManageWindow", "DeleteStamp", "Count", 1L);
				if (ModernMessageBox.Show(Resources.StampManageDeleteMessage, "PDFgear", MessageBoxButton.YesNo, MessageBoxResult.None, null, false) != MessageBoxResult.Yes)
				{
					return;
				}
				this.DeleteStamp(item);
				this.ReadStampList(this.SelectedGroup.Name);
			}
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x00025C4C File Offset: 0x00023E4C
		public void CreateStamp()
		{
			if (this.SelectedGroup != null)
			{
				GAManager.SendEvent("CustomStampManageWindow", "NewStamp", "Count", 1L);
				EditStampWindow editStampWindow = new EditStampWindow(this.SelectedGroup.Name);
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				editStampWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				editStampWindow.Owner = Application.Current.Windows.OfType<StampManageWindow>().FirstOrDefault<StampManageWindow>();
				if (!editStampWindow.ShowDialog().GetValueOrDefault())
				{
					DataOperationModel viewerOperationModel = requiredService.ViewerOperationModel;
					if (viewerOperationModel != null)
					{
						viewerOperationModel.Dispose();
					}
					requiredService.AnnotationMode = AnnotationMode.None;
					return;
				}
				if (requiredService.Document != null && editStampWindow.isSave)
				{
					requiredService.AnnotationToolbar.SaveStamp(editStampWindow.StampTextModel);
					this.ReadStampList(editStampWindow.StampTextModel.GroupName);
				}
			}
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x00025D14 File Offset: 0x00023F14
		private void DeleteStamp(GroupItem item)
		{
			this.SelectedGroup.Items.Remove(item);
			int num = 0;
			for (int i = 0; i < this.Groups.Count; i++)
			{
				num += this.Groups[i].ItemCount;
			}
			this.AllCategoriesCount = num;
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			List<DynamicStampTextModel> list = ToolbarContextMenuHelper.ReadDynamicStamp(false);
			if (item.custStampModel.Image == "Visible")
			{
				if (item != null)
				{
					list.Remove(list.Find((DynamicStampTextModel x) => x.TemplateName == item.custStampModel.ImageFilePath));
				}
				string text = Path.Combine(AppDataHelper.LocalFolder, "Stamp");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				if (!Directory.GetFiles(text).ToList<string>().Contains(item.custStampModel.ImageFilePath))
				{
					goto IL_0141;
				}
				try
				{
					string imageFilePath = item.custStampModel.ImageFilePath;
					item = null;
					File.Delete(imageFilePath);
					Directory.GetFiles(text).ToList<string>();
					ConfigManager.RemoveSignatureRemoveBg(imageFilePath);
					goto IL_0141;
				}
				catch (Exception)
				{
					goto IL_0141;
				}
			}
			list.Remove(list.Find((DynamicStampTextModel x) => x.GroupId == item.custStampModel.GroupId));
			IL_0141:
			string text2 = Path.Combine(AppDataHelper.LocalCacheFolder, "Config");
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			string text3 = Path.Combine(text2, "Stamp.json");
			try
			{
				using (FileStream fileStream = new FileStream(text3, FileMode.Create, FileAccess.ReadWrite))
				{
					using (StreamWriter streamWriter = new StreamWriter(fileStream))
					{
						string text4 = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
						{
							TypeNameHandling = TypeNameHandling.Auto
						});
						streamWriter.Write(text4);
						streamWriter.Close();
					}
					fileStream.Close();
				}
				requiredService.AnnotationToolbar.ReflashStampList();
			}
			catch (Exception)
			{
			}
			base.OnPropertyChanged("DeleteStamp");
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x00025F40 File Offset: 0x00024140
		private void EditStamp(GroupItem item)
		{
			GAManager.SendEvent("CustomStampManageWindow", "EditStamp", "Count", 1L);
			EditStampWindow editStampWindow = new EditStampWindow(item.custStampModel);
			editStampWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			editStampWindow.Owner = Application.Current.Windows.OfType<StampManageWindow>().FirstOrDefault<StampManageWindow>();
			editStampWindow.ShowDialog();
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			if (requiredService.Document != null && editStampWindow.isSave)
			{
				ObservableCollection<StampGroupModel> observableCollection = this.Groups;
				if (observableCollection != null)
				{
					observableCollection.Clear();
				}
				List<DynamicStampTextModel> list = ToolbarContextMenuHelper.ReadDynamicStamp(false);
				if (item.custStampModel.Image == "Visible")
				{
					list.Remove(list.Find((DynamicStampTextModel x) => x.TemplateName == item.custStampModel.ImageFilePath));
					string text = Path.Combine(AppDataHelper.LocalFolder, "Stamp");
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					if (!Directory.GetFiles(text).ToList<string>().Contains(item.custStampModel.ImageFilePath) || !(item.custStampModel.ImageFilePath != editStampWindow.StampTextModel.TemplateName))
					{
						goto IL_017B;
					}
					try
					{
						string imageFilePath = item.custStampModel.ImageFilePath;
						File.Delete(imageFilePath);
						Directory.GetFiles(text).ToList<string>();
						ConfigManager.RemoveSignatureRemoveBg(imageFilePath);
						goto IL_017B;
					}
					catch (Exception)
					{
						goto IL_017B;
					}
				}
				list.Remove(list.Find((DynamicStampTextModel x) => x.GroupId == item.custStampModel.GroupId));
				IL_017B:
				string text2 = Path.Combine(AppDataHelper.LocalCacheFolder, "Config");
				if (!Directory.Exists(text2))
				{
					Directory.CreateDirectory(text2);
				}
				string text3 = Path.Combine(text2, "Stamp.json");
				try
				{
					using (FileStream fileStream = new FileStream(text3, FileMode.Create, FileAccess.ReadWrite))
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream))
						{
							string text4 = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
							{
								TypeNameHandling = TypeNameHandling.Auto
							});
							streamWriter.Write(text4);
							streamWriter.Close();
						}
						fileStream.Close();
					}
					requiredService.AnnotationToolbar.ReflashStampList();
				}
				catch (Exception)
				{
				}
				requiredService.AnnotationToolbar.SaveStamp(editStampWindow.StampTextModel);
				string name = this.SelectedGroup.Name;
				this.ReadStampList(editStampWindow.StampTextModel.GroupName);
				if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(editStampWindow.StampTextModel.GroupName))
				{
					this.SelectedAll();
				}
			}
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x000261E4 File Offset: 0x000243E4
		private void AddNewCategory()
		{
			if (this.Groups.Count >= 5)
			{
				ModernMessageBox.Show(Resources.StampLimitErrorMessage, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			GAManager.SendEvent("CustomStampManageWindow", "AddNewCategory", "Count", 1L);
			ChangeCategoryNameWindow changeCategoryNameWindow = new ChangeCategoryNameWindow(null);
			changeCategoryNameWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			changeCategoryNameWindow.Owner = Application.Current.Windows.OfType<StampManageWindow>().FirstOrDefault<StampManageWindow>();
			if (changeCategoryNameWindow.ShowDialog().GetValueOrDefault())
			{
				Ioc.Default.GetRequiredService<MainViewModel>();
				string stampCategory = ConfigManager.GetStampCategory();
				if (!string.IsNullOrEmpty(stampCategory))
				{
					List<string> list = JsonConvert.DeserializeObject<List<string>>(stampCategory);
					list.Add(changeCategoryNameWindow.NewCategoryName);
					string text = JsonConvert.SerializeObject(list);
					if (!string.IsNullOrEmpty(text))
					{
						ConfigManager.SetStampCategory(text);
					}
				}
				else
				{
					string text2 = JsonConvert.SerializeObject(new List<string> { changeCategoryNameWindow.NewCategoryName });
					if (!string.IsNullOrEmpty(text2))
					{
						ConfigManager.SetStampCategory(text2);
					}
				}
				this.ReadStampList(changeCategoryNameWindow.NewCategoryName);
			}
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x000262D8 File Offset: 0x000244D8
		private void ReadStampList(string categoryName = null)
		{
			this.stampModels = new ObservableCollection<CustStampModel>();
			string text = Path.Combine(AppDataHelper.LocalFolder, "Stamp");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			List<string> list = Directory.GetFiles(text).ToList<string>();
			List<DynamicStampTextModel> list2 = ToolbarContextMenuHelper.ReadDynamicStamp(false);
			ObservableCollection<CustStampModel> observableCollection = this.stampModels;
			if (observableCollection != null)
			{
				observableCollection.Clear();
			}
			List<CustStampModel> list3 = new List<CustStampModel>();
			foreach (string text2 in list)
			{
				FileInfo fileInfo = new FileInfo(text2);
				CustStampModel custStampModel = new CustStampModel
				{
					ImageFilePath = fileInfo.FullName,
					Image = "Visible",
					Text = "Collapsed",
					dateTime = fileInfo.CreationTime,
					DynamicStampTextModel = ((list2 != null) ? list2.FirstOrDefault((DynamicStampTextModel x) => x.TemplateName == fileInfo.FullName) : null)
				};
				try
				{
					ShellFile shellFile = ShellFile.FromFilePath(fileInfo.FullName);
					custStampModel.StampImageSource = shellFile.Thumbnail.LargeBitmapSource;
					list3.Add(custStampModel);
				}
				catch
				{
				}
			}
			if (list2 != null)
			{
				foreach (DynamicStampTextModel dynamicStampTextModel in list2)
				{
					if (dynamicStampTextModel.DynamicProperties != null || !string.IsNullOrEmpty(dynamicStampTextModel.TextContent))
					{
						CustStampModel custStampModel2 = new CustStampModel
						{
							TextContent = dynamicStampTextModel.TextContent,
							FontColor = dynamicStampTextModel.FontColor,
							GroupId = dynamicStampTextModel.GroupId,
							Image = "Collapsed",
							Text = "Visible",
							dateTime = dynamicStampTextModel.dateTime,
							DynamicStampTextModel = dynamicStampTextModel
						};
						list3.Add(custStampModel2);
					}
				}
			}
			foreach (CustStampModel custStampModel3 in list3.OrderByDescending((CustStampModel x) => x.dateTime).Take(50))
			{
				this.stampModels.Add(custStampModel3);
			}
			this.OrganizeIntoGroups(this.stampModels);
			this.ReflashSelectedAll();
			if (!string.IsNullOrEmpty(categoryName))
			{
				this.SelectedGroup = this.groups.FirstOrDefault((StampGroupModel x) => x.Name == categoryName);
			}
			else
			{
				this.SelectedAll();
			}
			if (this.SelectedGroup != null)
			{
				this.SelectedGroup.AllItemSelected = new bool?(false);
			}
			else
			{
				this.SelectGroup(this.AllSelectedGroup);
			}
			int num = 0;
			for (int i = 0; i < this.Groups.Count; i++)
			{
				num += this.Groups[i].ItemCount;
			}
			this.AllCategoriesCount = num;
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x0002660C File Offset: 0x0002480C
		public StampManageViewModel()
		{
			this.ReadStampList(null);
			this.SelectGroupCommand = new RelayCommand<StampGroupModel>(new Action<StampGroupModel>(this.SelectGroup));
			this.DeleteStampCommand = new RelayCommand<GroupItem>(new Action<GroupItem>(this.DeleteSelectedItems));
			this.EditStampCommand = new RelayCommand<GroupItem>(new Action<GroupItem>(this.EditStamp));
			this.CreateCommand = new RelayCommand(new Action(this.CreateStamp));
			this.DeleteSelectedItemsCommand = new RelayCommand(new Action(this.DeleteSelectedItems));
			this.AddStampCommand = new RelayCommand<StampGroupModel>(new Action<StampGroupModel>(this.AddStampToGroup));
			this.EditCategoryCommand = new RelayCommand<StampGroupModel>(new Action<StampGroupModel>(this.EditCategoryName));
			this.DeleteCategoryCommand = new RelayCommand<StampGroupModel>(new Action<StampGroupModel>(this.DeleteCategory));
			this.EditCurrentCategeorCommand = new RelayCommand(new Action(this.EditSelectedCategoryName));
			this.DeleteCurrentCategoryCommand = new RelayCommand(new Action(this.DeleteCurrentCategory));
			this.AddNewCategoryCommand = new RelayCommand(new Action(this.AddNewCategory));
			this.SelectedAllCategory = new RelayCommand(new Action(this.SelectedAll));
			this.SelectedAll();
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x0002674C File Offset: 0x0002494C
		private void ReflashSelectedAll()
		{
			this.AllSelectedGroup = new StampGroupModel();
			ObservableCollection<StampGroupModel> observableCollection = this.Groups;
			if (observableCollection != null && observableCollection.Count >= 0)
			{
				foreach (StampGroupModel stampGroupModel in this.Groups)
				{
					foreach (GroupItem groupItem in stampGroupModel.Items)
					{
						this.AllSelectedGroup.Items.Add(groupItem);
					}
				}
			}
			this.AllSelectedGroup.IsSelected = false;
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x00026808 File Offset: 0x00024A08
		private void SelectedAll()
		{
			this.ReflashSelectedAll();
			this.SelectGroup(this.AllSelectedGroup);
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x0002681C File Offset: 0x00024A1C
		private void SelectGroup(StampGroupModel group)
		{
			foreach (StampGroupModel stampGroupModel in this.Groups)
			{
				stampGroupModel.IsSelected = false;
			}
			if (group != null)
			{
				this.SelectedGroup = group;
			}
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x00026874 File Offset: 0x00024A74
		private void OrganizeIntoGroups(ObservableCollection<CustStampModel> stampModels)
		{
			this.Groups.Clear();
			Dictionary<string, StampGroupModel> dictionary = new Dictionary<string, StampGroupModel>();
			string stampCategory = ConfigManager.GetStampCategory();
			if (!string.IsNullOrEmpty(stampCategory))
			{
				foreach (string text in JsonConvert.DeserializeObject<List<string>>(stampCategory))
				{
					if (!dictionary.ContainsKey(text))
					{
						dictionary[text] = new StampGroupModel
						{
							Name = text
						};
					}
				}
			}
			string editStampWinUntitledCategoryName = Resources.EditStampWinUntitledCategoryName;
			foreach (CustStampModel custStampModel in stampModels)
			{
				DynamicStampTextModel dynamicStampTextModel = custStampModel.DynamicStampTextModel;
				string text2 = ((dynamicStampTextModel != null) ? dynamicStampTextModel.GroupName : null);
				if (string.IsNullOrWhiteSpace(text2))
				{
					text2 = editStampWinUntitledCategoryName;
				}
				if (!dictionary.ContainsKey(text2))
				{
					dictionary[text2] = new StampGroupModel
					{
						Name = text2
					};
				}
				GroupItem groupItem = new GroupItem();
				DynamicStampTextModel dynamicStampTextModel2 = custStampModel.DynamicStampTextModel;
				groupItem.Title = ((dynamicStampTextModel2 != null) ? dynamicStampTextModel2.TextContent : null) ?? "Untitled Stamp";
				groupItem.custStampModel = custStampModel;
				groupItem.IsChecked = false;
				GroupItem groupItem2 = groupItem;
				dictionary[text2].Items.Add(groupItem2);
			}
			foreach (StampGroupModel stampGroupModel in dictionary.Values)
			{
				this.Groups.Add(stampGroupModel);
			}
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x00026A18 File Offset: 0x00024C18
		public void DeleteGroupItem(GroupItem item)
		{
			StampGroupModel stampGroupModel = this.Groups.FirstOrDefault((StampGroupModel x) => x.IsSelected);
			if (stampGroupModel != null)
			{
				stampGroupModel.Items.Remove(stampGroupModel.Items.FirstOrDefault((GroupItem x) => x.custStampModel.GroupId == item.custStampModel.GroupId));
				if (stampGroupModel.IsSelected)
				{
					this.SelectGroup(stampGroupModel);
				}
			}
		}

		// Token: 0x040003FD RID: 1021
		private ObservableCollection<StampGroupModel> groups = new ObservableCollection<StampGroupModel>();

		// Token: 0x040003FE RID: 1022
		private StampGroupModel _allSelectedGroup;

		// Token: 0x040003FF RID: 1023
		private ObservableCollection<CustStampModel> stampModels;

		// Token: 0x04000401 RID: 1025
		private StampGroupModel _selectedGroup;

		// Token: 0x04000402 RID: 1026
		private int _allCategoriesCount;
	}
}
