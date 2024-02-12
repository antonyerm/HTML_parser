using HtmlAgilityPack;
using System;

namespace TemplaterLib
{
	internal static class NodeWithOperatorFactory
	{
		public static INodeWithOperator Create(HtmlNode node, string operatorName, TemplateDataModel data)
		{
			switch (operatorName.ToLower())
			{
				case Constants.For:
					return new NodeWithOperatorFor(node, data);
				default:
					throw new ArgumentException("Invalid operator");
			}
		}
	}
}
