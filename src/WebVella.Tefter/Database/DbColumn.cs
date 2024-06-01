namespace WebVella.Tefter.Database;

public abstract class DbColumn : DbObject
{
    public DbTable Table { get; set; }
    public virtual DbType Type { get; set; }
    public virtual object DefaultValue { get; set; } = null;
    public virtual bool IsNullable { get; set; } = false;
}
