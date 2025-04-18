﻿namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class SelectEditColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		Context.RenderComponent<FluentMenuProvider>();
		// Act
		var cut = Context.RenderComponent<TfSelectEditColumnComponent>(args => args
		.Add(x => x.RegionContext, new TfSpaceViewColumnScreenRegionContext())
		);

		// Assert
		cut.Find("fluent-button");

		Context.DisposeComponents();
	}

}
