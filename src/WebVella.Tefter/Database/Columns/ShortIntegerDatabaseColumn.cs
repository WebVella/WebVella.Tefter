namespace WebVella.Tefter.Database;

public record ShortIntegerDatabaseColumn : DatabaseColumn
{
    internal override string DatabaseColumnType => "SMALLINT";
}
