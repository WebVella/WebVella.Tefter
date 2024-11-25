namespace WebVella.Tefter;

public partial interface ITfDataProviderManager
{
	Result<ReadOnlyCollection<DatabaseColumnTypeInfo>> GetDatabaseColumnTypeInfos();
}

public partial class TfDataProviderManager : ITfDataProviderManager
{
	public static ReadOnlyCollection<DatabaseColumnTypeInfo> GetDatabaseColumnTypeInfosList()
	{
		List<DatabaseColumnTypeInfo> databaseColumnTypeInfos =
			new List<DatabaseColumnTypeInfo>();

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "AUTO INCREMENT",
				Type = TfDatabaseColumnType.AutoIncrement,
				CanBeProviderDataType = false,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "GUID",
				Type = TfDatabaseColumnType.Guid,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = true
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "DATE",
				Type = TfDatabaseColumnType.Date,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = true
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "DATE AND TIME",
				Type = TfDatabaseColumnType.DateTime,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = true
			});


		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "BOOLEAN",
				Type = TfDatabaseColumnType.Boolean,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "TEXT",
				Type = TfDatabaseColumnType.Text,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "SHORT TEXT",
				Type = TfDatabaseColumnType.ShortText,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "SHORT INTEGER (16bit)",
				Type = TfDatabaseColumnType.ShortInteger,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "INTEGER (32bit)",
				Type = TfDatabaseColumnType.Integer,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "LONG INTEGER (64bit)",
				Type = TfDatabaseColumnType.LongInteger,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "DECIMAL",
				Type = TfDatabaseColumnType.Number,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		return databaseColumnTypeInfos.AsReadOnly();
	}
	public Result<ReadOnlyCollection<DatabaseColumnTypeInfo>> GetDatabaseColumnTypeInfos()
	{
		return Result.Ok(GetDatabaseColumnTypeInfosList());
	}
}

public record DatabaseColumnTypeInfo
{
	public string Name { get; init; }
	public TfDatabaseColumnType Type { get; init; }
	public bool CanBeProviderDataType { get; init; }
	public bool SupportAutoDefaultValue { get; init; }
}