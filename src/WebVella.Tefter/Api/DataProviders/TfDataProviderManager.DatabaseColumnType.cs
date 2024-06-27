namespace WebVella.Tefter;

public partial interface ITfDataProviderManager
{
	Result<ReadOnlyCollection<DatabaseColumnTypeInfo>> GetDatabaseColumnTypeInfos();
}

public partial class TfDataProviderManager : ITfDataProviderManager
{
	public Result<ReadOnlyCollection<DatabaseColumnTypeInfo>> GetDatabaseColumnTypeInfos()
	{
		List<DatabaseColumnTypeInfo> databaseColumnTypeInfos = new List<DatabaseColumnTypeInfo>();
		
		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "AUTO INCREMENT",
				Type = DatabaseColumnType.AutoIncrement,
				CanBeProviderDataType = false,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "GUID",
				Type = DatabaseColumnType.Guid,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = true
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "DATE",
				Type = DatabaseColumnType.Date,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = true
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "DATE AND TIME",
				Type = DatabaseColumnType.DateTime,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = true
			});


		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "BOOLEAN",
				Type = DatabaseColumnType.Boolean,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "TEXT",
				Type = DatabaseColumnType.Text,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "SHORT TEXT",
				Type = DatabaseColumnType.ShortText,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "SHORT INTEGER (16bit)",
				Type = DatabaseColumnType.ShortInteger,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "INTEGER (32bit)",
				Type = DatabaseColumnType.Integer,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "LONG INTEGER (64bit)",
				Type = DatabaseColumnType.LongInteger,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "DECIMAL",
				Type = DatabaseColumnType.Number,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		return Result.Ok( databaseColumnTypeInfos.AsReadOnly());
	}
}

public record DatabaseColumnTypeInfo
{
	public string Name { get; set; }
	public DatabaseColumnType Type { get; set; }
	public bool CanBeProviderDataType { get; set; }
	public bool SupportAutoDefaultValue { get; set; }
}