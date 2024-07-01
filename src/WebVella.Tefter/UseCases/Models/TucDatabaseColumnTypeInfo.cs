namespace WebVella.Tefter.UseCases.Models;
public record TucDatabaseColumnTypeInfo
{
	public int TypeValue { get; init; }
	public string Name { get; init; }
	public bool CanBeProviderDataType { get; init; }
	public bool SupportAutoDefaultValue { get; init; }
	public TucDatabaseColumnTypeInfo() { }
	public TucDatabaseColumnTypeInfo(DatabaseColumnType model)
	{
		TypeValue = (int)model;
		switch (model)
		{
			case DatabaseColumnType.ShortInteger:
				{
					Name = "SHORT INTEGER (16bit)";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case DatabaseColumnType.Integer:
				{
					Name = "INTEGER (32bit)";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case DatabaseColumnType.LongInteger:
				{
					Name = "LONG INTEGER (64bit)";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case DatabaseColumnType.Number:
				{
					Name = "DECIMAL";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case DatabaseColumnType.Boolean:
				{
					Name = "BOOLEAN";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case DatabaseColumnType.Date:
				{
					Name = "DATE";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = true;
				}
				break;
			case DatabaseColumnType.DateTime:
				{
					Name = "DATE AND TIME";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = true;
				}
				break;
			case DatabaseColumnType.ShortText:
				{
					Name = "SHORT TEXT";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case DatabaseColumnType.Text:
				{
					Name = "TEXT";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = false;
				}
				break;
			case DatabaseColumnType.Guid:
				{
					Name = "GUID";
					CanBeProviderDataType = true;
					SupportAutoDefaultValue = true;
				}
				break;
			case DatabaseColumnType.AutoIncrement:
				throw new Exception($"'{model}' should not be available for use cases");
			default:
				throw new Exception($"DatabaseColumnType: '{model}' not supported by use case");
		}
	}
	public DatabaseColumnType ToModel()
	{
		return (DatabaseColumnType)TypeValue;
	}
}
