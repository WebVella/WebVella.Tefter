namespace WebVella.Tefter.Database;

internal sealed record TfDatabaseColumnMeta
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

	[JsonPropertyName("database_column_type")]
	public TfDatabaseColumnType ColumnType { get; set; }

	[JsonInclude]
    [JsonPropertyName("last_commited")]
    internal DateTime LastCommited { get; set; } = TfConstants.DB_INITIAL_LAST_COMMITED;

    [JsonConstructor]
    internal TfDatabaseColumnMeta() : this(Guid.Empty)
    {
    }

    internal TfDatabaseColumnMeta(Guid id)
    {
        Id = id;
    }

    internal TfDatabaseColumnMeta(TfDatabaseColumnMeta meta)
    {
        Id = meta.Id;
		ColumnType = meta.ColumnType;
        LastCommited = meta.LastCommited;
    }
}
