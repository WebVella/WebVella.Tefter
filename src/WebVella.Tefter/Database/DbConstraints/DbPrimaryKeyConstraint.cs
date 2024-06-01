namespace WebVella.Tefter.Database;

public class DbPrimaryKeyConstraint : DbConstraint
{
    public DbColumnCollection Columns { get; set; } = new();
}
