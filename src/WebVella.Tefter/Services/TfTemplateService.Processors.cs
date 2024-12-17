namespace WebVella.Tefter.Services;

public partial interface ITfTemplateService
{
	public Result<ITfTemplateProcessor> GetTemplateProcessor(
			string typeName);

	public Result<ITfTemplateProcessor> GetTemplateProcessor(
			Type type);

	public Result<ReadOnlyCollection<Type>> GetTemplateProcessorTypes();

	public Result<ReadOnlyCollection<ITfTemplateProcessor>> GetTemplateProcessors();
}

internal partial class TfTemplateService : ITfTemplateService
{
	private static AsyncLock _lock = new AsyncLock();
	private static Dictionary<Type, ITfTemplateProcessor> _templateProcessorsDict = null;

	public Result<ITfTemplateProcessor> GetTemplateProcessor(
		string typeName )
	{
		try
		{
			var type = Type.GetType( typeName );

			return GetTemplateProcessor(type);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get template processor.").CausedBy(ex));
		}
	}

	public Result<ITfTemplateProcessor> GetTemplateProcessor(
		Type type)
	{
		try
		{
			ScanAndRegisterProcessorTypes();

			if (_templateProcessorsDict.ContainsKey(type))
			{
				return Result.Ok(_templateProcessorsDict[type]);
			}

			return Result.Ok((ITfTemplateProcessor)null);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get template processor.").CausedBy(ex));
		}
	}

	public Result<ReadOnlyCollection<ITfTemplateProcessor>> GetTemplateProcessors()
	{
		try
		{
			ScanAndRegisterProcessorTypes();

			var result = _templateProcessorsDict
				.Values
				.ToList()
				.AsReadOnly();

			return Result.Ok(result);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get template processors list.").CausedBy(ex));
		}
	}

	public Result<ReadOnlyCollection<Type>> GetTemplateProcessorTypes()
	{
		try
		{
			ScanAndRegisterProcessorTypes();

			var result = _templateProcessorsDict
				.Keys
				.ToList()
				.AsReadOnly();

			return Result.Ok(result);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get template processors types list.").CausedBy(ex));
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
