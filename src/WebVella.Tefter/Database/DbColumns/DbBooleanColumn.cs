namespace WebVella.Tefter.Database;

public record DbBooleanColumn : DbColumn
{
    public override DbType Type => DbType.Boolean;
    internal override string DatabaseColumnType => "BOOLEAN";
}
