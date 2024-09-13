namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.FilterManage.TfFilterManage","WebVella.Tefter")]
public partial class TfFilterManage : TfBaseComponent
{
	[CascadingParameter(Name = "TfSpaceDataManage")]
	public TfSpaceDataManage TfSpaceDataManage { get; set; }

	[Parameter]
	public TucFilterBase Item { get; set; }

	[Parameter]
	public List<string> ProviderColumns { get; set; }

	[Parameter]
	public bool Disabled { get; set; } = false;

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

	private async Task _valueChanged(TucFilterBase model, string propName, object valueObj)
	{
		if (model is TucFilterAnd || model is TucFilterOr) return;
		TucFilterBase updateObj = null;
		if (model is TucFilterBoolean)
		{
			var original = (TucFilterBoolean)model with { Id = model.Id };
			var item = (TucFilterBoolean)model with { Id = model.Id };
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TucFilterBooleanComparisonMethod)valueObj;
				item.ComparisonMethod = value;
				if (item.ComparisonMethod == original.ComparisonMethod) return;
				updateObj = item;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (Option<string>)valueObj;
				item.ValueOptionChanged(value);
				if (item.Value == original.Value) return;
				updateObj = item;
			}
			else throw new Exception("propName not supported");
		}
		else if (model is TucFilterDateOnly)
		{
			var original = (TucFilterDateOnly)model with { Id = model.Id };
			var item = (TucFilterDateOnly)model with { Id = model.Id };
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TucFilterDateTimeComparisonMethod)valueObj;
				item.ComparisonMethod = value;
				if (item.ComparisonMethod == original.ComparisonMethod) return;
				updateObj = item;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (string)valueObj;
				item.ValueStringChanged(value);
				if (item.Value == original.Value) return;
				updateObj = item;
			}
			else throw new Exception("propName not supported");
		}
		else if (model is TucFilterDateTime)
		{
			var original = (TucFilterDateTime)model with { Id = model.Id };
			var item = (TucFilterDateTime)model with { Id = model.Id };
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TucFilterDateTimeComparisonMethod)valueObj;
				item.ComparisonMethod = value;
				if (item.ComparisonMethod == original.ComparisonMethod) return;
				updateObj = item;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (string)valueObj;
				item.ValueStringChanged(value);
				if (item.Value == original.Value) return;
				updateObj = item;
			}
			else throw new Exception("propName not supported");
		}
		else if (model is TucFilterGuid)
		{
			var original = (TucFilterGuid)model with { Id = model.Id };
			var item = (TucFilterGuid)model with { Id = model.Id };
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TucFilterGuidComparisonMethod)valueObj;
				item.ComparisonMethod = value;
				if (item.ComparisonMethod == original.ComparisonMethod) return;
				updateObj = item;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (string)valueObj;
				if (!String.IsNullOrWhiteSpace(value) && !Guid.TryParse(value, out Guid outGuid))
					ToastService.ShowError(LOC("Invalid GUID value"));

				item.ValueStringChanged(value);
				if (item.Value == original.Value) return;
				updateObj = item;
				////70efbe52-033f-43b8-a8b9-65f62ca0080f
			}
			else throw new Exception("propName not supported");
		}
		else if (model is TucFilterNumeric)
		{
			var original = (TucFilterNumeric)model with { Id = model.Id };
			var item = (TucFilterNumeric)model with { Id = model.Id };
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TucFilterNumericComparisonMethod)valueObj;
				updateObj = item with { ComparisonMethod = value };
				item.ComparisonMethod = value;
				if (item.ComparisonMethod == original.ComparisonMethod) return;
				updateObj = item;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (decimal?)valueObj;
				item.ValueChanged(value);
				if (item.Value == original.Value) return;
				updateObj = item;
			}
			else throw new Exception("propName not supported");
		}
		else if (model is TucFilterText)
		{
			var original = (TucFilterText)model with { Id = model.Id };
			var item = (TucFilterText)model with { Id = model.Id };
			if (propName == nameof(item.ComparisonMethod))
			{
				var value = (TucFilterTextComparisonMethod)valueObj;
				item.ComparisonMethod = value;
				if (item.ComparisonMethod == original.ComparisonMethod) return;
				updateObj = item;
			}
			else if (propName == nameof(item.Value))
			{
				var value = (string)valueObj;
				item.ValueChanged(value);
				if (item.Value == original.Value) return;
				updateObj = item;
			}
			else throw new Exception("propName not supported");
		}
		else throw new Exception("Unsupported TucFilterBase in _valueChanged");

		await TfSpaceDataManage.UpdateColumnFilter(updateObj);
		await InvokeAsync(StateHasChanged);
	}


}
