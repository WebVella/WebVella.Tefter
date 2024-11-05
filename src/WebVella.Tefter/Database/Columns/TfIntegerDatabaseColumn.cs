namespace WebVella.Tefter.Database;

public record TfIntegerDatabaseColumn : TfDatabaseColumn
{
    internal override string DatabaseColumnType => "INTEGER";
}
