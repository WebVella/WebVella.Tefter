namespace WebVella.Tefter.Database;

public abstract record DbColumn : DbObject
{
    #region <=== Meta ===>

    private DbColumnMeta _meta = new();
    public Guid Id { get { return _meta.Id; } init { _meta.Id = value; } }
    internal DateTime LastCommited { get { return _meta.LastCommited; } set { _meta.LastCommited= value; } }

    #endregion

    public virtual DbType Type { get; set; }
    public virtual object DefaultValue { get; set; } = null;
    public virtual bool IsNullable { get; set; } = false;
    public override string ToString()
    {
        return Name;
    }
}
