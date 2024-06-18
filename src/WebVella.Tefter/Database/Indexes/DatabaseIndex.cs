namespace WebVella.Tefter.Database;

public abstract record DatabaseIndex : DatabaseObject
{
    private List<string> _columns = new List<string>();
    public ReadOnlyCollection<string> Columns => _columns.AsReadOnly();

    internal void AddColumn(string name)
    {
        _columns.Add(name);
    }
    internal void RemoveColumn(string name)
    {
        _columns.Remove(name);
    }

    public override string ToString()
    {
        return Name;
    }
}
