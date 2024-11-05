namespace WebVella.Tefter.Database;

internal sealed record TfDatabaseTableMeta
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
    internal TfDatabaseTableMeta() : this(Guid.Empty)
    {
    }

    internal TfDatabaseTableMeta(Guid id)
    {
        Id = id;
    }

    internal TfDatabaseTableMeta(TfDatabaseTableMeta meta)
    {
        Id = meta.Id;
        DataProviderId = meta.DataProviderId;
        ApplicationId = meta.ApplicationId;
        LastCommited = meta.LastCommited;
    }
}
