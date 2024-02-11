using System;

namespace TemplaterLib
{
	public class Templater
	{
		

		public string CreateHtml(string template, string jsonData)
		{
			var result = new HtmlBuilder()
				   .WithTemplate(template)
				   .WithData(jsonData)
				   .WithDefaultDescription(Constants.DefaultDescription)
				   .WithDefaultPrice(Constants.DefaultPrice)
				   .Build();

			return result;
		}
	}
}
