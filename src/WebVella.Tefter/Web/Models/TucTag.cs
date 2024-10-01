namespace WebVella.Tefter.Web.Models;

public record TucTag
{
	public Guid Id { get; init; }
	public string Label { get; set; }

	public TucTag() { }
	public TucTag(TfTag model)
	{
		Id = model.Id;
		Label = model.Label;
	}
	public TfTag ToModel()
	{
		return new TfTag
		{
			Id = Id,
			Label = Label
		};
	}

}
