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
		private string collectionName;
		private string iterationVariableName;

		public TemplateDataModel Data { get; }

		public HtmlNode Node { get; }

		public Func<HtmlNode> ExecuteOperator { get; }

		public NodeWithOperatorFor(HtmlNode node, TemplateDataModel data)
		{
			Node = node;
			Data = data;
			ExecuteOperator = For;
		}

		private HtmlNode For()
		{
			var replacementNode = Node.CloneNode(deep: false);
			var cycleDeclaration = Node.FirstChild.InnerHtml.Trim();
			var words = cycleDeclaration.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			iterationVariableName = words.SkipWhile(x => x.ToLower() != Constants.For)?.ElementAtOrDefault(1);
			collectionName = words.SkipWhile(x => x.ToLower() != Constants.In).ElementAtOrDefault(1);
			
			var validationMessage = GetOperatorValidationResult(collectionName);
			if (!String.IsNullOrEmpty(validationMessage))
			{
				throw new ArgumentException(validationMessage);
			}

			// Get collection object by its name
			var collectionObjectInfo = typeof(InputDataModel).GetProperty(collectionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
			var collectionObject = (IList)collectionObjectInfo.GetValue(Data.InputData);

			for (int i = 0; i < collectionObject.Count; i++)
			{
				var item = collectionObject[i];
				var newNode = Node.CloneNode(deep: true);
				var textNodes = newNode.DescendantsAndSelf().Where(x => x.NodeType == HtmlNodeType.Text);
				foreach (var textNode in textNodes)
				{
					var newText = Regex.Replace(textNode.InnerHtml, @"{{.+?}}", m => ReplaceTags(m, item));
					textNode.InnerHtml = newText;
				}

				newNode.FirstChild.InnerHtml = RemoveOperatorLine(newNode.FirstChild.InnerHtml);
				if (i < collectionObject.Count - 1)
				{
					newNode.LastChild.Remove();
				}
				else
				{
					newNode.LastChild.InnerHtml = RemoveOperatorLine(newNode.LastChild.InnerHtml);
				}

				replacementNode.AppendChildren(newNode.ChildNodes);
			};

			return replacementNode;
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
				return $"Inconsistent Template. Collection name in HTML node definition \"{collectionName}\" and in For cycle definition do not match. Template line {Node.Line}";
			}

			var typesInDataModel = typeof(InputDataModel).GetProperties();
			if (!typesInDataModel.Any(x => x.Name.Equals(collectionName, StringComparison.OrdinalIgnoreCase)))
			{
				return $"Error. The collection \"{collectionName}\" was not found in input data.";
			}

			return null;
		}

		private string ReplaceTags(Match match, object item)
		{
			var valueExpression = match.ToString().TrimValueTags().Trim();
			var valueExpressionParts = valueExpression.Split(' ');
			var replacementValue = String.Empty;

			foreach (var name in valueExpressionParts)
			{
				ValidateIterationVariableUsage(name);
				
				if (name == "|")
				{
					continue;
				}

				replacementValue = TryGetValueFromCollection(name, item);
				if (replacementValue != null)
				{
					break;
				}

				replacementValue = TryGetValueFromAdditionalData(name);
				if (replacementValue != null)
				{
					break;
				}
			}

			if (String.IsNullOrEmpty(replacementValue))
			{
				throw new ArgumentException($"Could not recognize template item name \"{valueExpression}\". Template line {Node.Line}.");
			}

			return replacementValue;
		}

		private void ValidateIterationVariableUsage(string name)
		{
			var itemNameFirstPart = name.Split('.')[0];
			if (name.Contains(".") && !itemNameFirstPart.Equals(iterationVariableName, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException($"Possible error. Cycle iteration vairable \"{iterationVariableName}\" does not match item name \"{name}\". Template line {Node.Line}.");
			}
		}

		private string TryGetValueFromCollection(string name, object item)
		{
			string nameSecondPart = GetNameSecondPart(name);
			string result = null;

			if (!string.IsNullOrEmpty(nameSecondPart))
			{
				var collectionProperty = item.GetType().GetProperty(nameSecondPart, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
				
				if (collectionProperty == null)
				{
					throw new ArgumentException($"Could not recognize template item name \"{name}\". Template line {Node.Line}.");
				}

				result = collectionProperty.GetValue(item)?.ToString();
			}

			return result;
		}

		private string TryGetValueFromAdditionalData(string name)
		{
			string result = null;

			if (!string.IsNullOrEmpty(name))
			{
				var additionalDataProperty = Data.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
				result = additionalDataProperty == null? null : additionalDataProperty.GetValue(Data)?.ToString();
			}

			return result;
		}

		private static string GetNameSecondPart(string name)
		{
			var nameSecondPart = String.Empty;
			if (name.Split(".", StringSplitOptions.RemoveEmptyEntries).Count() > 1)
			{
				nameSecondPart = name.Split(".")[1];
			}

			return nameSecondPart;
		}

		private List<HtmlNode> GetPlaceholderNodes(HtmlNode node) =>
			node.DescendantsAndSelf().Where(x => x.NodeType == HtmlNodeType.Text
				&& x.InnerHtml.StartsWith(Constants.StartValueTag)
				&& x.InnerHtml.EndsWith(Constants.EndValueTag)).ToList();

		private string RemoveOperatorLine(string s)
		{
			var result = String.Empty;
			var lineSeparator = new String(s.SkipWhile(x => !"\n\r".Contains(x))
											.TakeWhile(x => "\n\r".Contains(x))
											.ToArray());
			var fragments = s.Split(lineSeparator);

			if (fragments.Count() < 3)
			{
				result = s.Split(Constants.StartValueTag)[0] + s.Split(Constants.EndValueTag)[1];
			}
			else
			{
				result = fragments[0] + lineSeparator + fragments.Last();
			}
			
			return result;
		}
	}
}