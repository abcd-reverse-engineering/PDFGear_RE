using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDFKit.Utils.XObjects;

namespace pdfeditor.Controls.PageHeaderFooters
{
	// Token: 0x02000242 RID: 578
	public class PagePlaceholderFormatter
	{
		// Token: 0x17000AF0 RID: 2800
		// (get) Token: 0x060020FD RID: 8445 RVA: 0x000975A8 File Offset: 0x000957A8
		public static IReadOnlyList<string> AllSupportedPageNumberFormats
		{
			get
			{
				if (PagePlaceholderFormatter.allSupportedPageNumberFormats == null)
				{
					object locker = PagePlaceholderFormatter._locker;
					lock (locker)
					{
						if (PagePlaceholderFormatter.allSupportedPageNumberFormats == null)
						{
							PagePlaceholderFormatter.allSupportedPageNumberFormats = new string[] { "1", "1 - n", "1/n", "1 of n", "Page 1", "Page 1 of n" };
						}
					}
				}
				return PagePlaceholderFormatter.allSupportedPageNumberFormats;
			}
		}

		// Token: 0x17000AF1 RID: 2801
		// (get) Token: 0x060020FE RID: 8446 RVA: 0x00097630 File Offset: 0x00095830
		public static IReadOnlyList<string> AllSupportedDateFormats
		{
			get
			{
				if (PagePlaceholderFormatter.allSupportedDateFormats == null)
				{
					object locker = PagePlaceholderFormatter._locker;
					lock (locker)
					{
						if (PagePlaceholderFormatter.allSupportedDateFormats == null)
						{
							PagePlaceholderFormatter.allSupportedDateFormats = new string[]
							{
								"m/d", "m/d/yy", "m/d/yyyy", "mm/dd/yy", "mm/dd/yyyy", "d/m/yy", "d/m/yyyy", "dd/mm/yy", "dd/mm/yyyy", "mm/yyyy",
								"m.d.yyyy", "mm.dd.yyyy", "mm.yy", "mm.yyyy", "d.m.yy", "d.m.yyyy", "yy-mm-dd", "yyyy-mm-dd"
							};
						}
					}
				}
				return PagePlaceholderFormatter.allSupportedDateFormats;
			}
		}

