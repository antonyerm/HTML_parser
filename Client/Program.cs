using System;
using System.IO;
using TemplaterLib;

namespace Client
{
	internal class Program
	{
		const string baseFilePath = "..\\..\\..\\..\\TestData\\";
		const string templateFilePath = "TemplateFile.html";
		const string dataFilePath = "DataFile.json";
		static void Main(string[] args)
		{
			Console.WriteLine("An example of CreateHtml library method.\n");

			var templater = new Templater();
			var template = File.ReadAllText(Path.Combine(baseFilePath, templateFilePath));
			var data = File.ReadAllText(Path.Combine(baseFilePath, dataFilePath));

			var result = templater.CreateHtml(template, data);

			Console.WriteLine(result);

		}
	}
}
