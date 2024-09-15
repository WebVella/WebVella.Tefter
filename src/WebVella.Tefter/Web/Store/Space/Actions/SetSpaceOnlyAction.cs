namespace WebVella.Tefter.Web.Store;

public record SetSpaceOnlyAction : TfBaseAction
{
	public TucSpace Space { get; }

	internal SetSpaceOnlyAction(
	TfBaseComponent component,
		TucSpace space
		)
	{
		Component = component;
		Space = space;
	}
}
