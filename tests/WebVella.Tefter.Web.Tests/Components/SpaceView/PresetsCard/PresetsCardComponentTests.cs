﻿namespace WebVella.Tefter.Web.Tests.Components;

public class PresetsCardComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfPresetsCard>();

		// Assert
		cut.Find(".tf-card");

		Context.DisposeComponents();
	}

}
