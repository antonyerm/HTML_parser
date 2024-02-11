using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TemplaterLib
{
	internal static class NodeWithOperatorFactory
	{
		private const string ForOperator = "for";
		public static INodeWithOperator Create(HtmlNode node, string operatorName, TemplateDataModel data)
		{
			switch (operatorName.ToLower())
			{
				case ForOperator:
					return new NodeWithOperatorFor(node, operatorName, data);
				default:
					throw new ArgumentException("Invalid operator");
			}
		}
	}
}
