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
			output = TfConverters.GenerateDbNameFromText(input);
			output.Should().Be(input);
		}
		{ 
			input = "DEVIATION REGARDING REQUESTED CUSTOMER DELIVERY";
			output = TfConverters.GenerateDbNameFromText(input);
			output.Should().Be("deviation_regarding_requested_customer_delivery");
		}
		{ 
			input = "Customer PO №";
			output = TfConverters.GenerateDbNameFromText(input);
			output.Should().Be("customer_po_no");
		}
		{ 
			input = "Bike model/description";
			output = TfConverters.GenerateDbNameFromText(input);
			output.Should().Be("bike_model_description");
		}
		{ 
			input = "Item No.";
			output = TfConverters.GenerateDbNameFromText(input);
			output.Should().Be("item_no_");
		}
		{ 
			input = "Vendor Item №";
			output = TfConverters.GenerateDbNameFromText(input);
			output.Should().Be("vendor_item_no");
		}
		{ 
			input = "Secured/Not  secured - missing OC";
			output = TfConverters.GenerateDbNameFromText(input);
			output.Should().Be("secured_not_secured_missing_oc");
		}
	}


}
