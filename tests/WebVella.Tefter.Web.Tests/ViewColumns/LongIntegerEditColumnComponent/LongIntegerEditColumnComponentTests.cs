﻿namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class LongIntegerEditColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfLongIntegerEditColumnComponent>(args => args
		.Add(x => x.RegionContext, new TfSpaceViewColumnScreenRegionContext())
		);

		// Assert
		cut.Find("fluent-number-field");

		Context.DisposeComponents();
	}

}
