using WebVella.Tefter.Web.Components.SpaceDataManage;

namespace WebVella.Tefter.Web.Components.FilterManage;
public partial class TfFilterManage : TfBaseComponent
{
	[CascadingParameter(Name = "TfSpaceDataManage")]
	public TfSpaceDataManage TfSpaceDataManage { get; set; }

	[Parameter]
	public TucFilterBase Item { get; set;}

	[Parameter]
	public bool Disabled { get; set;} = false;

	private string _selectedFilterColumn = null;

	private async Task _addColumnFilterHandler()
	{
		if (String.IsNullOrWhiteSpace(_selectedFilterColumn)) return;
		if (Item is null) return;
		await TfSpaceDataManage.AddColumnFilter(_selectedFilterColumn, Item.Id);
		//_selectedFilterColumn = null; //do not clear for convenience
	}

	private async Task _deleteFilterHandler()
	{
		await TfSpaceDataManage.RemoveColumnFilter(Item.Id);
	}



}
