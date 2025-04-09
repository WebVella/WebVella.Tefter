namespace WebVella.Tefter.Web.Models;
public record TucDataProviderTypeInfo
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public ITfDataProviderAddon Model { get; init; }
	public List<TucDataProviderTypeDataTypeInfo> SupportedSourceDataTypes { get; init; } = new();
	public TucDataProviderTypeInfo() { }
	public TucDataProviderTypeInfo(ITfDataProviderAddon model)
	{
		Id = model.Id;
		Name = model.Name;
		Description = model.Description;
		FluentIconName = model.FluentIconName;
		foreach (var sourceDataType in model.GetSupportedSourceDataTypes())
		{
			var supportedDbTypes = new List<TucDatabaseColumnTypeInfo>();
			foreach (var item in model.GetDatabaseColumnTypesForSourceDataType(sourceDataType))
			{
				supportedDbTypes.Add(new TucDatabaseColumnTypeInfo(item));
			}
			SupportedSourceDataTypes.Add(new TucDataProviderTypeDataTypeInfo{ 
				Name = sourceDataType,
				SupportedDatabaseColumnTypes = supportedDbTypes
			});
		}
		Model = model;
	}
	public ITfDataProviderAddon ToModel(ReadOnlyCollection<ITfDataProviderAddon> type)
	{
		return type.Single(x=> x.Id == Id);
	}
}
