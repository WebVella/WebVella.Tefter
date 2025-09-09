namespace WebVella.Tefter.UI.Components;
public partial class TucFilterQueryManage : TfBaseComponent
{

	[Parameter]
	public TfFilterQuery? Item { get; set; }

	[Parameter]
	public EventCallback<(string, List<string>?)> AddFilter { get; set; }

	[Parameter]
	public EventCallback<TfFilterQuery> RemoveFilter { get; set; }

	[Parameter]
	public EventCallback<TfFilterQuery> UpdateFilter { get; set; }

	[Parameter]
	public List<TfFilterQuery> AllOptions { get; set; } = new();

	[Parameter]
	public Dictionary<string, string> ColumnDict { get; set; } = new();

	[Parameter]
	public Dictionary<string, TfDatabaseColumnType> TypeDict { get; set; } = new();

	[Parameter]
	public bool Disabled { get; set; } = false;

	[Parameter]
	public bool ReadOnly { get; set; } = false;

	private TfFilterQuery _selectedOption = new();

	private string _columnViewTitle
	{
		get
		{
			if (Item is not null && ColumnDict.ContainsKey(Item.Name))
				return ColumnDict[Item.Name];
			return "undefined";
		}
	}

	private string _columnViewType
	{
		get
		{
			if (Item is not null && TypeDict.ContainsKey(Item.Name))
				return TypeDict[Item.Name].ToDescriptionString();
			return "undefined";
		}
	}

	private async Task _addColumnFilterHandler()
	{
		//if (String.IsNullOrWhiteSpace(_selectedFilterColumn)) return;
		//if (Item is null) return;
		//await AddColumnFilter.InvokeAsync((_selectedFilterColumn, Item.Id));
		//await TfSpaceDataManage.AddColumnFilter(_selectedFilterColumn, Item.Id);

		//_selectedFilterColumn = null; //do not clear for convenience
	}

	private async Task _deleteFilterHandler()
	{
		//await RemoveColumnFilter.InvokeAsync(Item.Id);
		//await TfSpaceDataManage.RemoveColumnFilter(Item.Id);
	}

	private async Task _valueChanged(TfFilterBase model, string propName, object valueObj)
	{
		if (model is TfFilterAnd || model is TfFilterOr) return;
		TfFilterBase updateObj = null;
		if (model is TfFilterBoolean)
		{
			var item = (TfFilterBoolean)model;
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TfFilterBooleanComparisonMethod)valueObj;
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
		else if (model is TfFilterDateTime)
		{
			var item = (TfFilterDateTime)model;
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TfFilterDateTimeComparisonMethod)valueObj;
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
		else if (model is TfFilterGuid)
		{
			var item = (TfFilterGuid)model;
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TfFilterGuidComparisonMethod)valueObj;
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
		else if (model is TfFilterNumeric)
		{
			var item = (TfFilterNumeric)model;
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TfFilterNumericComparisonMethod)valueObj;
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
		else if (model is TfFilterText)
		{
			var item = (TfFilterText)model;
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TfFilterTextComparisonMethod)valueObj;
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

		//await UpdateColumnFilter.InvokeAsync(updateObj);
		//await TfSpaceDataManage.UpdateColumnFilter(updateObj);
		await InvokeAsync(StateHasChanged);
	}


}
