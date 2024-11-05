namespace WebVella.Tefter.Web.Models;
public record TucDatabaseColumnTypeInfo
{
	public TucDatabaseColumnType TypeValue { get; init; }
	public string Name { get; init; }
	public bool CanBeProviderDataType { get; init; }
	public bool SupportAutoDefaultValue { get; init; }
	public TucDatabaseColumnTypeInfo() { }
	public TucDatabaseColumnTypeInfo(TfDatabaseColumnType model)
	{
		TypeValue = model.ConvertSafeToEnum<TfDatabaseColumnType,TucDatabaseColumnType>();
		switch (model)
		{
			case TfDatabaseColumnType.ShortInteger:
				{
					Name = "SHORT INTEGER (16bit)";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case TfDatabaseColumnType.Integer:
				{
					Name = "INTEGER (32bit)";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case TfDatabaseColumnType.LongInteger:
				{
					Name = "LONG INTEGER (64bit)";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case TfDatabaseColumnType.Number:
				{
					Name = "DECIMAL";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case TfDatabaseColumnType.Boolean:
				{
					Name = "BOOLEAN";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case TfDatabaseColumnType.Date:
				{
					Name = "DATE";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = true;
				}
				break;
			case TfDatabaseColumnType.DateTime:
				{
					Name = "DATE AND TIME";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = true;
				}
				break;
			case TfDatabaseColumnType.ShortText:
				{
					Name = "SHORT TEXT";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case TfDatabaseColumnType.Text:
				{
					Name = "TEXT";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case TfDatabaseColumnType.Guid:
				{
					Name = "GUID";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = true;
				}
				break;
			case TfDatabaseColumnType.AutoIncrement:
				throw new Exception($"'{model}' should not be available for use cases");
			default:
				throw new Exception($"DatabaseColumnType: '{model}' not supported by use case");
		}
	}
	public TucDatabaseColumnTypeInfo(DatabaseColumnTypeInfo model){
		TypeValue = model.Type.ConvertSafeToEnum<TfDatabaseColumnType,TucDatabaseColumnType>();
		Name = model.Name;
		CanBeProviderDataType = model.CanBeProviderDataType;
		SupportAutoDefaultValue = model.SupportAutoDefaultValue;
	}
	public TfDatabaseColumnType ToModel()
	{
		return TypeValue.ConvertSafeToEnum<TucDatabaseColumnType,TfDatabaseColumnType>();
	}
}
