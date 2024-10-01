namespace WebVella.Tefter.Talk.Components;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Talk Comments Count Display")]
[LocalizationResource("WebVella.Tefter.Talk.ViewColumns.Components.TalkCommentsCountComponent.TfTalkCommentsCountComponent", "WebVella.Tefter")]
public partial class TfTalkCommentsCountComponent : TucBaseViewColumn<TfTalkCommentsCountComponentOptions>
{
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfTalkCommentsCountComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfTalkCommentsCountComponent(TucViewColumnComponentContext context)
	{
		Context = context;
	}

	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override object GetData()
	{
		return null;
	}


	private Task _onClick(){ 
		ToastService.ShowInfo("clicked");
		return Task.CompletedTask;
	}

}

public class TfTalkCommentsCountComponentOptions { }