using System;
using System.Collections.Generic;
using System.Text;

namespace TemplaterLib
{
	/// <summary>
	/// Model for data which combines the data from the input data file and default values.
	/// </summary>
	internal class TemplateDataModel
	{
		/// <summary>
		/// Data from input data file.
		/// </summary>
		public InputDataModel InputData { get; set; }

		/// <summary>
		/// Default product description.
		/// </summary>
		public string Paragraph{ get; set; }

		/// <summary>
		/// Default product price.
		/// </summary>
		public decimal? Price { get; set; }
	}
}
