namespace WebVella.Tefter.Templates.Services;

public partial interface ITemplatesService
{
	public Result<ITemplateProcessor> GetTemplateProcessor(
			string typeName);

	public Result<ITemplateProcessor> GetTemplateProcessor(
			Type type);

	public Result<ReadOnlyCollection<Type>> GetTemplateProcessorTypes();

	public Result<ReadOnlyCollection<ITemplateProcessor>> GetTemplateProcessors();
}

internal partial class TemplatesService : ITemplatesService
{
	private static AsyncLock _lock = new AsyncLock();
	private static Dictionary<Type, ITemplateProcessor> _templateProcessorsDict = null;

	public Result<ITemplateProcessor> GetTemplateProcessor(
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

	public Result<ITemplateProcessor> GetTemplateProcessor(
		Type type)
	{
		try
		{
			ScanAndRegisterProcessorTypes();

			if (_templateProcessorsDict.ContainsKey(type))
			{
				return Result.Ok(_templateProcessorsDict[type]);
			}

			return Result.Ok((ITemplateProcessor)null);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get template processor.").CausedBy(ex));
		}
	}

	public Result<ReadOnlyCollection<ITemplateProcessor>> GetTemplateProcessors()
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

			_templateProcessorsDict = new Dictionary<Type, ITemplateProcessor>();

			var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));

			foreach (var assembly in assemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.GetInterfaces().Any(x => x == typeof(ITemplateProcessor)))
					{
						var instance = (ITemplateProcessor)Activator.CreateInstance(type);
						_templateProcessorsDict.Add(type, instance);
					}
				}
			}
		}
	}
}
