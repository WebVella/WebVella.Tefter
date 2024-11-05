namespace WebVella.Tefter.Database;

public sealed record TfDifference
{
    public TfDifferenceActionType Type { get; init; }
    public TfDifferenceObjectType ObjectType { get; init; }
    public string TableName { get; init; }
    public string ObjectName { get; init; }
    public TfDatabaseObject Object { get; init; }
    public ReadOnlyCollection<string> Descriptions { get; init; }
}
