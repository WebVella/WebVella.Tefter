namespace WebVella.Tefter.Database;

internal sealed record DbTableMeta
{
    public Guid Id { get; set; }
    public Guid? ApplicationId { get; set; } = null;
    public Guid? DataProviderId { get; set; } = null;

    [JsonConstructor]
    internal DbTableMeta() : this(Guid.Empty)
    {
    }

    internal DbTableMeta(Guid id)
    {
        Id = id;
    }

    internal DbTableMeta(DbTableMeta meta)
    {
        Id = meta.Id;
        DataProviderId = meta.DataProviderId;
        ApplicationId = meta.ApplicationId;
    }
}
