namespace WebVella.Tefter.Database;

public record DbTextColumn : DbColumn
{
    public override DbType Type => DbType.Text;
    internal override string DatabaseColumnType => "TEXT";
}
