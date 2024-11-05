namespace WebVella.Tefter.Database;

public record TfNumberDatabaseColumn : TfDatabaseColumn
{
    internal override string DatabaseColumnType => "NUMERIC";
}
