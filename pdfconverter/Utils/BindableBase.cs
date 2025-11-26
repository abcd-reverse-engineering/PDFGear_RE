using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace pdfconverter.Utils
{
	// Token: 0x0200003A RID: 58
	public abstract class BindableBase : INotifyPropertyChanged
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060004BE RID: 1214 RVA: 0x00013044 File Offset: 0x00011244
		// (remove) Token: 0x060004BF RID: 1215 RVA: 0x0001307C File Offset: 0x0001127C
		public event PropertyChangedEventHandler PropertyChanged = delegate
		{
		};

		// Token: 0x060004C0 RID: 1216 RVA: 0x000130B1 File Offset: 0x000112B1
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

		// Token: 0x060004C1 RID: 1217 RVA: 0x000130DC File Offset: 0x000112DC
		public void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
