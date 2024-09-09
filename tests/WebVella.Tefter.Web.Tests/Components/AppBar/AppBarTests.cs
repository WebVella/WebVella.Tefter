using System;
using WebVella.Tefter.Web.Components;

namespace WebVella.Tefter.Web.Tests.Components;


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
