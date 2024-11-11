﻿namespace WebVella.Tefter.Web.Tests.Components;
public class PageLoaderComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfPageLoader>();

			// Assert
			cut.Find(".tf-loader-bar");
		}
	}
}
