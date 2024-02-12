using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace TemplaterLib
{
	internal class HtmlBuilder
	{
		private HtmlDocument document;
		private InputDataModel inputData;
		private TemplateDataModel data;
		private string defaultDescription;
		private decimal  defaultPrice;

        public HtmlBuilder()
        {
            this.data = new TemplateDataModel();
        }

        public HtmlBuilder WithTemplate(string template)
		{
			this.document = new HtmlDocument();
			this.document.LoadHtml(template);
			return this;
		}

		public HtmlBuilder WithData(string inputData)
		{
			var options = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true
			};

			this.data.InputData = JsonSerializer.Deserialize<InputDataModel>(inputData, options);
			return this;
		}

		public HtmlBuilder WithDefaultDescription(string defaultDescription)
		{
			this.data.Paragraph = defaultDescription;
			return this;
		}

		public HtmlBuilder WithDefaultPrice(decimal defaultPrice)
		{
			this.data.Price = defaultPrice;
			return this;
		}

		public string Build()
		{
			var nodesWithOperatorDelegate = GetNodesWithAddedOperatorDelegate();
			foreach (var node in nodesWithOperatorDelegate)
			{
				var replacementNodes = node.ExecuteOperator();
				ReplaceNodesWithProcessedOnes(node.Node, replacementNodes);
			}

			return document.DocumentNode.OuterHtml;
		}

		private List<INodeWithOperator> GetNodesWithAddedOperatorDelegate()
		{
			var candidateNodes = document.DocumentNode.DescendantsAndSelf()
				.Where(x => hasSomeFunction(x)).ToList();
			var nodesWithOperatorDelegate = new List<INodeWithOperator>();
			foreach (var node in candidateNodes)
			{
				var operatorName = node.FirstChild.InnerHtml
					.Trim().TrimOperatorTags().Trim()
					.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0].ToLower();
				var nodeWithOperatorDelegate = NodeWithOperatorFactory.Create(node, operatorName, data);
				nodesWithOperatorDelegate.Add(nodeWithOperatorDelegate);
			}
			
			return nodesWithOperatorDelegate;
		}

		private bool hasSomeFunction(HtmlNode x)
		{
			var firstChildText = x.FirstChild?.InnerHtml.Trim();
			var lastChildText = x.LastChild?.InnerHtml.Trim();

			if (firstChildText == null || lastChildText == null)
			{
				return false;
			}

			var isHavingFunctionTagsOnEnds = firstChildText.StartsWith(Constants.StartFunctionTag)
				&& firstChildText.EndsWith(Constants.EndFunctionTag)
				&& lastChildText.StartsWith(Constants.StartFunctionTag)
				&& lastChildText.EndsWith(Constants.EndFunctionTag)
				&& x.FirstChild != x.LastChild
				&& x.FirstChild?.NodeType == HtmlNodeType.Text
				&& x.LastChild?.NodeType == HtmlNodeType.Text;

			return isHavingFunctionTagsOnEnds;
		}

		private void ReplaceNodesWithProcessedOnes(HtmlNode parent, HtmlNodeCollection newChildren)
		{
			parent.RemoveAllChildren();
			parent.AppendChildren(newChildren);
		}
	}
}
