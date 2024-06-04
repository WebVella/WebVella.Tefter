namespace WebVella.Tefter.Database;

internal sealed record DbObjectMeta
{
    public Guid Id { get; set; }
    public Guid? ApplicationId { get; set; } = null;
    public Guid? DataProviderId { get; set; } = null;

    [JsonConstructor]
    internal DbObjectMeta() : this(Guid.Empty)
    {
    }

    internal DbObjectMeta(Guid id)
    {
        Id = id;
    }

    internal DbObjectMeta(DbObjectMeta meta)
    {
        Id = meta.Id;
        DataProviderId = meta.DataProviderId;
        ApplicationId = meta.ApplicationId;
    }
}
