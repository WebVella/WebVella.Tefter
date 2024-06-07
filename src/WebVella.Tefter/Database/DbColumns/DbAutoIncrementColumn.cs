namespace WebVella.Tefter.Database;

public record DbAutoIncrementColumn : DbColumn
{
    public override DbType Type => DbType.AutoIncrement;
    public override object DefaultValue => null;
    public override bool IsNullable => false;
    internal override string DatabaseColumnType => "SERIAL";
}