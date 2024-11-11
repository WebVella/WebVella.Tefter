using System.Collections.Generic;
using WebVella.Tefter.Web.Components;

namespace WebVella.Tefter.Web.Tests.Components.ColumnCard;


public class ColumnCardComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var items = new List<string> { "col1", "col2" };
		// Act
		var cut = Context.RenderComponent<TfColumnCard>(parameters => parameters
		.Add(p => p.Items, items)
		);

		// Assert
		var mainWrapper = cut.Find($".tf-card");
	}

}
