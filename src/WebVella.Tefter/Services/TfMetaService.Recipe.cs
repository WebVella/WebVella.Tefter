namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	ITfOnboardRecipeAddon? GetOnboardRecipe(Guid id);

	ReadOnlyCollection<ITfOnboardRecipeAddon> GetOnboardRecipes();

	ITfSpaceRecipeAddon? GetSpaceRecipe(Guid id);

	ReadOnlyCollection<ITfSpaceRecipeAddon> GetSpaceRecipes();
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
		=> _onboardRecipes.SingleOrDefault(x => x.AddonId == id);

	public ReadOnlyCollection<ITfOnboardRecipeAddon> GetOnboardRecipes()
		=> _onboardRecipes.AsReadOnly();
	
	public ITfSpaceRecipeAddon? GetSpaceRecipe(Guid id)
		=> _spaceRecipes.SingleOrDefault(x => x.AddonId == id);

	public ReadOnlyCollection<ITfSpaceRecipeAddon> GetSpaceRecipes()
		=> _spaceRecipes.AsReadOnly();	
}