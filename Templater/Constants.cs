using System;
using System.Collections.Generic;
using System.Text;

namespace TemplaterLib
{
	internal static class Constants
	{
		public const string DefaultDescription = "default description";
		public const decimal DefaultPrice = 5.5m;

		public const string StartFunctionTag = "{%";
		public const string EndFunctionTag = "%}";
		public const string StartValueTag = "{{";
		public const string EndValueTag = "}}";
		public const string LogicalOr = "|";

		public const string For = "for";
		public const string In = "in";
		public const string EndFor = "endfor";
	}
}
