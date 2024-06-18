namespace WebVella.Tefter.Database;

public sealed record Difference
{
    public DifferenceActionType Type { get; init; }
    public DifferenceObjectType ObjectType { get; init; }
    public string TableName { get; init; }
    public string ObjectName { get; init; }
    public DatabaseObject Object { get; init; }
    public ReadOnlyCollection<string> Descriptions { get; init; }
}
