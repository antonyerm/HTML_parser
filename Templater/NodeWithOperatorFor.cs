using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Xsl;

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
			var iterationVariable = words.SkipWhile(x => x.ToLower() != Constants.For)?.ElementAtOrDefault(1);
			var collectionName = words.SkipWhile(x => x.ToLower() != Constants.In).ElementAtOrDefault(1);
			
			var validationMessage = GetOperatorValidationResult(collectionName);
			if (!String.IsNullOrEmpty(validationMessage))
			{
				throw new ArgumentException(validationMessage);
			}

			// Get collection object by its name
			var collectionObjectInfo = typeof(InputDataModel).GetProperty(collectionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
			var collectionObject = (IEnumerable)collectionObjectInfo.GetValue(Data.InputData);

			foreach (var item in collectionObject)
			{
				var newNode = Node.CloneNode(true);
				var textNodes = newNode.DescendantsAndSelf().Where(x => x.NodeType == HtmlNodeType.Text);
				foreach (var textNode in textNodes)
				{
					var newText = Regex.Replace(textNode.InnerHtml, @"{{.+?\..+?}}", m => ReplaceTags(m, iterationVariable, item));
					textNode.InnerHtml = newText;
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

			var typesInDataModel = typeof(InputDataModel).GetProperties();
			if (!typesInDataModel.Any(x => x.Name.Equals(collectionName, StringComparison.OrdinalIgnoreCase)))
			{
				return $"Error. The collection \"{collectionName}\" was not found in input data.";
			}

			return null;
		}

		private string ReplaceTags(Match match, string iterationVariable, object item)
		{
			var valueExpression = match.ToString().TrimValueTags().Trim().Split(' ');
			var finalValue = String.Empty;

			foreach (var name in valueExpression)
			{
				var itemNameFirstPart = name.Split('.')[0];

				// Refactor:
				// if LogicalOr then continue;
				// finalValue = TryGetValueFromCollection;
				// if (finalValue == null)
				// { finalValue = GetValueFromAdditionalParameters }
				// if (finalValue == null) 
				// { throw "unrecogizable"

				if (itemNameFirstPart.Equals(iterationVariable, StringComparison.OrdinalIgnoreCase))
				{
					itemNameFirstPart = name.Split(".")[1];
					finalValue = item.GetType().GetProperty(itemNameFirstPart, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).GetValue(item).ToString();
				}
				// else SEARCH AMONG ADDITIONAL PARAMS: if (itemNameFirstPart.Equals(Any additional parameter, StringComparison.OrdinalIgnoreCase))
				//  { finalValue = take additional parameter }



				else if (itemNameFirstPart == Constants.LogicalOr)
				{
					break;
				}
				else if (name.Contains("."))
				{
					throw new ArgumentException($"The iteration variable name \"{iterationVariable}\" does not match the template item name \"{name}\". Template line {Node.Line}.");
				}

				if (!String.IsNullOrEmpty(finalValue))
				{
					break;
				}
			}

			return finalValue;
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