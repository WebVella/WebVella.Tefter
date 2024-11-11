﻿using System.Collections.Generic;
using WebVella.Tefter.Web.Components;

namespace WebVella.Tefter.Web.Tests.Components;


public class ColumnCardComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var items = new List<string> { "col1", "col2" };
			// Act
			var cut = Context.RenderComponent<TfColumnCard>(parameters => parameters
			.Add(p => p.Items, items)
			);

			// Assert
			var mainWrapper = cut.Find($".tf-card");
		}
	}

}
