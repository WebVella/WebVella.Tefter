﻿namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class EmailDisplayColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfEmailDisplayColumnComponent>(args => args
		.Add(x => x.RegionContext, new TfSpaceViewColumnScreenRegionContext())
		);

		// Assert
		cut.Markup.Should().Be("<div></div>");

		Context.DisposeComponents();
	}

}
