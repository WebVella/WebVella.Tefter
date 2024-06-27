using System;
using System.Globalization;
using WebVella.Tefter.Web.Components.AppBar;

namespace WebVella.Tefter.Web.Tests.Components.SidebarToggle;


public class AppBarComponentTests : BaseTest
{

	[Fact]
	public void AppBarComponentRendersCorrectly()
	{
		//Given
		var userId = Guid.NewGuid();

		// Act
		var cut = Context.RenderComponent<TfAppBar>();

		// Assert
		cut.Find(".tf-appbar");
	}

}
