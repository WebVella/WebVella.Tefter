namespace WebVella.Tefter.Database;

public record TfLongIntegerDatabaseColumn : TfDatabaseColumnWithAutoDefaultValue
{
    internal override string DatabaseColumnType => "BIGINT";
}
