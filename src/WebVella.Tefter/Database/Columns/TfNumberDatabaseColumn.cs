namespace WebVella.Tefter.Database;

public record TfNumberDatabaseColumn : TfDatabaseColumnWithAutoDefaultValue
{
    internal override string DatabaseColumnType => "NUMERIC";
}
