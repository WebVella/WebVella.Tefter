namespace WebVella.Tefter.Database;

public abstract class DbObject
{
    public virtual string Name { get; set; }
    internal DbObjectMeta Meta { get; set; } = new();

    #region <=== Meta ===>
    internal void SetMeta(Guid id, DbObjectType type)
    {
        Meta = new DbObjectMeta(id, type);
    }

    #endregion
}
