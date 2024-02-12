using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TemplaterLib
{
	internal class InputDataModel
	{
        public List<Product> Products { get; set; }
    }

	internal class Product
	{
		public string Name { get; set; }

		public decimal? Price { get; set; }

		public string Description { get; set; }

    } 
}