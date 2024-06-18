namespace WebVella.Tefter.Database;

internal sealed record DatabaseTableMeta
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("application_id")]
    public Guid? ApplicationId { get; set; } = null;

    [JsonPropertyName("data_provider_id")]
    public Guid? DataProviderId { get; set; } = null;

    [JsonInclude]
    [JsonPropertyName("last_commited")]
    internal DateTime LastCommited { get; set; } = Constants.DB_INITIAL_LAST_COMMITED;

    [JsonConstructor]
    internal DatabaseTableMeta() : this(Guid.Empty)
    {
    }

    internal DatabaseTableMeta(Guid id)
    {
        Id = id;
    }

    internal DatabaseTableMeta(DatabaseTableMeta meta)
    {
        Id = meta.Id;
        DataProviderId = meta.DataProviderId;
        ApplicationId = meta.ApplicationId;
        LastCommited = meta.LastCommited;
    }
}
