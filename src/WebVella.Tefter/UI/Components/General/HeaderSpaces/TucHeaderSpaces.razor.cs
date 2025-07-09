namespace WebVella.Tefter.UI.Components;
public partial class TucHeaderSpaces : TfBaseComponent
{
	[Inject] protected ITfConfigurationService TfConfigurationService { get; set; } = default!;

	private IReadOnlyDictionary<string, object>? _attributes = null;
		
	private List<TfMenuItem> _menu = new();
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_attributes = (new Dictionary<string,object>(){
			{"title",LOC("Browse Spaces")}
		}).AsReadOnly();

		var iconName = "AppFolder";
		_menu.Add(new TfMenuItem
		{
			Id = (new Guid("176c9d30-58bb-4ff9-8101-ba90252147f4")).ToString(),
			Text = null,
			Url = "#",
			IconCollapsed = TfConstants.GetIcon(iconName),
			IconExpanded = TfConstants.GetIcon(iconName)
		});
	}
	private Task _onClick(){ 
		return Task.CompletedTask;
	}
}