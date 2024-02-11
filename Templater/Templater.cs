using System;

namespace TemplaterLib
{
	public class Templater
	{
		const string defaultDescription = "default description";
		const decimal defaultPrice = 5.5m;

		public string CreateHtml(string template, string jsonData)
		{
			var result = new HtmlBuilder()
				   .WithTemplate(template)
				   .WithData(jsonData)
				   .WithDefaultDescription(defaultDescription)
				   .WithDefaultPrice(defaultPrice)
				   .Build();

			return result;
		}
	}
}
