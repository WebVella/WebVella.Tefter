﻿namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class TimeEditColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfTimeEditColumnComponent>(args => args
		.Add(x => x.Context, new TfSpaceViewColumnScreenRegion())
		);

		// Assert
		cut.Find("fluent-text-field");

		Context.DisposeComponents();
	}

}
