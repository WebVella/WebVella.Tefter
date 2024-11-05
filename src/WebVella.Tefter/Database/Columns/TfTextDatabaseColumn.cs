namespace WebVella.Tefter.Database;

public record TfTextDatabaseColumn : TfDatabaseColumn
{
    internal override string DatabaseColumnType => "TEXT";
}
