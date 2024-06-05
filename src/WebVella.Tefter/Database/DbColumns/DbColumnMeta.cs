namespace WebVella.Tefter.Database;

internal sealed record DbColumnMeta
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonConstructor]
    internal DbColumnMeta() : this(Guid.Empty)
    {
    }

    internal DbColumnMeta(Guid id)
    {
        Id = id;
    }
}
