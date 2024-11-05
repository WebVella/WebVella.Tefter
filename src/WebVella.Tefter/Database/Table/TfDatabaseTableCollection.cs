namespace WebVella.Tefter.Database;

public sealed record TfDatabaseTableCollection : TfDatabaseObjectCollection<TfDatabaseTable>
{
    public TfDatabaseTable Find(Guid id)
    {
        using (_lock.Lock())
        {
            return _dbObjects.SingleOrDefault(c => c.Id == id);
        }
    }
}
