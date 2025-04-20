namespace WebVella.Tefter.Web.Models;

public record TucDataProviderInfo
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public int Index { get; init; }
	public long RowsCount { get; internal set; }
	public DateTime? NextSyncOn { get; internal set; }
	public TucDataProviderTypeInfo ProviderType { get; internal set; }
	public TucDataProviderInfo() { }
	public TucDataProviderInfo(TfDataProviderInfo model)
	{
		Id = model.Id;
		Name = model.Name;
		Index = model.Index;
		ProviderType = new TucDataProviderTypeInfo(model.ProviderType);
		RowsCount = model.RowsCount;
		NextSyncOn = model.NextSyncOn;
	}

}
