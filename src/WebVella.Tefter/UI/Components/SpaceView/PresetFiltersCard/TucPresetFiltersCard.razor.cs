namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.PresetFiltersCard.TucPresetFiltersCard", "WebVella.Tefter")]
public partial class TucPresetFiltersCard : TfBaseComponent
{
	[Parameter]
	public TfDataProvider DataProvider { get; set; } = default!;

	[Parameter]
	public List<TfSpaceViewPreset> Items { get; set; } = new();

	[Parameter]
	public EventCallback<List<TfSpaceViewPreset>> ItemsChanged { get; set; }

	[Parameter]
	public string? Title { get; set; } = null;

	public bool _submitting = false;
	public TfPresetFilterItemType _selectedType = TfPresetFilterItemType.PresetFilter;
	public TfSpaceViewPreset? _selectedParent = null;
	public string? _selectedName = null;

	private async Task _addPreset()
	{
		if (String.IsNullOrEmpty(_selectedName))
		{
			ToastService.ShowWarning(LOC("Please select a name"));
			return;
		}

		_submitting = true;
		await InvokeAsync(StateHasChanged);
		var preset = new TfSpaceViewPreset
		{
			Id = Guid.NewGuid(),
			ParentId = _selectedParent is null ? null : _selectedParent.Id,
			Filters = new(),
			SortOrders = new(),
			IsGroup = _selectedType == TfPresetFilterItemType.Group,
			Name = _selectedName,
			Pages = new()
		};

		if (_selectedParent is not null)
		{
			TfSpaceViewPreset parentNode = ModelHelpers.GetPresetById(Items, _selectedParent.Id);
			if (parentNode is not null)
				parentNode.Pages.Add(preset);
		}
		else
		{
			Items.Add(preset);
		}

		await ItemsChanged.InvokeAsync(Items);
		_submitting = false;
		_selectedName = null;
		await InvokeAsync(StateHasChanged);
	}

	private async Task _removePreset(Guid nodeId)
	{
		Items = _removeNode(Items, nodeId);
		_submitting = true;
		await InvokeAsync(StateHasChanged);
		await ItemsChanged.InvokeAsync(Items);
		_submitting = false;
		await InvokeAsync(StateHasChanged);
	}

	private async Task _movePreset(Tuple<Guid, bool> args)
	{
		Items = _moveNode(Items, args.Item1, args.Item2);
		_submitting = true;
		await InvokeAsync(StateHasChanged);
		await ItemsChanged.InvokeAsync(Items);
		_submitting = false;
		await InvokeAsync(StateHasChanged);
	}

	private async Task _copyPreset(Guid presetId)
	{
		TfSpaceViewPreset source = ModelHelpers.GetPresetById(Items, presetId);
		if (source is null) return;
		if (source.ParentId is not null)
		{
			TfSpaceViewPreset parent = ModelHelpers.GetPresetById(Items, source.ParentId.Value);
			if (parent is null) return;

			var sourceIndex = parent.Pages.FindIndex(x => x.Id == source.Id);
			if (sourceIndex > -1)
			{
				parent.Pages.Insert(sourceIndex + 1, _copyNode(source, parent.Id));
			}
		}
		else
		{
			var sourceIndex = Items.FindIndex(x => x.Id == source.Id);
			if (sourceIndex > -1)
			{
				Items.Insert(sourceIndex + 1, _copyNode(source, null));
			}
		}

		_submitting = true;
		await InvokeAsync(StateHasChanged);
		await ItemsChanged.InvokeAsync(Items);
		_submitting = false;
		await InvokeAsync(StateHasChanged);
	}

