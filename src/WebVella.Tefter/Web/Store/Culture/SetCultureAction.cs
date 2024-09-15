namespace WebVella.Tefter.Web.Store;

public record SetCultureAction : TfBaseAction
{
	public TucCultureOption Culture { get; }

	internal SetCultureAction(
		TfBaseComponent component,
		TucCultureOption culture)
	{
		Component = component;
		Culture = culture;
	}
}
