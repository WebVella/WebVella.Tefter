namespace WebVella.Tefter.Database;

public abstract class DbIndex : DbObject
{
    public DbTable Table { get; set; }
    public virtual string Name { get; set; }
    public DbColumnCollection Columns { get; set; } = new();
}
