using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace WebVella.Tefter.Database;

public abstract record DbObjectWithMeta : DbObject
{
    public Guid Id { get { return _meta.Id; } internal set { _meta.Id = value; } }
    public Guid? ApplicationId { get { return _meta.ApplicationId; } internal set { _meta.ApplicationId = value; } } 
    public Guid? DataProviderId { get { return _meta.DataProviderId; } internal set { _meta.DataProviderId = value; } }

    private DbObjectMeta _meta = new();

    #region <=== Meta ===>

    internal DbObjectMeta GetMeta()
    {
        return new DbObjectMeta(_meta);
    }

    internal string GetMetaJson()
    {
        return JsonSerializer.Serialize(_meta);
    }

    internal void SetMeta(Guid? id = null, Guid? dataProviderId = null, Guid? applicationId = null)
    {
        _meta = new DbObjectMeta(id.HasValue ? id.Value : Guid.Empty);
        _meta.ApplicationId = applicationId;
        _meta.DataProviderId = dataProviderId;
    }

    internal void SetMeta(DbObjectMeta meta)
    {
        if (meta is null)
            throw new ArgumentNullException(nameof(meta));

        _meta = new DbObjectMeta(meta);
    }

    internal static DbObjectMeta GetMetaFromJson(string json)
    {
        return JsonSerializer.Deserialize<DbObjectMeta>(json);
    }

    #endregion
}
