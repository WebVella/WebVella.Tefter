namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	ITfRecipeStepAddon GetRecipeStep(Guid id);

	ReadOnlyCollection<ITfRecipeStepAddon> GetRecipeSteps();
}

public partial class TfMetaService : ITfMetaService
{
	public static List<ITfRecipeStepAddon> _recipeSteps { get; internal set; } = new List<ITfRecipeStepAddon>();

	private static void ScanAndRegisterRecipeSteps(
		Type type)
	{
		if (type.GetInterfaces().Any(x => x == typeof(ITfRecipeStepAddon)))
		{
			var instance = (ITfRecipeStepAddon)Activator.CreateInstance(type);
			if(_addonIdHS.Contains(instance.AddonId))
				throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
			_addonIdHS.Add(instance.AddonId);
			_recipeSteps.Add(instance);
			_typesMap[type.FullName] = type;
		}
	}
	public ITfRecipeStepAddon GetRecipeStep(Guid id)
	{
		return _recipeSteps.SingleOrDefault(x => x.AddonId == id);
	}

	public ReadOnlyCollection<ITfRecipeStepAddon> GetRecipeSteps()
	{
		return _recipeSteps.AsReadOnly();
	}
}


