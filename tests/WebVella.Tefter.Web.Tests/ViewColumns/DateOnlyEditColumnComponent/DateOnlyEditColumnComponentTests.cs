﻿namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class DateOnlyEditColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfDateOnlyEditColumnComponent>(args => args
		.Add(x => x.RegionContext, new TfSpaceViewColumnScreenRegionContext())
		);

		// Assert
		cut.Find("fluent-text-field");

		Context.DisposeComponents();
	}

}
