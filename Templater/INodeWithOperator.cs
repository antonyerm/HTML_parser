using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace TemplaterLib
{
	internal interface INodeWithOperator
	{
		HtmlNode Node { get; }

		Func<HtmlNode> ExecuteOperator { get; }

		TemplateDataModel Data { get; }
	}
}
