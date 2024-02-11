using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace TemplaterLib
{
	internal class NodeWithOperatorFor : INodeWithOperator
	{
		public TemplateDataModel Data { get; }

		public HtmlNode Node { get; }

		public Func<HtmlNodeCollection> ExecuteOperator { get; }

		public NodeWithOperatorFor(HtmlNode node, string operatorName, TemplateDataModel data)
		{
			Node = node;
			Data = data;
			ExecuteOperator = For;
		}

		private HtmlNodeCollection For()
		{
			var replacementNodes = new HtmlNodeCollection(Node);
			foreach (var product in Data.Products)
			{
				//replacementNodes.Add(HtmlNode.CreateNode($"<div>New product: {product.Name}</div>"));
				// todo
				// 1. find name of item (product)
				// 2. find name of collection (products)
				// 3. compare name of collection to name of model collection via reflection. If not equal => exception "Unknown collection"
				// 4. replace all item.value in descendants.innerHtml with model-prop.model-value, via reflection
				// 5. remove first child innerHtml up to the point where "%}" ends and \t start, keeping the \t chars
				// 6. remove last child completely
			}

			return replacementNodes;
		}
	}
}
