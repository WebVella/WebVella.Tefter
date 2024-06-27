namespace WebVella.Tefter.Database;

public record GuidDatabaseColumn : DatabaseColumnWithAutoDefaultValue
{
    internal override string DatabaseColumnType => "UUID";
}
