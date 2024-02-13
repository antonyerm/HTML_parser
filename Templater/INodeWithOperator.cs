using HtmlAgilityPack;
using System;

namespace TemplaterLib
{
	/// <summary>
	/// Interface can be used for different operators.
	/// </summary>
	internal interface INodeWithOperator
	{
		/// <summary>
		/// Node from the input template which has some operator inside.
		/// </summary>
		HtmlNode Node { get; }

		/// <summary>
		/// Delegate which performs the operator function.
		/// </summary>
		Func<HtmlNode> ExecuteOperator { get; }

		/// <summary>
		/// Data from the input file combined with default values.
		/// </summary>
		TemplateDataModel Data { get; }
	}
}
