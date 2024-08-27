using WebVella.Tefter.Web.Components.SpaceDataManage;

namespace WebVella.Tefter.Web.Components.FilterManage;
public partial class TfFilterManage : TfBaseComponent
{
	[CascadingParameter(Name = "TfSpaceDataManage")]
	public TfSpaceDataManage TfSpaceDataManage { get; set; }

	[Parameter]
	public TucFilterBase Item { get; set;}
	private string _selectedFilterColumn = null;

	private void _addColumnFilterHandler()
	{
		if (String.IsNullOrWhiteSpace(_selectedFilterColumn)) return;
		if (Item is null) return;
		TfSpaceDataManage.AddColumnFilter(_selectedFilterColumn, Item.Id);
		//_selectedFilterColumn = null; //do not clear for convenience
	}

	private void _deleteFilterHandler()
	{
		TfSpaceDataManage.RemoveColumnFilter(Item.Id);
	}



}
