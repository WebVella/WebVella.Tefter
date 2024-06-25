namespace WebVella.Tefter.Web.Store.DataProviderDetailsState;

public record GetDataProviderDetailsAction
{
	public Guid RecordId { get; }

	public GetDataProviderDetailsAction(Guid recordId)
	{
		RecordId = recordId;
	}
}
