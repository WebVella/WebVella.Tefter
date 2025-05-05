namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	ITfRecipeAddon GetRecipe(Guid id);

	ReadOnlyCollection<ITfRecipeAddon> GetRecipes();
}

public partial class TfMetaService : ITfMetaService
{
	public static List<ITfRecipeAddon> _recipes { get; internal set; } = new List<ITfRecipeAddon>();

	private static void ScanAndRegisterRecipes(
		Type type)
	{
		if (type.GetInterfaces().Any(x => x == typeof(ITfRecipeAddon)))
		{
			var instance = (ITfRecipeAddon)Activator.CreateInstance(type);
			_recipes.Add(instance);
			_typesMap[type.FullName] = type;
		}
	}
	public ITfRecipeAddon GetRecipe(Guid id)
	{
		return _recipes.SingleOrDefault(x => x.Id == id);
	}

	public ReadOnlyCollection<ITfRecipeAddon> GetRecipes()
	{
		return _recipes.AsReadOnly();
	}
}


