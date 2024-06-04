namespace WebVella.Tefter.Database;

public record DbGuidColumn : DbColumn
{
    public override DbType Type => DbType.Guid;
    public bool GenerateNewIdAsDefaultValue { get; set; } = false;
}
