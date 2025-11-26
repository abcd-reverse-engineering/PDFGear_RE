using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace pdfeditor.Controls.Speech
{
	// Token: 0x020001F0 RID: 496
	public class ImgaeFilePath : INotifyPropertyChanged
	{
		// Token: 0x14000038 RID: 56
		// (add) Token: 0x06001C16 RID: 7190 RVA: 0x00075060 File Offset: 0x00073260
		// (remove) Token: 0x06001C17 RID: 7191 RVA: 0x00075098 File Offset: 0x00073298
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06001C18 RID: 7192 RVA: 0x000750CD File Offset: 0x000732CD
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		// Token: 0x17000A22 RID: 2594
		// (get) Token: 0x06001C19 RID: 7193 RVA: 0x000750E9 File Offset: 0x000732E9
		// (set) Token: 0x06001C1A RID: 7194 RVA: 0x000750F1 File Offset: 0x000732F1
		public string ImagePath
		{
			get
			{
				return this.imagepath;
			}
			set
			{
				this.Set<string>(ref this.imagepath, value, "ImagePath");
			}
		}

		// Token: 0x06001C1B RID: 7195 RVA: 0x00075105 File Offset: 0x00073305
		private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (object.Equals(storage, value))
			{
				return;
			}
			storage = value;
			this.NotifyPropertyChanged(propertyName);
		}

		// Token: 0x04000A36 RID: 2614
		private string imagepath = "pack://application:,,,/Style/Resources/Speech/Play.png";
	}
}
