namespace WebVella.Tefter.Database;

public record TfDatabaseColumnCollection : TfDatabaseObjectCollection<TfDatabaseColumn>
{
    public TfDatabaseColumn Find(Guid id)
    {
        using (_lock.Lock())
        {
            return _dbObjects.SingleOrDefault(c => c.Id == id);
        }
    }
}
