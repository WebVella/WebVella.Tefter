using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Web.Addons.Recipes;
public class TestRecipeAddon : ITfRecipeAddon
{
	public Guid Id { get; init; } = new Guid("958b007c-e377-4440-975e-48952c030a30");
	public string Name { get; init; } = "General Test";
	public string Description { get; init; } = "start with a blank configuration without any presets";
	public string FluentIconName { get; init; } = "Wrench";
	public int SortIndex { get; init; } = 1;
	public string Author { get; init; } = "WebVella";
	public string Website { get; init; } = "https://tefter.webvella.com";
	public List<TfRecipeStepBase> Steps { get; init; } = new();

	public TestRecipeAddon()
	{
		Steps.Add(new TfCreateUserRecipeStep
		{
			StepId = new Guid("52a05dfd-177f-4c12-a068-b0843ff95fdc"),
			StepName = "Create administrator",
			UserEmail = "admin@webvella.com",
			UserPassword = "@adminPassword",
			UserRoles = new List<Guid> { TfConstants.ADMIN_ROLE_ID}
		});
	}

}
