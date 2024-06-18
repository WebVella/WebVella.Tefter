namespace WebVella.Tefter.Database;

public sealed record DatabaseTableCollection : DatabaseObjectCollection<DatabaseTable>
{
    public DatabaseTable Find(Guid id)
    {
        using (_lock.Lock())
        {
            return _dbObjects.SingleOrDefault(c => c.Id == id);
        }
    }
}
