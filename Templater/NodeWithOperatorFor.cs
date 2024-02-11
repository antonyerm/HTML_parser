using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

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
			var cycleDeclaration = Node.FirstChild.InnerHtml.Trim();
			var words = cycleDeclaration.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			var itemNameInCycleDeclaration = words.SkipWhile(x => x.ToLower() != Constants.For)?.ElementAtOrDefault(1);
			var collectionName = words.SkipWhile(x => x.ToLower() != Constants.In).ElementAtOrDefault(1);
			
			var validationMessage = GetOperatorValidationResult(collectionName);
			if (!String.IsNullOrEmpty(validationMessage))
			{
				throw new ArgumentException(validationMessage);
			}

			// Get collection object by its name
			var collectionObjectInfo = typeof(TemplateDataModel).GetProperty(collectionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
			var collectionObject = (IEnumerable)collectionObjectInfo.GetValue(Data);

			foreach (var item in collectionObject)
			{
				var newNode = Node.CloneNode(true);
				var valuePlaceholders = GetPlaceholderNodes(newNode);
				foreach (var placeholder in valuePlaceholders)
				{
					var expression = placeholder.InnerHtml.TrimValueTags().Trim();
					var valueNames = expression.Split(' ');
					var finalValue = String.Empty;
					foreach (var name in valueNames)
					{
						if (!(name.Contains(".") && name.Split('.')[0].Equals(itemNameInCycleDeclaration, StringComparison.OrdinalIgnoreCase)))
						{
							throw new ArgumentException($"The iteration item name \"{itemNameInCycleDeclaration}\" does not match the template item name \"{name}\". Template line {Node.Line}.");
						}

						// TODO: if not contains "." then search additional parameters

						var valueName = name.Split(".")[1];
						finalValue = item.GetType().GetProperty(valueName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).GetValue(item).ToString();
					}
					
					placeholder.InnerHtml = finalValue;
				}

				// TODO:modify InnerHtml of first and last children to keep \t and \n at the end and start
				newNode.FirstChild.Remove();
				newNode.LastChild.Remove();
				foreach (var childNode in newNode.ChildNodes)
				{
					replacementNodes.Add(childNode);
				}
			};

			return replacementNodes;
		}
				
		private string GetOperatorValidationResult(string collectionName)
		{
			var cycleBodyClosingLine = Node.LastChild.InnerHtml
				.Trim().TrimOperatorTags().Trim()
				.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
			if (!cycleBodyClosingLine.Equals(Constants.EndFor, StringComparison.OrdinalIgnoreCase))
			{
				return $"Inconsistent Template. Closing line for cycle body is missing. Template line {Node.Line}";
			}

			var collectionNameInNodeId = Node.Id;
			if (!collectionName.Equals(collectionNameInNodeId, StringComparison.OrdinalIgnoreCase))
			{
				return $"Inconsistent Template. Collection name in node definition and in For cycle definition do not match. Template line {Node.Line}";
			}

			var typesInDataModel = typeof(TemplateDataModel).GetProperties();
			if (!typesInDataModel.Any(x => x.Name.Equals(collectionName, StringComparison.OrdinalIgnoreCase)))
			{
				return $"Error. The collection \"{collectionName}\" was not found in input data.";
			}

			return null;
		}

		private List<HtmlNode> GetPlaceholderNodes(HtmlNode node) =>
			node.DescendantsAndSelf().Where(x => x.NodeType == HtmlNodeType.Text
				&& x.InnerHtml.StartsWith(Constants.StartValueTag)
				&& x.InnerHtml.EndsWith(Constants.EndValueTag)).ToList();
	}
}

//replacementNodes.Add(HtmlNode.CreateNode($"<div>New product: {product.Name}</div>"));
// todo
// OK1. find name of item (product)
// OK2. find name of collection (products)
// OK3. compare name of collection to name of model collection via reflection. If not equal => exception "Unknown collection"
// 4. replace all item.value in descendants.innerHtml with model-prop.model-value, via reflection
// 5. remove first child innerHtml up to the point where "%}" ends and \t start, keeping the \t chars
// 6. remove last child completely