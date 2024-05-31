namespace WebVella.Tefter.Database;

public abstract class DbConstraint : DbObject
{
    public DbTable Table { get; set; }
}
