namespace WebVella.Tefter.Database;

public abstract record DatabaseColumnWithAutoDefaultValue: DatabaseColumn 
{
    public bool AutoDefaultValue { get; init; } = false;

    public override string ToString()
    {
        return Name;
    }
}
