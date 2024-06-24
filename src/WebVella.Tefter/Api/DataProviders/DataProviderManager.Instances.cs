namespace WebVella.Tefter;

public partial interface IDataProviderManager
{
	internal Result<ReadOnlyCollection<TfDataProvider>> GetProviders();
}

internal partial class DataProviderManager : IDataProviderManager
{
	public Result<ReadOnlyCollection<TfDataProvider>> GetProviders()
	{
		try
		{
			List<TfDataProvider> providers = new List<TfDataProvider>();
			
			var getProviderTypes = GetProviderTypes();

			var providersDbo = _dboManager.GetList<DataProviderDbo>();
			
			foreach (var dbo in providersDbo)
			{
				var providerType = getProviderTypes.SingleOrDefault(x => x.Id == dbo.TypeId);
				
				if(providerType == null)
					return Result.Fail(new Error($"Failed to get data providers, because " +
						$"provider type {dbo.TypeName} with id = '{dbo.TypeId}' is not found."));

				TfDataProvider provider = new TfDataProvider
				{
					Id = dbo.Id,
					Name = dbo.Name,
					CompositeKeyPrefix = dbo.CompositeKeyPrefix,
					Index = dbo.Index,
					ProviderType = providerType
				};

				providers.Add(provider);
			}

			return Result.Ok(providers.AsReadOnly());
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data providers").CausedBy(ex));
		}
	}
}
