namespace WebVella.Tefter.Web.Models;
public record TucDataProviderTypeInfo
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public ITfDataProviderType Model { get; init; }
	public List<TucDataProviderTypeDataTypeInfo> SupportedSourceDataTypes { get; init; } = new();
	public TucDataProviderTypeInfo() { }
	public TucDataProviderTypeInfo(ITfDataProviderType model)
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
	public ITfDataProviderType ToModel(ReadOnlyCollection<ITfDataProviderType> type)
	{
		return type.Single(x=> x.Id == Id);
	}
}
