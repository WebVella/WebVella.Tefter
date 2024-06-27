namespace WebVella.Tefter.Database;

public record AutoIncrementDatabaseColumn : DatabaseColumn
{
    public override object DefaultValue => null;
    public override bool IsNullable => false;
    internal override string DatabaseColumnType => "SERIAL";
}