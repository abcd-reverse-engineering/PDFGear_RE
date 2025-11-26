using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using pdfeditor.Controls.Annotations;

namespace pdfeditor.Models.Stamp
{
	// Token: 0x02000139 RID: 313
	public class GroupItem : INotifyPropertyChanged
	{
		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x0600131A RID: 4890 RVA: 0x0004E3BA File Offset: 0x0004C5BA
		// (set) Token: 0x0600131B RID: 4891 RVA: 0x0004E3C2 File Offset: 0x0004C5C2
		public string Title { get; set; }

		// Token: 0x170007BC RID: 1980
		// (get) Token: 0x0600131C RID: 4892 RVA: 0x0004E3CB File Offset: 0x0004C5CB
		// (set) Token: 0x0600131D RID: 4893 RVA: 0x0004E3D3 File Offset: 0x0004C5D3
		public CustStampModel custStampModel { get; set; }

		// Token: 0x170007BD RID: 1981
		// (get) Token: 0x0600131E RID: 4894 RVA: 0x0004E3DC File Offset: 0x0004C5DC
		// (set) Token: 0x0600131F RID: 4895 RVA: 0x0004E3E4 File Offset: 0x0004C5E4
		public bool IsChecked
		{
			get
			{
				return this._isChecked;
			}
			set
			{
				if (this._isChecked != value)
				{
					this._isChecked = value;
				}
				this.OnPropertyChanged("IsChecked");
			}
		}

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x06001320 RID: 4896 RVA: 0x0004E404 File Offset: 0x0004C604
		// (remove) Token: 0x06001321 RID: 4897 RVA: 0x0004E43C File Offset: 0x0004C63C
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06001322 RID: 4898 RVA: 0x0004E471 File Offset: 0x0004C671
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		// Token: 0x04000608 RID: 1544
		private bool _isChecked;
	}
}
