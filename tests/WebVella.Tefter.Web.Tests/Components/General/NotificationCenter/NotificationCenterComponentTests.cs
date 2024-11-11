﻿namespace WebVella.Tefter.Web.Tests.Components;
public class NotificationCenterComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfNotificationCenter>();

			// Assert
			cut.Nodes.Length.Should().BeGreaterThan(0);
		}
	}
}
