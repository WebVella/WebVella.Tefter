namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public ITfTemplateProcessorAddon GetTemplateProcessor(
			string typeName);

	public ITfTemplateProcessorAddon GetTemplateProcessor(
			Type type);

	public ReadOnlyCollection<Type> GetTemplateProcessorTypes();

	public ReadOnlyCollection<ITfTemplateProcessorAddon> GetTemplateProcessors();
}

public partial class TfService : ITfService
{
	private static Dictionary<Type, ITfTemplateProcessorAddon> _templateProcessorsDict = null;

	public ITfTemplateProcessorAddon GetTemplateProcessor(
		string typeName)
	{
		try
		{
			var type = Type.GetType(typeName);
			return GetTemplateProcessor(type);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public ITfTemplateProcessorAddon GetTemplateProcessor(
		Type type)
	{
		try
		{
			ScanAndRegisterProcessorTypes();

			if (_templateProcessorsDict.ContainsKey(type))
				return _templateProcessorsDict[type];

			return null;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public ReadOnlyCollection<ITfTemplateProcessorAddon> GetTemplateProcessors()
	{
		try
		{
			ScanAndRegisterProcessorTypes();

			return _templateProcessorsDict
				.Values
				.ToList()
				.AsReadOnly();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public ReadOnlyCollection<Type> GetTemplateProcessorTypes()
	{
		try
		{
			ScanAndRegisterProcessorTypes();

			return _templateProcessorsDict
				.Keys
				.ToList()
				.AsReadOnly();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private void ScanAndRegisterProcessorTypes()
	{
		using (_lock.Lock())
		{
			if (_templateProcessorsDict is not null)
			{
				return;
			}

			_templateProcessorsDict = new Dictionary<Type, ITfTemplateProcessorAddon>();

			var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));

			foreach (var assembly in assemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.GetInterfaces().Any(x => x == typeof(ITfTemplateProcessorAddon)))
					{
						var instance = (ITfTemplateProcessorAddon)Activator.CreateInstance(type);
						_templateProcessorsDict.Add(type, instance);
					}
				}
			}
		}
	}
}
