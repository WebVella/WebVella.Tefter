namespace WebVella.Tefter.Database;

public abstract record DbColumn : DbObject
{
    #region <=== Meta ===>

    private DbColumnMeta _meta = new();
    public Guid Id { get { return _meta.Id; } init { _meta.Id = value; } }
    
    #endregion

    public DbTable Table { get; set; }
    public virtual DbType Type { get; set; }
    public virtual object DefaultValue { get; set; } = null;
    public virtual bool IsNullable { get; set; } = false;
}