	private async Task _editPreset(Guid presetId)
	{
		var context = new TfPresetFilterManagementContext
		{
			Item = ModelHelpers.GetPresetById(Items, presetId),
			Parents = _getParents().ToList(),
			DataProvider = DataProvider
		};
		var dialog = await DialogService.ShowDialogAsync<TucPresetFilterManageDialog>(
		context,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthExtraLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var record = (TfSpaceViewPreset)result.Data;
			var currentValue = ModelHelpers.GetPresetById(Items, record.Id);
			var currentParentId = currentValue.ParentId;
			if (currentValue is not null)
			{
				currentValue.Name = record.Name;
				currentValue.Filters = record.Filters.ToList();
				currentValue.SortOrders = record.SortOrders.ToList();
				currentValue.ParentId = record.ParentId;
				currentValue.Icon = record.Icon;
				currentValue.Color = record.Color;
			}
			if (currentParentId != record.ParentId)
			{
				TfSpaceViewPreset? currentParent = null;
				TfSpaceViewPreset? newParent = null;
				if (currentParentId.HasValue) currentParent = ModelHelpers.GetPresetById(Items, currentParentId.Value);
				if (record.ParentId.HasValue) newParent = ModelHelpers.GetPresetById(Items, record.ParentId.Value);

				if (currentParent is not null)
				{
					var findIndex = currentParent.Pages.FindIndex(x => x.Id == record.Id);
					if (findIndex > -1) currentParent.Pages.RemoveAt(findIndex);
				}
				else
				{
					var findIndex = Items.FindIndex(x => x.Id == record.Id);
					if (findIndex > -1) Items.RemoveAt(findIndex);
				}

				if (newParent is not null)
				{
					newParent.Pages.Add(currentValue);
				}
				else
				{
					Items.Add(currentValue);
				}
			}

			_submitting = true;
			await InvokeAsync(StateHasChanged);
			await ItemsChanged.InvokeAsync(Items);
			_submitting = false;
			await InvokeAsync(StateHasChanged);
			ToastService.ShowSuccess(LOC("Preset Filter updated!"));

		}
	}

	private IEnumerable<TfSpaceViewPreset> _getParents()
	{
		var parents = new List<TfSpaceViewPreset>();
		foreach (var item in Items)
		{
			_fillParents(parents, item);
		}
		return parents.AsEnumerable();
	}

	private void _fillParents(List<TfSpaceViewPreset> parents, TfSpaceViewPreset current)
	{
		if (current.IsGroup) parents.Add(current);
		foreach (var item in current.Pages) _fillParents(parents, item);
	}

	private List<TfSpaceViewPreset> _removeNode(List<TfSpaceViewPreset> nodes, Guid nodeId)
	{
		if (nodes.Count == 0) return nodes;
		if (nodes.Any(x => x.Id == nodeId))
		{
			return nodes.Where(x => x.Id != nodeId).ToList();
		}
		foreach (var item in nodes)
		{
			item.Pages = _removeNode(item.Pages, nodeId);
		}

		return nodes;
	}

	private List<TfSpaceViewPreset> _moveNode(List<TfSpaceViewPreset> nodes, Guid nodeId, bool isUp)
	{
		if (nodes.Count == 0) return nodes;

		var nodeIndex = nodes.FindIndex(x => x.Id == nodeId);
		if (nodeIndex > -1)
		{
			var list = nodes.Where(x => x.Id != nodeId).ToList();
			var newIndex = isUp ? nodeIndex - 1 : nodeIndex + 1;
			if (newIndex < 0 || newIndex > nodes.Count - 1) return nodes;

			list.Insert(newIndex, nodes[nodeIndex]);
			return list;
		}

		foreach (var item in nodes)
		{
			item.Pages = _moveNode(item.Pages, nodeId, isUp);
		}

		return nodes;
	}

	private TfSpaceViewPreset _copyNode(TfSpaceViewPreset item,Guid? parentId = null)
	{
		var newItem = item with { Id = Guid.NewGuid(), ParentId = parentId };
		var newNodes = new List<TfSpaceViewPreset>();
		foreach (var node in item.Pages)
		{
			newNodes.Add(_copyNode(node, newItem.Id));
		}
		newItem.Pages = newNodes;
		return newItem;
	}
}

public enum TfPresetFilterItemType
{
	[Description("preset filter")]
	PresetFilter = 0,
	[Description("filter group")]
	Group = 1
}
