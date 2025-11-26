using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FileWatcher
{
	// Token: 0x02000002 RID: 2
	public static class KnownFolders
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public static string GetPath(KnownFolder knownFolder)
		{
			return KnownFolders.SHGetKnownFolderPath(KnownFolders._guids[knownFolder], 0U, IntPtr.Zero);
		}

		// Token: 0x06000002 RID: 2
		[DllImport("shell32", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
		private static extern string SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken);

		// Token: 0x06000003 RID: 3 RVA: 0x00002060 File Offset: 0x00000260
		// Note: this type is marked as 'beforefieldinit'.
		static KnownFolders()
		{
			Dictionary<KnownFolder, Guid> dictionary = new Dictionary<KnownFolder, Guid>();
			dictionary[KnownFolder.Contacts] = new Guid("56784854-C6CB-462B-8169-88E350ACB882");
			dictionary[KnownFolder.Downloads] = new Guid("374DE290-123F-4565-9164-39C4925E467B");
			dictionary[KnownFolder.Favorites] = new Guid("1777F761-68AD-4D8A-87BD-30B759FA33DD");
			dictionary[KnownFolder.Links] = new Guid("BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968");
			dictionary[KnownFolder.SavedGames] = new Guid("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4");
			dictionary[KnownFolder.SavedSearches] = new Guid("7D1D3A04-DEBB-4115-95CF-2F29DA2920DA");
			KnownFolders._guids = dictionary;
		}

		// Token: 0x04000001 RID: 1
		private static readonly Dictionary<KnownFolder, Guid> _guids;
	}
}
