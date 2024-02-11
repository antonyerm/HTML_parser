using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace TemplaterLib
{
	internal interface INodeWithOperator
	{
		TemplateDataModel Data { get; }

		HtmlNode Node { get; }

		Func<HtmlNodeCollection> ExecuteOperator { get; }
	}
}
