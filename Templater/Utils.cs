using System;
using System.Collections.Generic;
using System.Text;

namespace TemplaterLib
{
	internal static class StringExtensions
	{
		public static string TrimOperatorTags(this string s)
		{
			if (s.StartsWith(Constants.StartOperatorTag, StringComparison.OrdinalIgnoreCase))
			{
				s = s.Substring(Constants.StartOperatorTag.Length);
			}

			if (s.EndsWith(Constants.EndOperatorTag, StringComparison.OrdinalIgnoreCase))
			{
				s = s.Substring(0, s.Length - Constants.StartOperatorTag.Length);
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
