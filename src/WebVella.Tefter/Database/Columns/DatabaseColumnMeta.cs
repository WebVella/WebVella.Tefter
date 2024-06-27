namespace WebVella.Tefter.Database;

internal sealed record DatabaseColumnMeta
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

	[JsonPropertyName("database_column_type")]
	public DatabaseColumnType ColumnType { get; set; }

	[JsonInclude]
    [JsonPropertyName("last_commited")]
    internal DateTime LastCommited { get; set; } = Constants.DB_INITIAL_LAST_COMMITED;

    [JsonConstructor]
    internal DatabaseColumnMeta() : this(Guid.Empty)
    {
    }

    internal DatabaseColumnMeta(Guid id)
    {
        Id = id;
    }

    internal DatabaseColumnMeta(DatabaseColumnMeta meta)
    {
        Id = meta.Id;
		ColumnType = meta.ColumnType;
        LastCommited = meta.LastCommited;
    }
}
