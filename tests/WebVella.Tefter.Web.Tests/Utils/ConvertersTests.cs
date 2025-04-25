using FluentAssertions;
using System;
using WebVella.Tefter.Web.Store;
using WebVella.Tefter.Web.Utils;

namespace WebVella.Tefter.Web.Tests.Utils;
public class ConvertersTests
{
	[Fact]
	public void GenerateDbNameFromTextTests()
	{
		//Given
		var input = "";
		var output = "";

		//standard should be returned
		{ 
			input = "number_one";
			output = input.GenerateDbNameFromText();
			output.Should().Be(input);
		}
		{ 
			input = "DEVIATION REGARDING REQUESTED CUSTOMER DELIVERY";
			output = input.GenerateDbNameFromText();
			output.Should().Be("deviation_regarding_requested_customer_delivery");
		}
		{ 
			input = "Customer PO №";
			output = input.GenerateDbNameFromText();
			output.Should().Be("customer_po_no");
		}
		{ 
			input = "Bike model/description";
			output = input.GenerateDbNameFromText();
			output.Should().Be("bike_model_description");
		}
		{ 
			input = "Item No.";
			output = input.GenerateDbNameFromText();
			output.Should().Be("item_no_");
		}
		{ 
			input = "Vendor Item №";
			output = input.GenerateDbNameFromText();
			output.Should().Be("vendor_item_no");
		}
		{ 
			input = "Secured/Not  secured - missing OC";
			output = input.GenerateDbNameFromText();
			output.Should().Be("secured_not_secured_missing_oc");
		}
	}


}
