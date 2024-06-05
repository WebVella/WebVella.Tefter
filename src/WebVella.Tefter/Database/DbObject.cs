namespace WebVella.Tefter.Database;

public abstract record DbObject
{
    public virtual string Name { get; init; }
    internal virtual bool IsNew { get; init; }

    public override string ToString()
    {
        return Name;
    }
}
