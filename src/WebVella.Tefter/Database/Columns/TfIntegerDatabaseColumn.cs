namespace WebVella.Tefter.Database;

public record TfIntegerDatabaseColumn : TfDatabaseColumnWithAutoDefaultValue
{
    internal override string DatabaseColumnType => "INTEGER";
}
