namespace WebVella.Tefter.Web.Store.CultureState;

public record SetCultureAction
{

	public Guid UserId { get; }
	public CultureOption Culture { get; }
	public bool Persist { get; } = true;

	public SetCultureAction(
		Guid userId,
		CultureOption culture,
		bool persist)
	{
		UserId = userId;
		Culture = culture;
		Persist = persist;
	}
}
