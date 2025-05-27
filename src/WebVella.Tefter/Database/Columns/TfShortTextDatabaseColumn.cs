namespace WebVella.Tefter.Database;

public record TfShortTextDatabaseColumn : TfDatabaseColumnWithAutoDefaultValue
{
    internal override string DatabaseColumnType => "VARCHAR(1024)";
}
