namespace WebVella.Tefter.Models;

public class TfRelDataIdentityQueryInfo
{
	public required string DataIdentity { get; set; }
	public string? RelDataIdentity { get; set; } = null;
	public required List<string> RelIdentityValues { get; set; } = new();
}
