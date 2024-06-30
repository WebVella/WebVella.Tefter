﻿namespace WebVella.Tefter;

public interface ITfDataProviderType
{
	Guid Id { get; }
	string Name { get; }
	string Description { get; }
	string IconBase64 { get; }
	Type SettingsComponentType { get; }
	public abstract ReadOnlyCollection<string> GetSupportedSourceDataTypes();
	public abstract ReadOnlyCollection<DatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(string dataType);
	public abstract ReadOnlyCollection<TfDataProviderDataRow> GetSourceData();
}
