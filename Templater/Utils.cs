using System;
using System.Collections.Generic;
using System.Text;

namespace TemplaterLib
{
	internal static class StringExtensions
	{
		public static string TrimOperatorTags(this string s)
		{
			if (s.StartsWith(Constants.StartFunctionTag, StringComparison.OrdinalIgnoreCase))
			{
				s = s.Substring(Constants.StartFunctionTag.Length);
			}

			if (s.EndsWith(Constants.EndFunctionTag, StringComparison.OrdinalIgnoreCase))
			{
				s = s.Substring(0, s.Length - Constants.StartFunctionTag.Length);
			}

			return s;
		}

		public static string TrimValueTags(this string s)
		{
			if (s.StartsWith(Constants.StartValueTag, StringComparison.OrdinalIgnoreCase))
			{
				s = s.Substring(Constants.StartValueTag.Length);
			}

			if (s.EndsWith(Constants.EndValueTag, StringComparison.OrdinalIgnoreCase))
			{
				s = s.Substring(0, s.Length - Constants.EndValueTag.Length);
			}

			return s;
		}
	}
}
