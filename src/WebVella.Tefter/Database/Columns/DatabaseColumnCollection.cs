namespace WebVella.Tefter.Database;

public record DatabaseColumnCollection : DatabaseObjectCollection<DatabaseColumn>
{
    public DatabaseColumn Find(Guid id)
    {
        using (_lock.Lock())
        {
            return _dbObjects.SingleOrDefault(c => c.Id == id);
        }
    }
}
