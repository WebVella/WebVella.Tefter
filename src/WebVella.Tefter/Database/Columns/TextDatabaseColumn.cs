namespace WebVella.Tefter.Database;

public record TextDatabaseColumn : DatabaseColumn
{
    internal override string DatabaseColumnType => "TEXT";
}
