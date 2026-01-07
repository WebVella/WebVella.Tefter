namespace WebVella.Tefter.Database;

public record TfShortIntegerDatabaseColumn : TfDatabaseColumnWithAutoDefaultValue
{
    internal override string DatabaseColumnType => "SMALLINT";
}
