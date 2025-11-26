using System;
using System.Windows.Input;
using System.Windows.Media;

namespace pdfeditor.Models
{
	// Token: 0x0200012A RID: 298
	public class RecentFileItem
	{
		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x06001247 RID: 4679 RVA: 0x0004AD74 File Offset: 0x00048F74
		// (set) Token: 0x06001248 RID: 4680 RVA: 0x0004AD7C File Offset: 0x00048F7C
		public string FilePath { get; set; }

		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x06001249 RID: 4681 RVA: 0x0004AD85 File Offset: 0x00048F85
		// (set) Token: 0x0600124A RID: 4682 RVA: 0x0004AD8D File Offset: 0x00048F8D
		public string FileName { get; set; }

		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x0600124B RID: 4683 RVA: 0x0004AD96 File Offset: 0x00048F96
		// (set) Token: 0x0600124C RID: 4684 RVA: 0x0004AD9E File Offset: 0x00048F9E
		public ImageSource Icon { get; set; }

		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x0600124D RID: 4685 RVA: 0x0004ADA7 File Offset: 0x00048FA7
		// (set) Token: 0x0600124E RID: 4686 RVA: 0x0004ADAF File Offset: 0x00048FAF
		public ICommand OpenCommand { get; set; }
	}
}
