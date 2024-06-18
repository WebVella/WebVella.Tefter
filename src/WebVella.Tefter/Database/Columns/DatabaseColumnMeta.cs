namespace WebVella.Tefter.Database;

internal sealed record DatabaseColumnMeta
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

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
        LastCommited = meta.LastCommited;
    }
}
