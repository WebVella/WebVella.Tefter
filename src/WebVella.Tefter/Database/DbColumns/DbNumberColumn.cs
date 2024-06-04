namespace WebVella.Tefter.Database;

public record DbNumberColumn : DbColumn
{
    public override DbType Type => DbType.Number;
}
