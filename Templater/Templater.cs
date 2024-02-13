namespace TemplaterLib
{
	public class Templater
	{
		/// <summary>
		/// Library method for converting input template and data into HTML.
		/// </summary>
		/// <param name="template"></param>
		/// <param name="jsonData"></param>
		/// <returns></returns>
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
