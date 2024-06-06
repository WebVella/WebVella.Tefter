namespace WebVella.Tefter.Database;

public record DbColumnCollection : DbObjectCollection<DbColumn>
{
    public DbColumn Find(Guid id)
    {
        using (_lock.Lock())
        {
            return _dbObjects.SingleOrDefault(c => c.Id == id);
        }
    }
}
