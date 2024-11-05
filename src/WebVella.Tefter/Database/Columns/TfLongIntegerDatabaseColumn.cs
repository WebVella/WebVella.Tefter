namespace WebVella.Tefter.Database;

public record TfLongIntegerDatabaseColumn : TfDatabaseColumn
{
    internal override string DatabaseColumnType => "BIGINT";
}
