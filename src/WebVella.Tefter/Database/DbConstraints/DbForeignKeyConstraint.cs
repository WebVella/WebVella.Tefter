namespace WebVella.Tefter.Database;

public record DbForeignKeyConstraint : DbConstraint
{
    private List<string> _foregnColumns = new List<string>();
    public string ForeignTable { get; init; } = string.Empty;
    public ReadOnlyCollection<string> ForeignColumns => _foregnColumns.AsReadOnly();

    internal void AddForeignColumn(string name)
    {
        _foregnColumns.Add(name);
    }
    internal void RemoveForeignColumn(string name)
    {
        _foregnColumns.Remove(name);
    }
}
