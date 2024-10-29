namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.FilterManage.TfFilterManage", "WebVella.Tefter")]
public partial class TfFilterManage : TfBaseComponent
{

	[Parameter]
	public TucFilterBase Item { get; set; }

	[Parameter]
	public TucDataProvider SelectedProvider { get; set; }

	[Parameter]
	public EventCallback<(Type, string, Guid?)> AddFilter { get; set; }

	[Parameter]
	public EventCallback<(string, Guid?)> AddColumnFilter { get; set; }

	[Parameter]
	public EventCallback<Guid> RemoveColumnFilter { get; set; }

	[Parameter]
	public EventCallback<TucFilterBase> UpdateColumnFilter { get; set; }

	[Parameter]
	public bool Disabled { get; set; } = false;

	[Parameter]
	public bool ReadOnly { get; set; } = false;

	private string _selectedFilterColumn = null;
	public List<string> AllColumnOptions
	{
		get
		{
			if (SelectedProvider is null) return new List<string>();
			return SelectedProvider.ColumnsPublic.Select(x => x.DbName).ToList();
		}
	}
	private async Task _addColumnFilterHandler()
	{
		if (String.IsNullOrWhiteSpace(_selectedFilterColumn)) return;
		if (Item is null) return;
		await AddColumnFilter.InvokeAsync((_selectedFilterColumn, Item.Id));
		//await TfSpaceDataManage.AddColumnFilter(_selectedFilterColumn, Item.Id);

		//_selectedFilterColumn = null; //do not clear for convenience
	}

	private async Task _deleteFilterHandler()
	{
		await RemoveColumnFilter.InvokeAsync(Item.Id);
		//await TfSpaceDataManage.RemoveColumnFilter(Item.Id);
	}

	private async Task _valueChanged(TucFilterBase model, string propName, object valueObj)
	{
		if (model is TucFilterAnd || model is TucFilterOr) return;
		TucFilterBase updateObj = null;
		if (model is TucFilterBoolean)
		{
			var item = (TucFilterBoolean)model;
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TucFilterBooleanComparisonMethod)valueObj;
				item.ComparisonMethod = value;
				updateObj = item;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (Option<string>)valueObj;
				item.ValueOptionChanged(value);
				updateObj = item;
			}
			else throw new Exception("propName not supported");
		}
		else if (model is TucFilterDateTime)
		{
			var item = (TucFilterDateTime)model;
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TucFilterDateTimeComparisonMethod)valueObj;
				item.ComparisonMethod = value;
				updateObj = item;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (string)valueObj;
				item.ValueStringChanged(value);
				updateObj = item;
			}
			else throw new Exception("propName not supported");
		}
		else if (model is TucFilterGuid)
		{
			var item = (TucFilterGuid)model;
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TucFilterGuidComparisonMethod)valueObj;
				item.ComparisonMethod = value;
				updateObj = item;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (string)valueObj;
				if (!String.IsNullOrWhiteSpace(value) && !Guid.TryParse(value, out Guid outGuid))
					ToastService.ShowError(LOC("Invalid GUID value"));

				item.ValueStringChanged(value);
				updateObj = item;
				////70efbe52-033f-43b8-a8b9-65f62ca0080f
			}
			else throw new Exception("propName not supported");
		}
		else if (model is TucFilterNumeric)
		{
			var item = (TucFilterNumeric)model;
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TucFilterNumericComparisonMethod)valueObj;
				updateObj = item with { ComparisonMethod = value };
				item.ComparisonMethod = value;
				updateObj = item;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (decimal?)valueObj;
				item.ValueChanged(value);
				updateObj = item;
			}
			else throw new Exception("propName not supported");
		}
		else if (model is TucFilterText)
		{
			var item = (TucFilterText)model;
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TucFilterTextComparisonMethod)valueObj;
				item.ComparisonMethod = value;
				updateObj = item;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (string)valueObj;
				item.ValueChanged(value);
				updateObj = item;
			}
			else throw new Exception("propName not supported");
		}
		else throw new Exception("Unsupported TucFilterBase in _valueChanged");

		await UpdateColumnFilter.InvokeAsync(updateObj);
		//await TfSpaceDataManage.UpdateColumnFilter(updateObj);
		await InvokeAsync(StateHasChanged);
	}


}
