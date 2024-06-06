namespace WebVella.Tefter.Database;

public sealed record DbTableCollection : DbObjectCollection<DbTable>
{
    public DbTable Find(Guid id)
    {
        using (_lock.Lock())
        {
            return _dbObjects.SingleOrDefault(c => c.Id == id);
        }
    }
}
