namespace WebVella.Tefter.Database;

public record GuidDatabaseColumn : DatabaseColumnWithAutoDefaultValue
{
    public override DatabaseColumnType Type => Database.DatabaseColumnType.Guid;
    internal override string DatabaseColumnType => "UUID";
}
