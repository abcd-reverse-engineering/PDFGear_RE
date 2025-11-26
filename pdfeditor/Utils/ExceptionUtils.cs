using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace pdfeditor.Utils
{
	// Token: 0x0200007B RID: 123
	public static class ExceptionUtils
	{
		// Token: 0x060008D5 RID: 2261 RVA: 0x0002C01C File Offset: 0x0002A21C
		public static string CreateUnhandledExceptionMessage(this Exception exception)
		{
			ExceptionUtils.<>c__DisplayClass0_0 CS$<>8__locals1;
			CS$<>8__locals1.exception = exception;
			if (CS$<>8__locals1.exception == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Time: ").AppendLine(DateTime.Now.ToString()).Append("Message: ")
				.AppendLine(CS$<>8__locals1.exception.Message ?? "")
				.Append("StackTrace: ")
				.AppendLine(ExceptionUtils.<CreateUnhandledExceptionMessage>g__GetStackTrace|0_2(CS$<>8__locals1.exception, ref CS$<>8__locals1))
				.Append("ExceptionType: ")
				.AppendLine(CS$<>8__locals1.exception.GetType().FullName)
				.Append("Exception: ")
				.AppendLine(CS$<>8__locals1.exception.ToString());
			foreach (Exception ex in ExceptionUtils.<CreateUnhandledExceptionMessage>g__ExpandException|0_0(CS$<>8__locals1.exception))
			{
				stringBuilder.Append('\n').AppendLine("InnerException: ").Append("\tMessage: ")
					.AppendLine(ex.Message ?? "")
					.Append("\tStackTrace: ")
					.AppendLine(ExceptionUtils.<CreateUnhandledExceptionMessage>g__GetStackTrace|0_2(ex, ref CS$<>8__locals1))
					.Append("\tExceptionType: ")
					.AppendLine(ex.GetType().FullName)
					.Append("\tException: ")
					.AppendLine(ex.ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x0002C1A4 File Offset: 0x0002A3A4
		[CompilerGenerated]
		internal static IReadOnlyCollection<Exception> <CreateUnhandledExceptionMessage>g__ExpandException|0_0(Exception ex)
		{
			if (ex.InnerException != null || ex is AggregateException)
			{
				List<Exception> list = new List<Exception>();
				ExceptionUtils.<CreateUnhandledExceptionMessage>g__ExpandExceptionCore|0_1(list, ex);
				return list;
			}
			return Array.Empty<Exception>();
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x0002C1C8 File Offset: 0x0002A3C8
		[CompilerGenerated]
		internal static void <CreateUnhandledExceptionMessage>g__ExpandExceptionCore|0_1(List<Exception> list, Exception ex)
		{
			if (ex == null)
			{
				return;
			}
			if (ex.InnerException != null)
			{
				list.Add(ex.InnerException);
				ExceptionUtils.<CreateUnhandledExceptionMessage>g__ExpandExceptionCore|0_1(list, ex.InnerException);
			}
			AggregateException ex2 = ex as AggregateException;
			if (ex2 != null)
			{
				foreach (Exception ex3 in ex2.InnerExceptions)
				{
					list.Add(ex3);
					ExceptionUtils.<CreateUnhandledExceptionMessage>g__ExpandExceptionCore|0_1(list, ex3);
				}
			}
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x0002C24C File Offset: 0x0002A44C
		[CompilerGenerated]
		internal static string <CreateUnhandledExceptionMessage>g__GetStackTrace|0_2(Exception ex, ref ExceptionUtils.<>c__DisplayClass0_0 A_1)
		{
			if (ex.Data != null && ex.Data.Contains("OriginalStackTrace"))
			{
				string text = ex.Data["OriginalStackTrace"] as string;
				if (text != null)
				{
					return text;
				}
			}
			return A_1.exception.StackTrace ?? "";
		}
	}
}
