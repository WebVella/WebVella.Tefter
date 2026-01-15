//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WebVella.Tefter.Recipes.Addons.Recipes;
//public class LeadManagementRecipeAddon : ITfRecipeAddon
//{
//	public Guid Id { get; init; } = new Guid("4c335956-c929-4eb8-95f9-b61f29f5e848");
//	public string Name { get; init; } = "Lead Management";
//	public string Description { get; init; } = "start with a blank configuration without any presets";
//	public string FluentIconName { get; init; } = "PeopleAdd";
//	public int SortIndex { get; init; } = 1;
//	public string Author { get; init; } = "WebVella";
//	public string Website { get; init; } = "https://tefter.webvella.com";
//	public List<TfRecipeStepBase> Steps { get; init; } = new();

//	public LeadManagementRecipeAddon()
//	{
//		Steps.Add(new TfCreateUserRecipeStep
//		{
//			StepId = new Guid("52a05dfd-177f-4c12-a068-b0843ff95fdc"),
//			StepMenuTitle = "Create administrator",
//			UserEmail = "admin@webvella.com",
//			UserPassword = "@adminPassword",
//			UserRolesHidden = new List<Guid> { TfConstants.ADMIN_ROLE_ID}
//		});
//	}

//}
