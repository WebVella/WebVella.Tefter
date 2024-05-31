namespace WebVella.Tefter.Database;

public class DbUniqueConstraint : DbConstraint
{
    public DbColumnCollection Columns { get; set; } = new();
}
