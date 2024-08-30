namespace WebVella.Tefter.Web.Store.SpaceState;

public record SetSpaceOnlyAction
{
	public TucSpace Space { get; }

	internal SetSpaceOnlyAction(
		TucSpace space
		)
	{

		Space = space;
	}
}
