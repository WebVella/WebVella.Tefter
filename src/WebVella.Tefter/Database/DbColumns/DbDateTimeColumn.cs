namespace WebVella.Tefter.Database;

public record DbDateTimeColumn : DbColumnWithAutoDefaultValue
{
    public override DbType Type => DbType.DateTime;
}
