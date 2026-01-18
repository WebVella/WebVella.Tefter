namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	ITfOnboardRecipeAddon? GetOnboardRecipe(Guid id);

	List<ITfOnboardRecipeAddon> GetOnboardRecipes();

	ITfSpaceRecipeAddon? GetSpaceRecipe(Guid id);

	List<ITfSpaceRecipeAddon> GetSpaceRecipes();
}

public partial class TfMetaService : ITfMetaService
{
	public static List<ITfOnboardRecipeAddon> _onboardRecipes { get; internal set; } =
		new List<ITfOnboardRecipeAddon>();

	public static List<ITfSpaceRecipeAddon> _spaceRecipes { get; internal set; } = new List<ITfSpaceRecipeAddon>();

	private static void ScanAndRegisterRecipes(
		Type type)
	{
		if (type.GetInterfaces().Any(x => x == typeof(ITfOnboardRecipeAddon)))
		{
			var instance = (ITfOnboardRecipeAddon)Activator.CreateInstance(type);
			if (_addonIdHS.Contains(instance.AddonId))
				throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
			_addonIdHS.Add(instance.AddonId);

			_onboardRecipes.Add(instance);
			_typesMap[type.FullName] = type;
		}

		else if (type.GetInterfaces().Any(x => x == typeof(ITfSpaceRecipeAddon)))
		{
			var instance = (ITfSpaceRecipeAddon)Activator.CreateInstance(type);
			if (_addonIdHS.Contains(instance.AddonId))
				throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
			_addonIdHS.Add(instance.AddonId);

			_spaceRecipes.Add(instance);
			_typesMap[type.FullName] = type;
		}
	}

	public ITfOnboardRecipeAddon? GetOnboardRecipe(Guid id)
	{
		var dictInstance = _onboardRecipes.SingleOrDefault(x => x.AddonId == id);
		if (dictInstance is null) return null;
		return (ITfOnboardRecipeAddon?)Activator.CreateInstance(dictInstance.GetType());
	}


	public List<ITfOnboardRecipeAddon> GetOnboardRecipes()
	{
		var instances = new List<ITfOnboardRecipeAddon>();
		foreach (var dictInstance in _onboardRecipes)
		{
			var instance = (ITfOnboardRecipeAddon?)Activator.CreateInstance(dictInstance.GetType());
			if(instance is null) continue;
			instances.Add(instance);
		}

		return instances;
	}

	
	public ITfSpaceRecipeAddon? GetSpaceRecipe(Guid id)
	{
		var dictInstance = _spaceRecipes.SingleOrDefault(x => x.AddonId == id);
		if (dictInstance is null) return null;
		return (ITfSpaceRecipeAddon?)Activator.CreateInstance(dictInstance.GetType());
	}	

	public List<ITfSpaceRecipeAddon> GetSpaceRecipes()
	{
		var instances = new List<ITfSpaceRecipeAddon>();
		foreach (var dictInstance in _spaceRecipes)
		{
			var instance = (ITfSpaceRecipeAddon?)Activator.CreateInstance(dictInstance.GetType());
			if(instance is null) continue;
			instances.Add(instance);
		}

		return instances;
	}	
}