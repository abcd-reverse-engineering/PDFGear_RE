using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace pdfeditor.Utils
{
	// Token: 0x02000095 RID: 149
	public class PdfViewJumpManager : ObservableObject
	{
		// Token: 0x17000259 RID: 601
		// (get) Token: 0x060009BD RID: 2493 RVA: 0x00031D40 File Offset: 0x0002FF40
		public bool IsFirstView
		{
			get
			{
				return this.backStack.Count == 0;
			}
		}

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x060009BE RID: 2494 RVA: 0x00031D50 File Offset: 0x0002FF50
		public bool IsLastView
		{
			get
			{
				return this.preStack.Count == 0;
			}
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x00031D60 File Offset: 0x0002FF60
		public int ViewStackOperation(bool isBack, int pageIndex)
		{
			if (isBack)
			{
				this.InsertRecord(ref this.preStack, pageIndex);
				return this.backStack.Pop();
			}
			this.InsertRecord(ref this.backStack, pageIndex);
			return this.preStack.Pop();
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x00031D98 File Offset: 0x0002FF98
		private void InsertRecord(ref Stack<int> stack, int pageIndex)
		{
			if (stack.Count <= 0 || pageIndex != stack.Peek())
			{
				if (stack.Count > 500)
				{
					stack = new Stack<int>(stack.Skip(stack.Count - 250).Take(250));
				}
				stack.Push(pageIndex);
			}
			base.OnPropertyChanged("IsFirstView");
			base.OnPropertyChanged("IsLastView");
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x00031E0A File Offset: 0x0003000A
		public void NewRecord(int pageIndex)
		{
			this.InsertRecord(ref this.backStack, pageIndex);
		}

		// Token: 0x060009C2 RID: 2498 RVA: 0x00031E19 File Offset: 0x00030019
		public int ViewBackCmd(int pageIndex)
		{
			int num = this.ViewStackOperation(true, pageIndex);
			base.OnPropertyChanged("IsFirstView");
			base.OnPropertyChanged("IsLastView");
			return num;
		}

		// Token: 0x060009C3 RID: 2499 RVA: 0x00031E39 File Offset: 0x00030039
		public int ViewPreCmd(int pageIndex)
		{
			int num = this.ViewStackOperation(false, pageIndex);
			base.OnPropertyChanged("IsFirstView");
			base.OnPropertyChanged("IsLastView");
			return num;
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x00031E59 File Offset: 0x00030059
		public void ClearStack()
		{
			this.backStack.Clear();
			this.preStack.Clear();
			base.OnPropertyChanged("IsFirstView");
			base.OnPropertyChanged("IsLastView");
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x00031E87 File Offset: 0x00030087
		public void StackChange()
		{
			this.preStack.Clear();
			base.OnPropertyChanged("IsFirstView");
			base.OnPropertyChanged("IsLastView");
		}

		// Token: 0x0400047B RID: 1147
		private Stack<int> backStack = new Stack<int>();

		// Token: 0x0400047C RID: 1148
		private Stack<int> preStack = new Stack<int>();
	}
}
