namespace WebVella.Tefter.Database;

public record ShortTextDatabaseColumn : DatabaseColumn
{
    internal override string DatabaseColumnType => "VARCHAR(1024)";
}
