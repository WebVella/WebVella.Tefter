namespace WebVella.Tefter.Database;

public class DbGuidColumn : DbColumn
{
    public override DbType Type => DbType.Guid;
    public bool GenerateNewIdAsDefaultValue { get; set; } = false;
}
