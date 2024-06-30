namespace WebVella.Tefter.Web.Store.CultureState;

public record InitCultureStateAction
{
	public TucCultureOption Culture { get; }

	internal InitCultureStateAction(
		TucCultureOption culture)
	{
		Culture = culture;
	}
}
