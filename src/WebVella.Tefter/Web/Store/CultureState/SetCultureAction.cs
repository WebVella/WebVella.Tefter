namespace WebVella.Tefter.Web.Store.CultureState;

public record SetCultureAction
{
	public Guid UserId { get; }
	public TucCultureOption Culture { get; }

	internal SetCultureAction(
		Guid userId,
		TucCultureOption culture)
	{
		UserId = userId;
		Culture = culture;
	}
}
