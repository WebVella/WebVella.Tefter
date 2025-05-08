//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WebVella.Tefter.Recipes.Addons.Recipes;
//public class InventoryCollaborationRecipeAddon : ITfRecipeAddon
//{
//	public Guid Id { get; init; } = new Guid("e9cc40f6-ca16-4a95-b618-06eac5b49a9c");
//	public string Name { get; init; } = "Inventory Collaboration";
//	public string Description { get; init; } = "start with a blank configuration without any presets";
//	public string FluentIconName { get; init; } = "BoxMultiple";
//	public int SortIndex { get; init; } = 1;
//	public string Author { get; init; } = "WebVella";
//	public string Website { get; init; } = "https://tefter.webvella.com";
//	public List<TfRecipeStepBase> Steps { get; init; } = new();

//	public InventoryCollaborationRecipeAddon()
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
