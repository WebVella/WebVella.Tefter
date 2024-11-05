namespace WebVella.Tefter.Database;

public abstract record TfDatabaseColumnWithAutoDefaultValue: TfDatabaseColumn 
{
    public bool AutoDefaultValue { get; init; } = false;

    public override string ToString()
    {
        return Name;
    }
}