		// Token: 0x060020FF RID: 8447 RVA: 0x00097728 File Offset: 0x00095928
		public static string DateModelToString(HeaderFooterSettings.DateModel model)
		{
			if (model == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<<");
			stringBuilder.Append(PagePlaceholderFormatter.DateModelToPlaceholder(model));
			stringBuilder.Append(">>");
			return stringBuilder.ToString();
		}

		// Token: 0x06002100 RID: 8448 RVA: 0x00097764 File Offset: 0x00095964
		public static HeaderFooterSettings.DateModel StringToDateModel(string str)
		{
			LocationStringParser.LocationToken[] array = LocationStringParser.GetTokens(str).ToArray<LocationStringParser.LocationToken>();
			if (array.Length != 1)
			{
				return null;
			}
			return PagePlaceholderFormatter.GetDateModel(array[0].Text.Substring(2, array[0].Text.Length - 4).Trim());
		}

		// Token: 0x06002101 RID: 8449 RVA: 0x000977B4 File Offset: 0x000959B4
		public static string PageModelToString(HeaderFooterSettings.PageModel model)
		{
			if (model == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<<");
			stringBuilder.Append(PagePlaceholderFormatter.PageModelToPlaceholder(model));
			stringBuilder.Append(">>");
			return stringBuilder.ToString();
		}

		// Token: 0x06002102 RID: 8450 RVA: 0x000977F0 File Offset: 0x000959F0
		public static HeaderFooterSettings.PageModel StringToPageModel(string str, int pageOffset)
		{
			LocationStringParser.LocationToken[] array = LocationStringParser.GetTokens(str).ToArray<LocationStringParser.LocationToken>();
			if (array.Length != 1)
			{
				return null;
			}
			return PagePlaceholderFormatter.GetPageModel(array[0].Text.Substring(2, array[0].Text.Length - 4).Trim(), pageOffset);
		}

		// Token: 0x06002103 RID: 8451 RVA: 0x00097844 File Offset: 0x00095A44
		public static string LocationToString(HeaderFooterSettings.LocationModel model)
		{
			if (model == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in model)
			{
				HeaderFooterSettings.PageModel pageModel = obj as HeaderFooterSettings.PageModel;
				if (pageModel != null)
				{
					stringBuilder.Append(PagePlaceholderFormatter.PageModelToString(pageModel));
				}
				else
				{
					HeaderFooterSettings.DateModel dateModel = obj as HeaderFooterSettings.DateModel;
					if (dateModel != null)
					{
						stringBuilder.Append(PagePlaceholderFormatter.DateModelToString(dateModel));
					}
					else
					{
						string text = obj as string;
						if (text != null)
						{
							stringBuilder.Append(text);
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06002104 RID: 8452 RVA: 0x000978E4 File Offset: 0x00095AE4
		public static void StringToLocation(HeaderFooterSettings.LocationModel location, string str, int pageOffset)
		{
			location.Clear();
			foreach (LocationStringParser.LocationToken locationToken in LocationStringParser.GetTokens(str))
			{
				if (locationToken.Tokenize == LocationStringParser.LocationTokenize.String)
				{
					location.Add(locationToken.Text);
				}
				else if (locationToken.Tokenize == LocationStringParser.LocationTokenize.PagePlaceholder)
				{
					string text = locationToken.Text.Substring(2, locationToken.Text.Length - 4).Trim();
					HeaderFooterSettings.DateModel dateModel = PagePlaceholderFormatter.GetDateModel(text);
					if (dateModel != null)
					{
						location.Add(dateModel);
					}
					else
					{
						HeaderFooterSettings.PageModel pageModel = PagePlaceholderFormatter.GetPageModel(text, pageOffset);
						if (pageModel != null)
						{
							location.Add(pageModel);
						}
						else
						{
							location.Add(locationToken.Text);
						}
					}
				}
			}
		}

		// Token: 0x06002105 RID: 8453 RVA: 0x000979B0 File Offset: 0x00095BB0
		private static HeaderFooterSettings.PageModel GetPageModel(string placeholder, int pageOffset)
		{
			string text = placeholder.Trim();
			HeaderFooterSettings.PageModel pageModel = null;
			if (!(text == "1"))
			{
				if (!(text == "1 - n"))
				{
					if (!(text == "1/n"))
					{
						if (!(text == "1 of n"))
						{
							if (!(text == "Page 1"))
							{
								if (text == "Page 1 of n")
								{
									pageModel = new HeaderFooterSettings.PageModel
									{
										"Page",
										new HeaderFooterSettings.VariableModel("PageIndex")
										{
											Format = "1"
										},
										"of",
										new HeaderFooterSettings.VariableModel("PageTotalNum")
										{
											Format = "n"
										}
									};
								}
							}
							else
							{
								pageModel = new HeaderFooterSettings.PageModel
								{
									"Page",
									new HeaderFooterSettings.VariableModel("PageIndex")
									{
										Format = "1"
									}
								};
							}
						}
						else
						{
							pageModel = new HeaderFooterSettings.PageModel
							{
								new HeaderFooterSettings.VariableModel("PageIndex")
								{
									Format = "1"
								},
								"of",
								new HeaderFooterSettings.VariableModel("PageTotalNum")
								{
									Format = "n"
								}
							};
						}
					}
					else
					{
						pageModel = new HeaderFooterSettings.PageModel
						{
							new HeaderFooterSettings.VariableModel("PageIndex")
							{
								Format = "1"
							},
							"/",
							new HeaderFooterSettings.VariableModel("PageTotalNum")
							{
								Format = "n"
							}
						};
					}
				}
				else
				{
					pageModel = new HeaderFooterSettings.PageModel
					{
						new HeaderFooterSettings.VariableModel("PageIndex")
						{
							Format = "1"
						},
						" - ",
						new HeaderFooterSettings.VariableModel("PageTotalNum")
						{
							Format = "n"
						}
					};
				}
			}
			else
			{
				pageModel = new HeaderFooterSettings.PageModel
				{
					new HeaderFooterSettings.VariableModel("PageIndex")
					{
						Format = "1"
					}
				};
			}
			if (pageModel != null)
			{
				pageModel.Offset = Math.Max(0, pageOffset - 1);
			}
			return pageModel;
		}

		// Token: 0x06002106 RID: 8454 RVA: 0x00097BC4 File Offset: 0x00095DC4
		private static HeaderFooterSettings.DateModel GetDateModel(string placeholder)
		{
			HashSet<char> hash = new HashSet<char> { 'd', 'm', 'y', '.', '/', '-' };
			if (placeholder.Any((char c) => !hash.Contains(c)))
			{
				return null;
			}
			char c3 = '\0';
			int num = 0;
			HeaderFooterSettings.DateModel dateModel = new HeaderFooterSettings.DateModel();
			for (int i = 0; i <= placeholder.Length; i++)
			{
				char c2 = '\0';
				bool flag;
				if (i < placeholder.Length)
				{
					c2 = placeholder[i];
					flag = c3 != '\0' && c2 != c3;
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					if (c3 == 'd')
					{
						dateModel.Add(new HeaderFooterSettings.VariableModel("Day")
						{
							Format = string.Format("{0}", num)
						});
					}
					else if (c3 == 'm')
					{
						dateModel.Add(new HeaderFooterSettings.VariableModel("Month")
						{
							Format = string.Format("{0}", num)
						});
					}
					else if (c3 == 'y')
					{
						dateModel.Add(new HeaderFooterSettings.VariableModel("Year")
						{
							Format = string.Format("{0}", num)
						});
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder(num);
						for (int j = 0; j < num; j++)
						{
							stringBuilder.Append(c3);
						}
						dateModel.Add(stringBuilder.ToString());
					}
					num = 0;
				}
				c3 = c2;
				num++;
			}
			return dateModel;
		}

		// Token: 0x06002107 RID: 8455 RVA: 0x00097D50 File Offset: 0x00095F50
		private static string PageModelToPlaceholder(HeaderFooterSettings.PageModel page)
		{
			if (page == null)
			{
				return PagePlaceholderFormatter.AllSupportedPageNumberFormats[0];
			}
			string text = PagePlaceholderFormatter.PageModelToPlaceholderCore(page);
			if (string.IsNullOrEmpty(text))
			{
				return PagePlaceholderFormatter.AllSupportedPageNumberFormats[0];
			}
			for (int i = 0; i < PagePlaceholderFormatter.AllSupportedPageNumberFormats.Count; i++)
			{
				if (PagePlaceholderFormatter.AllSupportedPageNumberFormats[i] == text)
				{
					return PagePlaceholderFormatter.AllSupportedPageNumberFormats[i];
				}
			}
			return PagePlaceholderFormatter.AllSupportedPageNumberFormats[0];
		}

		// Token: 0x06002108 RID: 8456 RVA: 0x00097DC8 File Offset: 0x00095FC8
		private static string DateModelToPlaceholder(HeaderFooterSettings.DateModel date)
		{
			if (date == null)
			{
				return PagePlaceholderFormatter.AllSupportedDateFormats[0];
			}
			string text = PagePlaceholderFormatter.DateModelToPlaceholderCore(date);
			if (string.IsNullOrEmpty(text))
			{
				return PagePlaceholderFormatter.AllSupportedDateFormats[0];
			}
			for (int i = 0; i < PagePlaceholderFormatter.AllSupportedDateFormats.Count; i++)
			{
				if (PagePlaceholderFormatter.AllSupportedDateFormats[i] == text)
				{
					return PagePlaceholderFormatter.AllSupportedDateFormats[i];
				}
			}
			return PagePlaceholderFormatter.AllSupportedDateFormats[0];
		}

		// Token: 0x06002109 RID: 8457 RVA: 0x00097E40 File Offset: 0x00096040
		private static string PageModelToPlaceholderCore(HeaderFooterSettings.PageModel page)
		{
			if (page == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in page)
			{
				string text = obj as string;
				if (text != null)
				{
					stringBuilder.Append(text);
				}
				else
				{
					HeaderFooterSettings.VariableModel variableModel = obj as HeaderFooterSettings.VariableModel;
					if (variableModel != null)
					{
						if (variableModel.Name == "PageIndex")
						{
							stringBuilder.Append('1');
						}
						else if (variableModel.Name == "PageTotalNum")
						{
							stringBuilder.Append('n');
						}
					}
				}
			}
			string text2 = stringBuilder.ToString().Trim();
			if (text2 == "1ofn")
			{
				text2 = "1 of n";
			}
			if (text2 == "Page1")
			{
				text2 = "Page 1";
			}
			if (text2 == "Page1ofn")
			{
				text2 = "Page 1 of n";
			}
			return text2;
		}

		// Token: 0x0600210A RID: 8458 RVA: 0x00097F34 File Offset: 0x00096134
		private static string DateModelToPlaceholderCore(HeaderFooterSettings.DateModel date)
		{
			if (date == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in date)
			{
				string text = obj as string;
				if (text != null)
				{
					stringBuilder.Append(text);
				}
				else
				{
					HeaderFooterSettings.VariableModel variableModel = obj as HeaderFooterSettings.VariableModel;
					if (variableModel != null)
					{
						int num;
						int num2;
						int num3;
						if (variableModel.Name == "Day" && int.TryParse(variableModel.Format, out num))
						{
							for (int i = 0; i < num; i++)
							{
								stringBuilder.Append('d');
							}
						}
						else if (variableModel.Name == "Month" && int.TryParse(variableModel.Format, out num2))
						{
							for (int j = 0; j < num2; j++)
							{
								stringBuilder.Append('m');
							}
						}
						else if (variableModel.Name == "Year" && int.TryParse(variableModel.Format, out num3))
						{
							for (int k = 0; k < num3; k++)
							{
								stringBuilder.Append('y');
							}
						}
					}
				}
			}
			return stringBuilder.ToString().Trim();
		}

		// Token: 0x04000D71 RID: 3441
		private static object _locker = new object();

		// Token: 0x04000D72 RID: 3442
		private static IReadOnlyList<string> allSupportedPageNumberFormats;

		// Token: 0x04000D73 RID: 3443
		private static IReadOnlyList<string> allSupportedDateFormats;
	}
}
