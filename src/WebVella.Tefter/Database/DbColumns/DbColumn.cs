namespace WebVella.Tefter.Database;

public abstract record DbColumn : DbObject
{
    public virtual DbType Type { get; set; }
    public virtual object DefaultValue { get; set; } = null;
    public virtual bool IsNullable { get; set; } = false;
    public override string ToString()
    {
        return Name;
    }
}
