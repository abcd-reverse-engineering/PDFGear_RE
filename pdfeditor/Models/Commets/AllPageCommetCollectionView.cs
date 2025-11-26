using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Net;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Commets
{
	// Token: 0x0200018D RID: 397
	public class AllPageCommetCollectionView : ObservableCollection<PageCommetCollection>, IDisposable
	{
		// Token: 0x060016CB RID: 5835 RVA: 0x000569A8 File Offset: 0x00054BA8
		public AllPageCommetCollectionView(PdfDocument document)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			this.Document = document;
			this.cts = new CancellationTokenSource();
			this.userList = new ObservableCollection<AnnotationsFilterModel>();
			this.AnnotationList = new ObservableCollection<AnnotationsFilterModel>();
			this.commetModels = new ObservableCollection<PageCommetCollection>();
		}

		// Token: 0x1700090C RID: 2316
		// (get) Token: 0x060016CC RID: 5836 RVA: 0x00056A0F File Offset: 0x00054C0F
		public PdfDocument Document { get; }

		// Token: 0x1700090D RID: 2317
		// (get) Token: 0x060016CD RID: 5837 RVA: 0x00056A17 File Offset: 0x00054C17
		// (set) Token: 0x060016CE RID: 5838 RVA: 0x00056A1F File Offset: 0x00054C1F
		public ObservableCollection<AnnotationsFilterModel> userList { get; private set; }

		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x060016CF RID: 5839 RVA: 0x00056A28 File Offset: 0x00054C28
		// (set) Token: 0x060016D0 RID: 5840 RVA: 0x00056A30 File Offset: 0x00054C30
		public ObservableCollection<AnnotationsFilterModel> AnnotationList { get; private set; }

		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x060016D1 RID: 5841 RVA: 0x00056A39 File Offset: 0x00054C39
		// (set) Token: 0x060016D2 RID: 5842 RVA: 0x00056A41 File Offset: 0x00054C41
		public ObservableCollection<PageCommetCollection> commetModels { get; private set; }

		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x060016D3 RID: 5843 RVA: 0x00056A4A File Offset: 0x00054C4A
		// (set) Token: 0x060016D4 RID: 5844 RVA: 0x00056A52 File Offset: 0x00054C52
		public bool IsLoading
		{
			get
			{
				return this.isLoading;
			}
			private set
			{
				if (this.isLoading != value)
				{
					this.isLoading = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("IsLoading"));
				}
			}
		}

		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x060016D5 RID: 5845 RVA: 0x00056A74 File Offset: 0x00054C74
		// (set) Token: 0x060016D6 RID: 5846 RVA: 0x00056A7C File Offset: 0x00054C7C
		public bool IsCompleted
		{
			get
			{
				return this.isCompleted;
			}
			private set
			{
				if (this.isCompleted != value)
				{
					this.isCompleted = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("IsCompleted"));
				}
			}
		}

		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x060016D7 RID: 5847 RVA: 0x00056A9E File Offset: 0x00054C9E
		public double Progress
		{
			get
			{
				if (!this.isCompletedInternal)
				{
					return (double)(this.loadedPageInternal + 1) * 1.0 / (double)this.Document.Pages.Count;
				}
				return 1.0;
			}
		}

		// Token: 0x060016D8 RID: 5848 RVA: 0x00056AD8 File Offset: 0x00054CD8
		public async void StartLoad()
		{
			if (!this.isCompletedInternal)
			{
				if (!this.IsLoading)
				{
					this.IsLoading = true;
					try
					{
						for (int i = 0; i < this.Document.Pages.Count; i++)
						{
							if (i != 0 && i % 3 == 0)
							{
								await Task.Delay(10, this.cts.Token);
								this.cts.Token.ThrowIfCancellationRequested();
							}
							PageCommetCollection pageCommetCollection = PageCommetCollection.Create(this.Document, i);
							if (pageCommetCollection != null && pageCommetCollection.Count > 0)
							{
								base.Add(pageCommetCollection);
								this.commetModels.Add(pageCommetCollection);
							}
							this.loadedPageInternal = i;
							this.OnPropertyChanged(new PropertyChangedEventArgs("Progress"));
							this.cts.Token.ThrowIfCancellationRequested();
						}
						this.isCompletedInternal = true;
						object obj = this.notifyChangedLocker;
						lock (obj)
						{
							this.IsCompleted = true;
							this.IsLoading = false;
						}
						foreach (PageCommetCollection pageCommetCollection2 in this.commetModels)
						{
							foreach (CommetModel commetModel in pageCommetCollection2)
							{
								AnnotationsFilterModel annotationsFilterModel = new AnnotationsFilterModel();
								annotationsFilterModel.Text = commetModel.Text;
								AnnotationsFilterModel annotationsFilterModel2 = new AnnotationsFilterModel();
								annotationsFilterModel2.Text = commetModel.Title;
								if (this.userList.Count < 1)
								{
									this.userList.Add(annotationsFilterModel);
								}
								else
								{
									bool flag2 = false;
									for (int j = 0; j < this.userList.Count; j++)
									{
										if (this.userList[j].Text == commetModel.Text)
										{
											this.userList[j].Count++;
											flag2 = true;
										}
									}
									if (!flag2)
									{
										this.userList.Add(annotationsFilterModel);
									}
								}
								if (this.AnnotationList.Count < 1)
								{
									this.AnnotationList.Add(annotationsFilterModel2);
								}
								else
								{
									bool flag3 = false;
									for (int k = 0; k < this.AnnotationList.Count; k++)
									{
										if (this.AnnotationList[k].Text == commetModel.Title)
										{
											this.AnnotationList[k].Count++;
											flag3 = true;
										}
									}
									if (!flag3)
									{
										this.AnnotationList.Add(annotationsFilterModel2);
									}
								}
							}
						}
						MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
						requiredService.AllCount = 0;
						foreach (PageCommetCollection pageCommetCollection3 in this)
						{
							requiredService.AllCount += pageCommetCollection3.Count;
						}
						requiredService.IsUserFilterAllChecked = new bool?(true);
						requiredService.IsKindFilterAllChecked = new bool?(true);
					}
					catch (OperationCanceledException)
					{
					}
				}
			}
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x00056B10 File Offset: 0x00054D10
		public void FilterShowItems()
		{
			base.Clear();
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			requiredService.IsSelectedAll = new bool?(false);
			for (int i = 0; i < this.commetModels.Count; i++)
			{
				List<CommetModel> list = new List<CommetModel>();
				foreach (CommetModel commetModel in this.commetModels[i])
				{
					if (this.userList != null && this.AnnotationList != null)
					{
						foreach (AnnotationsFilterModel annotationsFilterModel in this.userList)
						{
							foreach (AnnotationsFilterModel annotationsFilterModel2 in this.AnnotationList)
							{
								if (annotationsFilterModel.IsSelect && annotationsFilterModel.Text == commetModel.Text && annotationsFilterModel2.IsSelect && annotationsFilterModel2.Text == commetModel.Title)
								{
									list.Add(commetModel.Clone());
								}
							}
						}
					}
				}
				PageCommetCollection pageCommetCollection = new PageCommetCollection(this.commetModels[i].Document, this.commetModels[i].PageIndex, list);
				for (int j = 0; j < list.Count; j++)
				{
					list[j].Parent = pageCommetCollection;
				}
				if (pageCommetCollection != null && pageCommetCollection.Count > 0)
				{
					base.Add(pageCommetCollection);
				}
			}
			requiredService.AllCount = 0;
			foreach (PageCommetCollection pageCommetCollection2 in this)
			{
				requiredService.AllCount += pageCommetCollection2.Count;
			}
		}

		// Token: 0x060016DA RID: 5850 RVA: 0x00056D34 File Offset: 0x00054F34
		public void NotifyPageAnnotationChanged(int pageIndex)
		{
			this.NotifyPageAnnotationChanged(pageIndex, true);
		}

		// Token: 0x060016DB RID: 5851 RVA: 0x00056D40 File Offset: 0x00054F40
		public void NotifyDeletePageAnnotationChanged()
		{
			if (this.Document != null)
			{
				int pageIndex2;
				int pageIndex;
				for (pageIndex = 0; pageIndex <= this.loadedPageInternal; pageIndex = pageIndex2 + 1)
				{
					object obj = this.notifyChangedLocker;
					lock (obj)
					{
						try
						{
							this.IsLoading = true;
							this.IsCompleted = false;
							this.FirstOrDefault((PageCommetCollection c) => c.PageIndex == pageIndex);
							PageCommetCollection pageCommetCollection = this.commetModels.FirstOrDefault((PageCommetCollection c) => c.PageIndex == pageIndex);
							PageCommetCollection pageCommetCollection2 = PageCommetCollection.Create(this.Document, pageIndex);
							if (pageCommetCollection == null)
							{
								if (pageCommetCollection2 != null)
								{
									int num = 0;
									while (num < this.commetModels.Count && this.commetModels[num].PageIndex <= pageIndex)
									{
										num++;
									}
									this.commetModels.Insert(num, pageCommetCollection2);
								}
							}
							else
							{
								int num2 = this.commetModels.IndexOf(pageCommetCollection);
								if (pageCommetCollection2 != null)
								{
									this.commetModels[num2] = pageCommetCollection2;
								}
								else if (num2 >= 0)
								{
									this.commetModels.RemoveAt(num2);
								}
							}
						}
						finally
						{
							this.IsLoading = !this.isCompletedInternal;
							this.IsCompleted = this.isCompletedInternal;
						}
					}
					pageIndex2 = pageIndex;
				}
				this.ReflashUserList();
			}
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x00056EB8 File Offset: 0x000550B8
		private void NotifyPageAnnotationChanged(int pageIndex, bool requeue)
		{
			if (pageIndex >= 0 && this.Document != null)
			{
				if (pageIndex <= this.loadedPageInternal)
				{
					object obj = this.notifyChangedLocker;
					lock (obj)
					{
						try
						{
							this.IsLoading = true;
							this.IsCompleted = false;
							this.FirstOrDefault((PageCommetCollection c) => c.PageIndex == pageIndex);
							PageCommetCollection pageCommetCollection = this.commetModels.FirstOrDefault((PageCommetCollection c) => c.PageIndex == pageIndex);
							PageCommetCollection pageCommetCollection2 = PageCommetCollection.Create(this.Document, pageIndex);
							if (pageCommetCollection == null)
							{
								if (pageCommetCollection2 != null)
								{
									int num = 0;
									while (num < this.commetModels.Count && this.commetModels[num].PageIndex <= pageIndex)
									{
										num++;
									}
									this.commetModels.Insert(num, pageCommetCollection2);
								}
							}
							else
							{
								int num2 = this.commetModels.IndexOf(pageCommetCollection);
								if (pageCommetCollection2 != null)
								{
									this.commetModels[num2] = pageCommetCollection2;
								}
								else if (num2 >= 0)
								{
									this.commetModels.RemoveAt(num2);
								}
							}
							goto IL_017A;
						}
						finally
						{
							this.IsLoading = !this.isCompletedInternal;
							this.IsCompleted = this.isCompletedInternal;
						}
					}
				}
				if (pageIndex == this.loadedPageInternal + 1 && this.IsLoading && requeue)
				{
					DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
					{
						this.NotifyPageAnnotationChanged(pageIndex, false);
					}));
				}
				IL_017A:
				this.ReflashUserList();
			}
		}

		// Token: 0x060016DD RID: 5853 RVA: 0x00057064 File Offset: 0x00055264
		private void ReflashUserList()
		{
			try
			{
				ObservableCollection<AnnotationsFilterModel> observableCollection = new ObservableCollection<AnnotationsFilterModel>();
				foreach (AnnotationsFilterModel annotationsFilterModel in this.userList)
				{
					annotationsFilterModel.Count = 0;
					observableCollection.Add(annotationsFilterModel);
				}
				ObservableCollection<AnnotationsFilterModel> observableCollection2 = new ObservableCollection<AnnotationsFilterModel>();
				foreach (AnnotationsFilterModel annotationsFilterModel2 in this.AnnotationList)
				{
					annotationsFilterModel2.Count = 0;
					observableCollection2.Add(annotationsFilterModel2);
				}
				this.AnnotationList.Clear();
				this.userList.Clear();
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				foreach (PageCommetCollection pageCommetCollection in this.commetModels)
				{
					using (IEnumerator<CommetModel> enumerator3 = pageCommetCollection.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							CommetModel value = enumerator3.Current;
							AnnotationsFilterModel annotationsFilterModel3 = new AnnotationsFilterModel
							{
								Text = value.Text
							};
							AnnotationsFilterModel annotationsFilterModel4 = new AnnotationsFilterModel
							{
								Text = value.Title
							};
							if (observableCollection.Count < 1)
							{
								observableCollection.Add(annotationsFilterModel3);
								this.userList.Add(annotationsFilterModel3);
							}
							else
							{
								bool flag = false;
								Func<AnnotationsFilterModel, bool> <>9__0;
								for (int i = 0; i < observableCollection.Count; i++)
								{
									if (observableCollection[i].Text == value.Text)
									{
										IEnumerable<AnnotationsFilterModel> userList = this.userList;
										Func<AnnotationsFilterModel, bool> func;
										if ((func = <>9__0) == null)
										{
											func = (<>9__0 = (AnnotationsFilterModel x) => x.Text == value.Text);
										}
										if (userList.Where(func).Count<AnnotationsFilterModel>() < 1)
										{
											this.userList.Add(observableCollection[i]);
										}
										AnnotationsFilterModel annotationsFilterModel5 = observableCollection[i];
										int num = annotationsFilterModel5.Count;
										annotationsFilterModel5.Count = num + 1;
										flag = true;
										break;
									}
								}
								if (!flag)
								{
									if (!requiredService.IsUserFilterAllChecked.GetValueOrDefault())
									{
										annotationsFilterModel3.IsSelect = false;
									}
									observableCollection.Add(annotationsFilterModel3);
									this.userList.Add(annotationsFilterModel3);
									bool? flag2 = requiredService.IsUserFilterAllChecked;
									bool flag3 = false;
									if ((flag2.GetValueOrDefault() == flag3) & (flag2 != null))
									{
										requiredService.IsUserFilterAllChecked = null;
									}
								}
							}
							if (observableCollection2.Count < 1)
							{
								observableCollection2.Add(annotationsFilterModel4);
								this.AnnotationList.Add(annotationsFilterModel4);
							}
							else
							{
								bool flag4 = false;
								Func<AnnotationsFilterModel, bool> <>9__1;
								for (int j = 0; j < observableCollection2.Count; j++)
								{
									if (observableCollection2[j].Text == value.Title)
									{
										IEnumerable<AnnotationsFilterModel> annotationList = this.AnnotationList;
										Func<AnnotationsFilterModel, bool> func2;
										if ((func2 = <>9__1) == null)
										{
											func2 = (<>9__1 = (AnnotationsFilterModel x) => x.Text == value.Title);
										}
										if (annotationList.Where(func2).Count<AnnotationsFilterModel>() < 1)
										{
											this.AnnotationList.Add(observableCollection2[j]);
										}
										AnnotationsFilterModel annotationsFilterModel6 = observableCollection2[j];
										int num = annotationsFilterModel6.Count;
										annotationsFilterModel6.Count = num + 1;
										flag4 = true;
										break;
									}
								}
								if (!flag4)
								{
									if (!requiredService.IsKindFilterAllChecked.GetValueOrDefault())
									{
										annotationsFilterModel4.IsSelect = false;
									}
									observableCollection2.Add(annotationsFilterModel4);
									this.AnnotationList.Add(annotationsFilterModel4);
									bool? flag2 = requiredService.IsKindFilterAllChecked;
									bool flag3 = false;
									if ((flag2.GetValueOrDefault() == flag3) & (flag2 != null))
									{
										requiredService.IsKindFilterAllChecked = null;
									}
								}
							}
						}
					}
				}
				bool flag5 = false;
				foreach (AnnotationsFilterModel annotationsFilterModel7 in observableCollection2)
				{
					for (int k = 0; k < this.AnnotationList.Count; k++)
					{
						if (this.AnnotationList[k].Text == annotationsFilterModel7.Text)
						{
							this.AnnotationList[k].Count = annotationsFilterModel7.Count;
							if (this.AnnotationList[k].IsSelect)
							{
								flag5 = true;
							}
						}
					}
				}
				if (!flag5)
				{
					if (requiredService.IsKindFilterAllChecked == null)
					{
						requiredService.IsKindFilterAllChecked = new bool?(false);
					}
				}
				else
				{
					flag5 = true;
				}
				foreach (AnnotationsFilterModel annotationsFilterModel8 in observableCollection)
				{
					for (int l = 0; l < this.userList.Count; l++)
					{
						if (this.userList[l].Text == annotationsFilterModel8.Text)
						{
							this.userList[l].Count = annotationsFilterModel8.Count;
							if (this.userList[l].IsSelect)
							{
								flag5 = true;
							}
						}
					}
				}
				if (!flag5 && requiredService.IsUserFilterAllChecked == null)
				{
					requiredService.IsUserFilterAllChecked = new bool?(false);
				}
				this.FilterShowItems();
			}
			catch
			{
			}
		}

		// Token: 0x060016DE RID: 5854 RVA: 0x00057638 File Offset: 0x00055838
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					this.cts.Cancel();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x00057657 File Offset: 0x00055857
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x04000798 RID: 1944
		private bool disposedValue;

		// Token: 0x0400079A RID: 1946
		private bool isLoading;

		// Token: 0x0400079B RID: 1947
		private bool isCompleted;

		// Token: 0x0400079C RID: 1948
		private bool isCompletedInternal;

		// Token: 0x0400079D RID: 1949
		private int loadedPageInternal = -1;

		// Token: 0x0400079E RID: 1950
		private object notifyChangedLocker = new object();

		// Token: 0x0400079F RID: 1951
		private CancellationTokenSource cts;
	}
}
