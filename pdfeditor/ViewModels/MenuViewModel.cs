using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using pdfeditor.Models.Commets;
using pdfeditor.Models.LeftNavigations;
using pdfeditor.Models.Menus;
using pdfeditor.Properties;

namespace pdfeditor.ViewModels
{
	// Token: 0x02000069 RID: 105
	public class MenuViewModel : ObservableObject
	{
		// Token: 0x06000739 RID: 1849 RVA: 0x00021F0C File Offset: 0x0002010C
		public MenuViewModel(MainViewModel mainViewModel)
		{
			this.mainViewModel = mainViewModel;
			this.MainMenus = new ObservableCollection<MainMenuGroup>
			{
				new MainMenuGroup
				{
					Title = Resources.MenuGruopHomeTitle,
					Tag = "View"
				},
				new MainMenuGroup
				{
					Title = Resources.MenuGruopAnnotateTitle,
					Tag = "Annotate"
				},
				new MainMenuGroup
				{
					Title = Resources.WinScreenshotToolbarEditContent,
					Tag = "Insert"
				},
				new MainMenuGroup
				{
					Title = Resources.MenuGruopFillFormTitle,
					Tag = "FillForm"
				},
				new MainMenuGroup
				{
					Title = Resources.MenuGruopPageTitle,
					Tag = "Page"
				},
				new MainMenuGroup
				{
					Title = Resources.MenuGruopProtectTitle,
					Tag = "Protection"
				},
				new MainMenuGroup
				{
					Title = Resources.MenuGruopToolsTitle,
					Tag = "Tools"
				},
				new MainMenuGroup
				{
					Title = Resources.MenuGruopHelpTitle,
					Tag = "Help"
				}
			};
			this.LeftNavList = new ObservableCollection<NavigationModel>
			{
				new NavigationModel("Bookmark", Resources.LeftNavigationViewBookmarkDisplayName, ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/LeftNavIcon/Bookmark.png"), new Uri("pack://application:,,,/Style/DarkModeResources/LeftNavIcon/Bookmark.png")), ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/LeftNavIcon/Bookmark-Select.png"), new Uri("pack://application:,,,/Style/DarkModeResources/LeftNavIcon/Bookmark-Select.png"))),
				new NavigationModel("Thumbnail", Resources.LeftNavigationViewThumbnailDisplayName, ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/LeftNavIcon/Thumbnail.png"), new Uri("pack://application:,,,/Style/DarkModeResources/LeftNavIcon/Thumbnail.png")), ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/LeftNavIcon/Thumbnail-Select.png"), new Uri("pack://application:,,,/Style/DarkModeResources/LeftNavIcon/Thumbnail-Select.png"))),
				new NavigationModel("Annotation", Resources.LeftNavigationViewAnnotationDisplayName, ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/LeftNavIcon/Annotation.png"), new Uri("pack://application:,,,/Style/DarkModeResources/LeftNavIcon/Annotation.png")), ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/LeftNavIcon/Annotation-Select.png"), new Uri("pack://application:,,,/Style/DarkModeResources/LeftNavIcon/Annotation-Select.png"))),
				new NavigationModel("DigitalSignatures", Resources.LeftNavigationViewDigitalSignaturesDisplayName, ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/LeftNavIcon/DigitalSignature.png"), new Uri("pack://application:,,,/Style/DarkModeResources/LeftNavIcon/DigitalSignature.png")), ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/LeftNavIcon/DigitalSignature-Select.png"), new Uri("pack://application:,,,/Style/DarkModeResources/LeftNavIcon/DigitalSignature-Select.png"))),
				new NavigationModel("Attachment", Resources.AnnotationFileAttachment, ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/LeftNavIcon/Attachment.png"), new Uri("pack://application:,,,/Style/DarkModeResources/LeftNavIcon/Attachment.png")), ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/LeftNavIcon/Attachment-Select.png"), new Uri("pack://application:,,,/Style/DarkModeResources/LeftNavIcon/Attachment-Select.png")))
			};
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x0600073A RID: 1850 RVA: 0x000221AE File Offset: 0x000203AE
		// (set) Token: 0x0600073B RID: 1851 RVA: 0x000221B6 File Offset: 0x000203B6
		public ObservableCollection<MainMenuGroup> MainMenus { get; private set; }

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x0600073C RID: 1852 RVA: 0x000221BF File Offset: 0x000203BF
		// (set) Token: 0x0600073D RID: 1853 RVA: 0x000221C7 File Offset: 0x000203C7
		public ObservableCollection<NavigationModel> LeftNavList { get; private set; }

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x0600073E RID: 1854 RVA: 0x000221D0 File Offset: 0x000203D0
		// (set) Token: 0x0600073F RID: 1855 RVA: 0x000221D8 File Offset: 0x000203D8
		public bool ToolbarInited
		{
			get
			{
				return this.toolbarInited;
			}
			set
			{
				if (base.SetProperty<bool>(ref this.toolbarInited, value, "ToolbarInited") && value)
				{
					this.IsShowToolbar = ConfigManager.GetLaunchAPPShowFlag("LaunchToolbar");
				}
			}
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000740 RID: 1856 RVA: 0x00022200 File Offset: 0x00020400
		// (set) Token: 0x06000741 RID: 1857 RVA: 0x00022208 File Offset: 0x00020408
		public bool IsShowToolbar
		{
			get
			{
				return this.isShowToolbar;
			}
			set
			{
				base.SetProperty<bool>(ref this.isShowToolbar, value, "IsShowToolbar");
			}
		}

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000742 RID: 1858 RVA: 0x0002221D File Offset: 0x0002041D
		// (set) Token: 0x06000743 RID: 1859 RVA: 0x00022225 File Offset: 0x00020425
		public bool IsShowFooter
		{
			get
			{
				return this.isShowFooter;
			}
			set
			{
				base.SetProperty<bool>(ref this.isShowFooter, value, "IsShowFooter");
			}
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000744 RID: 1860 RVA: 0x0002223C File Offset: 0x0002043C
		// (set) Token: 0x06000745 RID: 1861 RVA: 0x0002226C File Offset: 0x0002046C
		public SearchModel SearchModel
		{
			get
			{
				SearchModel searchModel;
				if ((searchModel = this.searchModel) == null)
				{
					searchModel = (this.SearchModel = new SearchModel(this.mainViewModel.DocumentWrapper));
				}
				return searchModel;
			}
			set
			{
				SearchModel searchModel = this.searchModel;
				if (base.SetProperty<SearchModel>(ref this.searchModel, value, "SearchModel") && searchModel != null)
				{
					searchModel.Dispose();
				}
			}
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x0002229D File Offset: 0x0002049D
		// (set) Token: 0x06000747 RID: 1863 RVA: 0x000222A8 File Offset: 0x000204A8
		public NavigationModel SelectedLeftNavItem
		{
			get
			{
				return this.selectedLeftNavItem;
			}
			set
			{
				if (base.SetProperty<NavigationModel>(ref this.selectedLeftNavItem, value, "SelectedLeftNavItem"))
				{
					NavigationModel navigationModel = this.selectedLeftNavItem;
					if (((navigationModel != null) ? navigationModel.Name : null) == "Annotation")
					{
						AllPageCommetCollectionView pageCommetSource = this.mainViewModel.PageCommetSource;
						if (pageCommetSource == null)
						{
							return;
						}
						pageCommetSource.StartLoad();
					}
				}
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000748 RID: 1864 RVA: 0x000222FC File Offset: 0x000204FC
		public RelayCommand CloseLeftNavMenuCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.closeLeftNavMenuCmd) == null)
				{
					relayCommand = (this.closeLeftNavMenuCmd = new RelayCommand(delegate
					{
						this.SelectedLeftNavItem = null;
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000749 RID: 1865 RVA: 0x00022330 File Offset: 0x00020530
		public RelayCommand ShowToolbarCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.showToolbarCmd) == null)
				{
					relayCommand = (this.showToolbarCmd = new RelayCommand(new Action(this.ShowToolbar)));
				}
				return relayCommand;
			}
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00022361 File Offset: 0x00020561
		private void ShowToolbar()
		{
			this.IsShowToolbar = !this.IsShowToolbar;
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x00022374 File Offset: 0x00020574
		public async Task ShowLeftNavMenuAsync(string menuName)
		{
			NavigationModel navigationModel = this.SelectedLeftNavItem;
			if (!(((navigationModel != null) ? navigationModel.Name : null) == menuName))
			{
				this.SelectedLeftNavItem = this.LeftNavList.FirstOrDefault((NavigationModel c) => c.Name == menuName);
				await DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
				}));
			}
		}

		// Token: 0x040003C3 RID: 963
		private readonly MainViewModel mainViewModel;

		// Token: 0x040003C4 RID: 964
		private bool toolbarInited;

		// Token: 0x040003C5 RID: 965
		private bool isShowToolbar;

		// Token: 0x040003C6 RID: 966
		private bool isShowFooter = true;

		// Token: 0x040003C7 RID: 967
		private SearchModel searchModel;

		// Token: 0x040003C8 RID: 968
		private NavigationModel selectedLeftNavItem;

		// Token: 0x040003C9 RID: 969
		private RelayCommand closeLeftNavMenuCmd;

		// Token: 0x040003CA RID: 970
		private RelayCommand showToolbarCmd;
	}
}
