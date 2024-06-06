namespace WebVella.Tefter.Database;

public record DbBooleanColumn : DbColumn
{
    public override DbType Type => DbType.Boolean;
}
