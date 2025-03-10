namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public ITfTemplateProcessor GetTemplateProcessor(
			string typeName);

	public ITfTemplateProcessor GetTemplateProcessor(
			Type type);

	public ReadOnlyCollection<Type> GetTemplateProcessorTypes();

	public ReadOnlyCollection<ITfTemplateProcessor> GetTemplateProcessors();
}

public partial class TfService : ITfService
{
	private static Dictionary<Type, ITfTemplateProcessor> _templateProcessorsDict = null;

	public ITfTemplateProcessor GetTemplateProcessor(
		string typeName)
	{
		var type = Type.GetType(typeName);
		return GetTemplateProcessor(type);
	}

	public ITfTemplateProcessor GetTemplateProcessor(
		Type type)
	{
		ScanAndRegisterProcessorTypes();

		if (_templateProcessorsDict.ContainsKey(type))
			return _templateProcessorsDict[type];

		return null;
	}

	public ReadOnlyCollection<ITfTemplateProcessor> GetTemplateProcessors()
	{
		ScanAndRegisterProcessorTypes();

		return _templateProcessorsDict
			.Values
			.ToList()
			.AsReadOnly();
	}

	public ReadOnlyCollection<Type> GetTemplateProcessorTypes()
	{
		ScanAndRegisterProcessorTypes();

		return _templateProcessorsDict
			.Keys
			.ToList()
			.AsReadOnly();
	}

	private void ScanAndRegisterProcessorTypes()
	{
		using (_lock.Lock())
		{
			if (_templateProcessorsDict is not null)
			{
				return;
			}

			_templateProcessorsDict = new Dictionary<Type, ITfTemplateProcessor>();

			var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));

			foreach (var assembly in assemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.GetInterfaces().Any(x => x == typeof(ITfTemplateProcessor)))
					{
						var instance = (ITfTemplateProcessor)Activator.CreateInstance(type);
						_templateProcessorsDict.Add(type, instance);
					}
				}
			}
		}
	}
}
