using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
		public void CreateHtml_WithCorrectTemplateAndData_ReturnsExpectedFile()
		{
			var v = 1;
			var templater = new Templater();
			var template = File.ReadAllText(Path.Combine(baseFilePath,
				$"{Path.GetFileNameWithoutExtension(templateFilePath)}{v}{Path.GetExtension(templateFilePath)}"));
			var data = File.ReadAllText(Path.Combine(baseFilePath,
				$"{Path.GetFileNameWithoutExtension(dataFilePath)}{v}{Path.GetExtension(dataFilePath)}"));
			var expectedResult = File.ReadAllText(Path.Combine(baseFilePath,
				$"{Path.GetFileNameWithoutExtension(expectedFilePath)}{v}{Path.GetExtension(expectedFilePath)}"));

			var actualResult = templater.CreateHtml(template, data);

			Assert.AreEqual(expectedResult, actualResult);
		}

		[TestMethod]
		public void CreateHtml_WithDefaultValuesUsed_ReturnsExpectedFile()
		{
			var v = 2;
			var templater = new Templater();
			var template = File.ReadAllText(Path.Combine(baseFilePath,
				$"{Path.GetFileNameWithoutExtension(templateFilePath)}{v}{Path.GetExtension(templateFilePath)}"));
			var data = File.ReadAllText(Path.Combine(baseFilePath, 
				$"{Path.GetFileNameWithoutExtension(dataFilePath)}{v}{Path.GetExtension(dataFilePath)}"));
			var expectedResult = File.ReadAllText(Path.Combine(baseFilePath,
				$"{Path.GetFileNameWithoutExtension(expectedFilePath)}{v}{Path.GetExtension(expectedFilePath)}"));

			var actualResult = templater.CreateHtml(template, data);

			Assert.AreEqual(expectedResult, actualResult);
		}

		[TestMethod]
		public void CreateHtml_WithTemplateOfMultipleBlocks_ReturnsExpectedFile()
		{
			var v = 3;
			var templater = new Templater();
			var template = File.ReadAllText(Path.Combine(baseFilePath,
				$"{Path.GetFileNameWithoutExtension(templateFilePath)}{v}{Path.GetExtension(templateFilePath)}"));
			var data = File.ReadAllText(Path.Combine(baseFilePath,
				$"{Path.GetFileNameWithoutExtension(dataFilePath)}{v}{Path.GetExtension(dataFilePath)}"));
			var expectedResult = File.ReadAllText(Path.Combine(baseFilePath,
				$"{Path.GetFileNameWithoutExtension(expectedFilePath)}{v}{Path.GetExtension(expectedFilePath)}"));

			var actualResult = templater.CreateHtml(template, data);

			Assert.AreEqual(expectedResult, actualResult);
		}

		[TestMethod]
		public void CreateHtml_WithInvalidItemNameInTemplate_ThrowsException()
		{
			var v = 4;
			var templater = new Templater();
			var template = File.ReadAllText(Path.Combine(baseFilePath,
				$"{Path.GetFileNameWithoutExtension(templateFilePath)}{v}{Path.GetExtension(templateFilePath)}"));
			var data = File.ReadAllText(Path.Combine(baseFilePath,
				$"{Path.GetFileNameWithoutExtension(dataFilePath)}{Path.GetExtension(dataFilePath)}"));

			Assert.ThrowsException<ArgumentException>(() => templater.CreateHtml(template, data));
		}

		[TestMethod]
		public void CreateHtml_WithMismatchOfCollectionAndItemName_ThrowsException()
		{
			var v = 4;
			var templater = new Templater();
			var template = File.ReadAllText(Path.Combine(baseFilePath,
				$"{Path.GetFileNameWithoutExtension(templateFilePath)}{v}{Path.GetExtension(templateFilePath)}"));
			var data = File.ReadAllText(Path.Combine(baseFilePath,
				$"{Path.GetFileNameWithoutExtension(dataFilePath)}{Path.GetExtension(dataFilePath)}"));

			Assert.ThrowsException<ArgumentException>(() => templater.CreateHtml(template, data));
		}
	}
}
