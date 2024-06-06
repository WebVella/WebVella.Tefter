namespace WebVella.Tefter.Database;

public abstract record DbColumnWithAutoDefaultValue: DbColumn 
{
    public bool AutoDefaultValue { get; init; } = false;

    public override string ToString()
    {
        return Name;
    }
}
