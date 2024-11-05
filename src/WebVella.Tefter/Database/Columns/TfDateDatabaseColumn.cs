namespace WebVella.Tefter.Database;

public record TfDateDatabaseColumn : TfDatabaseColumnWithAutoDefaultValue
{
    internal override string DatabaseColumnType => "DATE";
}
