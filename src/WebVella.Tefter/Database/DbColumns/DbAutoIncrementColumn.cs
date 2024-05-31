namespace WebVella.Tefter.Database;

public class DbAutoIncrementColumn : DbColumn
{
    public override DbType Type => DbType.AutoIncrement;
    public override object DefaultValue => null;
    public override bool IsNullable => false;
}
