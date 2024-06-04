namespace WebVella.Tefter.Database;

public abstract record DbObject
{
    public virtual string Name { get; init; }

    public override string ToString()
    {
        return Name;
    }
}
