namespace WebVella.Tefter.Models;

public class TfRelDataIdentityQueryInfo
{
	public required string DataIdentity { get; set; }
	public required string RelDataIdentity { get; set; }
	public required List<string> RelIdentityValues { get; set; } = new();
}
