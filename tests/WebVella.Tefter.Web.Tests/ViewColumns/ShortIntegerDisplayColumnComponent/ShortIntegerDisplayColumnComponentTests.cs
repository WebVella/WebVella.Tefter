﻿namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class ShortIntegerDisplayColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfShortIntegerDisplayColumnComponent>(args => args
		.Add(x => x.Context, new TucViewColumnComponentContext())
		);

		// Assert
		cut.Markup.Should().Be("<div></div>");

		Context.DisposeComponents();
	}

}
