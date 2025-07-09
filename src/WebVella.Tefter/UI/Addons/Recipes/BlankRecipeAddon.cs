﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.Tefter.UI.Addons.RecipeSteps;

namespace WebVella.Tefter.UI.Addons.Recipes;
public class BlankRecipeAddon : ITfRecipeAddon
{
	public Guid AddonId { get; init; } = new Guid("de692b3f-be7a-4ace-a1d5-4cdfcbf50d12");
	public string AddonName { get; init; } = "Blank";
	public string AddonDescription { get; init; } = "start with a blank configuration without any presets";
	public string AddonFluentIconName { get; init; } = "Database";
	public int SortIndex { get; init; } = 1;
	public string Author { get; init; } = "WebVella";
	public string Website { get; init; } = "https://tefter.webvella.com";
	public List<ITfRecipeStepAddon> Steps { get; init; } = new();

	public BlankRecipeAddon()
	{
		Steps.Add(new TfInfoRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = true,
				StepId = new Guid("7593cd72-90fb-4278-9b3f-65f4afee7b79"),
				StepMenuTitle = "Get Started",
				StepContentTitle = "Get Started",
				StepContentDescription = null,
			},
			Data = new TfInfoRecipeStepData
			{
				HtmlContent = "<p>This recipe will not preconfigure any items except setting up the System administrator account</p><p>To Start with the step configuration, press the <strong>Forward button</strong></p>",
			}
		});
		Steps.Add(new TfCreateUserRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = true,
				StepId = new Guid("52a05dfd-177f-4c12-a068-b0843ff95fdc"),
				StepMenuTitle = "System Administrator",
				StepContentTitle = "Create administrative account",
				StepContentDescription = "This user will be designated as the system superuser, automatically assigned the administrator role, and granted access to all system areas.",
			},
			Data = new TfCreateUserRecipeStepData
			{
				Email = "",
				Password = "",
				FirstName = "System",
				LastName = "Administrator",
				Roles = new List<Guid> { TfConstants.ADMIN_ROLE_ID }
			}
		});
	}

}
