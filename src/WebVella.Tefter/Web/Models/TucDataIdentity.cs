namespace WebVella.Tefter.Web.Models;

public record TucDataIdentity
{
	public string Name { get; set; }
	public string Description { get; set; }
	public bool IsSystem { get; set; }

	public TucDataIdentity() { }
	public TucDataIdentity(TfDataIdentity model)
	{
		Name = model.DataIdentity;
		Description = model.Label;
		IsSystem = model.IsSystem;

	}

	public TfDataIdentity ToModel(){ 
		return new TfDataIdentity{ 
			DataIdentity = Name,
			Label = Description,
		};
	}
}
