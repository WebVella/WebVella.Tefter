namespace WebVella.Tefter.Database;

public record DbGuidColumn : DbColumnWithAutoDefaultValue
{
    public override DbType Type => DbType.Guid;
    internal override string DatabaseColumnType => "UUID";
}
