namespace WebVella.Tefter.Database;

internal sealed record DbColumnMeta
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonInclude]
    [JsonPropertyName("last_commited")]
    internal DateTime LastCommited { get; set; } = Constants.DB_INITIAL_LAST_COMMITED;

    [JsonConstructor]
    internal DbColumnMeta() : this(Guid.Empty)
    {
    }

    internal DbColumnMeta(Guid id)
    {
        Id = id;
    }

    internal DbColumnMeta(DbColumnMeta meta)
    {
        Id = meta.Id;
        LastCommited = meta.LastCommited;
    }
}
