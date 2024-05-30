namespace WebVella.Tefter.Database;

public abstract class DbColumn
{
    public Guid Id { get; set; }
    public DbTable Table { get; set; }
    public virtual string Name { get; set; }
    public virtual DbType DbType { get; set; }
    public virtual object DefaultValue { get; set; } = null;
    public virtual bool IsNullable { get; set; } = false;
}
