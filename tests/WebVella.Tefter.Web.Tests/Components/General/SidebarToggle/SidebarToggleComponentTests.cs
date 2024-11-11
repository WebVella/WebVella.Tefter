using System;
using System.Globalization;
using WebVella.Tefter.Web.Components;

namespace WebVella.Tefter.Web.Tests.Components;


public class SidebarToggleComponentTests : BaseTest
{

	[Fact]
	public async Task SidebarToggleComponentRendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var userId = Guid.NewGuid();

			// Act
			var cut = Context.RenderComponent<TfSidebarToggle>();

			// Assert
			cut.Find("fluent-button");
		}
	}

}
