namespace WebVella.Tefter.Database;

public abstract class DbObjectWithMeta : DbObject
{
    internal DbObjectMeta Meta { get; set; } = new();

    #region <=== Meta ===>
    internal void SetMeta(Guid? id = null, Guid? dataProviderId = null, Guid? applicationId = null)
    {
        Meta = new DbObjectMeta(id.HasValue ? id.Value : Guid.Empty);
        Meta.ApplicationId = applicationId;
        Meta.DataProviderId = dataProviderId;
    }

    internal void SetMeta(DbObjectMeta meta)
    {
        if (meta is null)
            throw new ArgumentNullException(nameof(meta));

        Meta = new DbObjectMeta(meta);
    }

    #endregion
}
