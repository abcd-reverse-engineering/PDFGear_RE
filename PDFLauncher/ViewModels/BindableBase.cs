using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PDFLauncher.ViewModels
{
	// Token: 0x02000013 RID: 19
	public abstract class BindableBase : INotifyPropertyChanged
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000156 RID: 342 RVA: 0x00005D78 File Offset: 0x00003F78
		// (remove) Token: 0x06000157 RID: 343 RVA: 0x00005DB0 File Offset: 0x00003FB0
		public event PropertyChangedEventHandler PropertyChanged = delegate
		{
		};

		// Token: 0x06000158 RID: 344 RVA: 0x00005DE5 File Offset: 0x00003FE5
		protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (object.Equals(storage, value))
			{
				return false;
			}
			storage = value;
			this.OnPropertyChanged(propertyName);
			return true;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00005E10 File Offset: 0x00004010
		public void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
