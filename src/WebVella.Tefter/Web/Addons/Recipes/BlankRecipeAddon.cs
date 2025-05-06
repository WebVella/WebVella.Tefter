using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Web.Addons.Recipes;
public class BlankRecipeAddon : ITfRecipeAddon
{
	public Guid Id { get; init; } = new Guid("de692b3f-be7a-4ace-a1d5-4cdfcbf50d12");
	public string Name { get; init; } = "Blank";
	public string Description { get; init; } = "start with a blank configuration without any presets";
	public string FluentIconName { get; init; } = "Database";
	public int SortIndex { get; init; } = 1;
	public string Author { get; init; } = "WebVella";
	public string Website { get; init; } = "https://tefter.webvella.com";
	public List<TfRecipeStepBase> Steps { get; init; } = new();

	public BlankRecipeAddon()
	{
		Steps.Add(new TfCreateUserRecipeStep
		{
			StepId = new Guid("52a05dfd-177f-4c12-a068-b0843ff95fdc"),
			StepName = "Create administrator",
			UserEmail = "admin@webvella.com",
			UserPassword = "@adminPassword",
			UserRoles = new List<Guid> { TfConstants.ADMIN_ROLE_ID}
		});
		Steps.Add(new TfCreateUserRecipeStep
		{
			StepId = new Guid("0af8a449-7933-4c52-92ed-619ffe11fa82"),
			StepName = "Data provider",
			UserEmail = "admin@webvella.com",
			UserPassword = "@adminPassword",
			UserRoles = new List<Guid> { TfConstants.ADMIN_ROLE_ID}
		});
	}

}
