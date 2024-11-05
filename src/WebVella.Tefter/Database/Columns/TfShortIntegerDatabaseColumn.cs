namespace WebVella.Tefter.Database;

public record TfShortIntegerDatabaseColumn : TfDatabaseColumn
{
    internal override string DatabaseColumnType => "SMALLINT";
}
