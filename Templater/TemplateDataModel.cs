using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TemplaterLib
{
	internal class TemplateDataModel
	{
        public List<Product> Products { get; set; }
    }

	internal class Product
	{
		public string Name { get; set; }

		public decimal price { get; set; }

		public string Description { get; set; }

    } 
}