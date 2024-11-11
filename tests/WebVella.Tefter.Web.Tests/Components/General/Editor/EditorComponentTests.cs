using System;
using System.Globalization;
using WebVella.Tefter.Web.Components;

namespace WebVella.Tefter.Web.Tests.Components;


public class EditorComponentTests : BaseTest
{

	[Fact]
	public async Task SidebarToggleComponentRendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var userId = Guid.NewGuid();

			// Act
			var cut = Context.RenderComponent<TfEditor>();

			// Assert
			cut.Find(".tf-editor");
		}
	}

}
