namespace WebVella.Tefter.Database;

public record TfShortTextDatabaseColumn : TfDatabaseColumn
{
    internal override string DatabaseColumnType => "VARCHAR(1024)";
}
