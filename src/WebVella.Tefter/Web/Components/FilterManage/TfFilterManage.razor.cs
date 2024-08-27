using WebVella.Tefter.Web.Components.SpaceDataManage;

namespace WebVella.Tefter.Web.Components.FilterManage;
public partial class TfFilterManage : TfBaseComponent
{
	[CascadingParameter (Name ="TfSpaceDataManage")]
	public TfSpaceDataManage TfSpaceDataManage { get; set;}

	[Parameter]
	public TucFilterBase Item
	{
		get => _item;
		set => _item = JsonSerializer.Deserialize<TucFilterBase>(JsonSerializer.Serialize(value));
	}
	private TucFilterBase _item = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
	}

	private void _addFilter()
	{
		TfSpaceDataManage.AddFilter(typeof(TucFilterAnd),null);
	}

}
