namespace WebVella.Tefter.Database;

public record TfBooleanDatabaseColumn : TfDatabaseColumn
{
    internal override string DatabaseColumnType => "BOOLEAN";
}
