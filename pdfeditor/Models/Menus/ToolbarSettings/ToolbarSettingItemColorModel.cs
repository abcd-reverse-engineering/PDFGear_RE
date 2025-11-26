using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x0200017D RID: 381
	public class ToolbarSettingItemColorModel : ToolbarSettingItemModel
	{
		// Token: 0x06001602 RID: 5634 RVA: 0x000548DE File Offset: 0x00052ADE
		public ToolbarSettingItemColorModel(ContextMenuItemType type)
			: base(type)
		{
		}

		// Token: 0x06001603 RID: 5635 RVA: 0x000548E7 File Offset: 0x00052AE7
		public ToolbarSettingItemColorModel(ContextMenuItemType type, string configCacheKey)
			: base(type, configCacheKey)
		{
		}

		// Token: 0x170008C8 RID: 2248
		// (get) Token: 0x06001604 RID: 5636 RVA: 0x000548F1 File Offset: 0x00052AF1
		// (set) Token: 0x06001605 RID: 5637 RVA: 0x000548F9 File Offset: 0x00052AF9
		public string RecentColorsKey
		{
			get
			{
				return this.recentColorsKey;
			}
			set
			{
				base.SetProperty<string>(ref this.recentColorsKey, value, "RecentColorsKey");
			}
		}

		// Token: 0x170008C9 RID: 2249
		// (get) Token: 0x06001606 RID: 5638 RVA: 0x0005490E File Offset: 0x00052B0E
		// (set) Token: 0x06001607 RID: 5639 RVA: 0x00054918 File Offset: 0x00052B18
		public ObservableCollection<string> StandardColors
		{
			get
			{
				return this.standardColors;
			}
			set
			{
				this.init = true;
				ObservableCollection<string> observableCollection = this.standardColors;
				if (base.SetProperty<ObservableCollection<string>>(ref this.standardColors, value, "StandardColors"))
				{
					if (observableCollection != null)
					{
						WeakEventManager<ObservableCollection<string>, CollectionChangeEventArgs>.RemoveHandler(observableCollection, "CollectionChanged", new EventHandler<CollectionChangeEventArgs>(this.OnStandardColorsItemChanged));
					}
					if (this.standardColors != null)
					{
						WeakEventManager<ObservableCollection<string>, CollectionChangeEventArgs>.AddHandler(this.standardColors, "CollectionChanged", new EventHandler<CollectionChangeEventArgs>(this.OnStandardColorsItemChanged));
					}
					EventHandler colorsChanged = this.ColorsChanged;
					if (colorsChanged == null)
					{
						return;
					}
					colorsChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x170008CA RID: 2250
		// (get) Token: 0x06001608 RID: 5640 RVA: 0x0005499B File Offset: 0x00052B9B
		// (set) Token: 0x06001609 RID: 5641 RVA: 0x000549A4 File Offset: 0x00052BA4
		public ObservableCollection<string> RecentColors
		{
			get
			{
				return this.recentColors;
			}
			protected set
			{
				ObservableCollection<string> observableCollection = this.recentColors;
				if (base.SetProperty<ObservableCollection<string>>(ref this.recentColors, value, "RecentColors"))
				{
					if (observableCollection != null)
					{
						WeakEventManager<ObservableCollection<string>, CollectionChangeEventArgs>.RemoveHandler(observableCollection, "CollectionChanged", new EventHandler<CollectionChangeEventArgs>(this.OnRecentColorsItemChanged));
					}
					if (this.recentColors != null)
					{
						WeakEventManager<ObservableCollection<string>, CollectionChangeEventArgs>.AddHandler(this.recentColors, "CollectionChanged", new EventHandler<CollectionChangeEventArgs>(this.OnRecentColorsItemChanged));
					}
					EventHandler colorsChanged = this.ColorsChanged;
					if (colorsChanged == null)
					{
						return;
					}
					colorsChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x0600160A RID: 5642 RVA: 0x00054A20 File Offset: 0x00052C20
		private void OnStandardColorsItemChanged(object sender, CollectionChangeEventArgs e)
		{
			EventHandler colorsChanged = this.ColorsChanged;
			if (colorsChanged == null)
			{
				return;
			}
			colorsChanged(this, EventArgs.Empty);
		}

		// Token: 0x0600160B RID: 5643 RVA: 0x00054A38 File Offset: 0x00052C38
		private void OnRecentColorsItemChanged(object sender, CollectionChangeEventArgs e)
		{
			EventHandler colorsChanged = this.ColorsChanged;
			if (colorsChanged == null)
			{
				return;
			}
			colorsChanged(this, EventArgs.Empty);
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x00054A50 File Offset: 0x00052C50
		protected override void OnSelectedValueChanged()
		{
			base.OnSelectedValueChanged();
			if (this.init)
			{
				string text = base.SelectedValue as string;
				if (text != null)
				{
					this.AddToRecentColor(text);
				}
			}
			this.init = true;
			if (base.Id.AnnotationMode == AnnotationMode.Ink)
			{
				(base.Parent[3] as ToolbarSettingInkEraserModel).IsChecked = false;
			}
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x00054AB0 File Offset: 0x00052CB0
		private void AddToRecentColor(string color)
		{
			try
			{
				ObservableCollection<string> observableCollection = this.RecentColors;
				List<string> list = ((observableCollection != null) ? observableCollection.ToList<string>() : null);
				if (list == null)
				{
					list = new List<string>();
				}
				Color color2 = (Color)ColorConverter.ConvertFromString(color);
				string text = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { color2.A, color2.R, color2.G, color2.B });
				ObservableCollection<string> observableCollection2 = this.StandardColors;
				IEnumerable<Color> enumerable;
				if (observableCollection2 == null)
				{
					enumerable = null;
				}
				else
				{
					enumerable = (from x in observableCollection2.Select(delegate(string x)
						{
							try
							{
								return (Color?)ColorConverter.ConvertFromString(x);
							}
							catch
							{
							}
							return null;
						})
						where x != null
						select x.Value).Distinct<Color>();
				}
				HashSet<Color> hashSet = new HashSet<Color>(enumerable ?? Enumerable.Empty<Color>());
				ObservableCollection<string> observableCollection3 = this.RecentColors;
				IEnumerable<Color> enumerable2;
				if (observableCollection3 == null)
				{
					enumerable2 = null;
				}
				else
				{
					enumerable2 = (from x in observableCollection3.Select(delegate(string x)
						{
							try
							{
								return (Color?)ColorConverter.ConvertFromString(x);
							}
							catch
							{
							}
							return null;
						})
						where x != null
						select x.Value).Distinct<Color>();
				}
				HashSet<Color> hashSet2 = new HashSet<Color>(enumerable2 ?? Enumerable.Empty<Color>());
				if (!hashSet.Contains(color2))
				{
					if (!hashSet2.Contains(color2))
					{
						list.Add(text);
						for (int i = 0; i < list.Count - 5; i++)
						{
							list.RemoveAt(i);
						}
						this.RecentColors = new ObservableCollection<string>(list);
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x0600160E RID: 5646 RVA: 0x00054CB4 File Offset: 0x00052EB4
		// (remove) Token: 0x0600160F RID: 5647 RVA: 0x00054CEC File Offset: 0x00052EEC
		public event EventHandler ColorsChanged;

		// Token: 0x06001610 RID: 5648 RVA: 0x00054D24 File Offset: 0x00052F24
		protected override void SaveConfigCore(Dictionary<string, string> dict)
		{
			base.SaveConfigCore(dict);
			dict["SelectedValue"] = (string)base.NontransientSelectedValue;
			string text = "RecentColors";
			ObservableCollection<string> observableCollection = this.recentColors;
			IEnumerable<string> enumerable = ((observableCollection != null) ? observableCollection.ToList<string>() : null);
			dict[text] = JsonConvert.SerializeObject(enumerable ?? Enumerable.Empty<string>());
		}

		// Token: 0x06001611 RID: 5649 RVA: 0x00054D7C File Offset: 0x00052F7C
		protected override void ApplyConfigCore(Dictionary<string, string> dict)
		{
			base.ApplyConfigCore(dict);
			string text;
			if (dict.TryGetValue("SelectedValue", out text))
			{
				base.NontransientSelectedValue = text;
			}
			string text2;
			if (dict.TryGetValue("RecentColors", out text2))
			{
				if (string.IsNullOrEmpty(text2) || text2 == "[]")
				{
					this.RecentColors = null;
					return;
				}
				try
				{
					this.RecentColors = new ObservableCollection<string>(JsonConvert.DeserializeObject<string[]>(text2));
				}
				catch
				{
				}
			}
		}

		// Token: 0x0400075C RID: 1884
		private string recentColorsKey;

		// Token: 0x0400075D RID: 1885
		private ObservableCollection<string> standardColors;

		// Token: 0x0400075E RID: 1886
		private ObservableCollection<string> recentColors;

		// Token: 0x0400075F RID: 1887
		private bool init;
	}
}
