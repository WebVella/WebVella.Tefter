namespace WebVella.Tefter.Web.Models;
public record TucDatabaseColumnTypeInfo
{
	public TfDatabaseColumnType TypeValue { get; init; }
	public string Name { get; init; }
	public bool CanBeProviderDataType { get; init; }
	public bool SupportAutoDefaultValue { get; init; }
	public TucDatabaseColumnTypeInfo() { }
	public TucDatabaseColumnTypeInfo(TfDatabaseColumnType model)
	{
		var typeTf = (TfService.GetDatabaseColumnTypeInfosList()).Single(x=> x.Type == model);
		TypeValue = typeTf.Type;
		Name = typeTf.Name;
		CanBeProviderDataType = typeTf.CanBeProviderDataType;
		SupportAutoDefaultValue = typeTf.SupportAutoDefaultValue;		
	}
	public TucDatabaseColumnTypeInfo(DatabaseColumnTypeInfo model){
		TypeValue = model.Type;
		Name = model.Name;
		CanBeProviderDataType = model.CanBeProviderDataType;
		SupportAutoDefaultValue = model.SupportAutoDefaultValue;
	}
}
