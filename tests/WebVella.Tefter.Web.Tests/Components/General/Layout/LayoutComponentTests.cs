﻿namespace WebVella.Tefter.Web.Tests.Components;
public class LayoutComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfLayout>();

			// Assert
			cut.Find(".tf-layout");
		}
	}
}
