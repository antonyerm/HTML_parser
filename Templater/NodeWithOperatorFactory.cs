using HtmlAgilityPack;
using System;

namespace TemplaterLib
{
	internal static class NodeWithOperatorFactory
	{
		/// <summary>
		/// Creates a node with a specific delegate which will perform the operator function.
		/// </summary>
		/// <param name="node">Node from the input template which has a function in it.</param>
		/// <param name="operatorName">Operator name which was parsed from the <paramref name="node"/></param>
		/// <param name="data">Input data.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
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
