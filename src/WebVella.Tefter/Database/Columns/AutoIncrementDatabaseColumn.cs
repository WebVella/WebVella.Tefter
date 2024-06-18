namespace WebVella.Tefter.Database;

public record AutoIncrementDatabaseColumn : DatabaseColumn
{
    public override DatabaseColumnType Type => Database.DatabaseColumnType.AutoIncrement;
    public override object DefaultValue => null;
    public override bool IsNullable => false;
    internal override string DatabaseColumnType => "SERIAL";
}