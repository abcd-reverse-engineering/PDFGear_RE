using System;
using System.Collections.Generic;

namespace pdfeditor.Controls.PageHeaderFooters
{
	// Token: 0x0200023D RID: 573
	public class LocationStringParser
	{
		// Token: 0x0600208D RID: 8333 RVA: 0x00094FCF File Offset: 0x000931CF
		public static IEnumerable<LocationStringParser.LocationToken> GetTokens(string str)
		{
			LocationStringParser.<GetTokens>d__0 <GetTokens>d__ = new LocationStringParser.<GetTokens>d__0(-2);
			<GetTokens>d__.<>3__str = str;
			return <GetTokens>d__;
		}

		// Token: 0x0600208E RID: 8334 RVA: 0x00094FDF File Offset: 0x000931DF
		private static IEnumerable<LocationStringParser.LocationToken> GetTokensCore(string str)
		{
			LocationStringParser.<GetTokensCore>d__1 <GetTokensCore>d__ = new LocationStringParser.<GetTokensCore>d__1(-2);
			<GetTokensCore>d__.<>3__str = str;
			return <GetTokensCore>d__;
		}

		// Token: 0x020006EA RID: 1770
		public enum LocationTokenize
		{
			// Token: 0x0400238B RID: 9099
			String,
			// Token: 0x0400238C RID: 9100
			PagePlaceholder
		}

		// Token: 0x020006EB RID: 1771
		public struct LocationToken
		{
			// Token: 0x0600351C RID: 13596 RVA: 0x0010B44A File Offset: 0x0010964A
			public LocationToken(LocationStringParser.LocationTokenize tokenize, string text)
			{
				if (string.IsNullOrEmpty(text))
				{
					throw new ArgumentException("text");
				}
				this.Tokenize = tokenize;
				this.Text = text;
			}

			// Token: 0x17000D40 RID: 3392
			// (get) Token: 0x0600351D RID: 13597 RVA: 0x0010B46D File Offset: 0x0010966D
			public LocationStringParser.LocationTokenize Tokenize { get; }

			// Token: 0x17000D41 RID: 3393
			// (get) Token: 0x0600351E RID: 13598 RVA: 0x0010B475 File Offset: 0x00109675
			public string Text { get; }

			// Token: 0x0600351F RID: 13599 RVA: 0x0010B47D File Offset: 0x0010967D
			public static LocationStringParser.LocationToken CreateString(string text)
			{
				return new LocationStringParser.LocationToken(LocationStringParser.LocationTokenize.String, text);
			}

			// Token: 0x06003520 RID: 13600 RVA: 0x0010B486 File Offset: 0x00109686
			public static LocationStringParser.LocationToken CreatePagePlaceholder(string text)
			{
				return new LocationStringParser.LocationToken(LocationStringParser.LocationTokenize.PagePlaceholder, text);
			}
		}
	}
}
