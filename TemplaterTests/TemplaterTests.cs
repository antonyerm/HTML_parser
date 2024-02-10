using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TemplaterLib;

namespace TemplaterTests
{
	[TestClass]
	public class TemplaterTests
	{
		const string baseFilePath = "..\\..\\..\\..\\TestData\\";
		const string templateFilePath = "TemplateFile.html";
		const string dataFilePath = "DataFile.json";
		const string expectedFilePath = "ExpectedFile.html";

		[TestMethod]
		public void CreateHtml_WithProposedTemplateAndData_ReturnsExpectedFile()
		{
			var templater = new Templater();
			var template = File.ReadAllText(Path.Combine(baseFilePath, templateFilePath));
			var data = File.ReadAllText(Path.Combine(baseFilePath, dataFilePath));
			var expectedResult = File.ReadAllText(Path.Combine(baseFilePath, expectedFilePath));
	
			var actualResult = templater.CreateHtml(template, data);

			Assert.AreEqual(expectedResult, actualResult);
		}
	}
}
