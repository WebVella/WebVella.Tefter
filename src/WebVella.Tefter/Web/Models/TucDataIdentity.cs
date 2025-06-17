namespace WebVella.Tefter.Web.Models;

public record TucDataIdentity
{
	public string DataIdentity { get; set; }
	public string Label { get; set; }
	public bool IsSystem { get; set; }

	public TucDataIdentity() { }
	public TucDataIdentity(TfDataIdentity model)
	{
		DataIdentity = model.DataIdentity;
		Label = model.Label;
		IsSystem = model.IsSystem;

	}

	public TfDataIdentity ToModel(){ 
		return new TfDataIdentity{ 
			DataIdentity = DataIdentity,
			Label = Label,
		};
	}
}
