namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.QuickFiltersCard.TfQuickFiltersCard", "WebVella.Tefter")]
public partial class TfQuickFiltersCard : TfBaseComponent
{
	[Parameter]
	public TucDataProvider DataProvider { get; set; }

	[Parameter]
	public List<TucSpaceViewPreset> Items { get; set; } = new();

	[Parameter]
	public EventCallback<List<TucSpaceViewPreset>> ItemsChanged { get; set; }

	public bool _submitting = false;
	public TfQuickFilterItemType _selectedType = TfQuickFilterItemType.QuickFilter;
	public TucSpaceViewPreset _selectedParent = null;
	public string _selectedName = null;

	private async Task _addPreset()
	{
		if (String.IsNullOrEmpty(_selectedName))
		{
			ToastService.ShowWarning(LOC("Please select a name"));
			return;
		}

		_submitting = true;
		await InvokeAsync(StateHasChanged);
		var preset = new TucSpaceViewPreset
		{
			Id = Guid.NewGuid(),
			ParentId = _selectedParent is null ? null : _selectedParent.Id,
			Filters = new(),
			SortOrders = new(),
			IsGroup = _selectedType == TfQuickFilterItemType.Group,
			Name = _selectedName,
			Pages = new()
		};

		if (_selectedParent is not null)
		{
			TucSpaceViewPreset parentNode = ModelHelpers.GetPresetById(Items, _selectedParent.Id);
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
		TucSpaceViewPreset source = ModelHelpers.GetPresetById(Items, presetId);
		if (source is null) return;
		if (source.ParentId is not null)
		{
			TucSpaceViewPreset parent = ModelHelpers.GetPresetById(Items, source.ParentId.Value);
			if (parent is null) return;

			var sourceIndex = parent.Pages.FindIndex(x => x.Id == source.Id);
			if (sourceIndex > -1)
			{
				parent.Pages.Insert(sourceIndex + 1, _copyNode(source));
			}
		}
		else
		{
			var sourceIndex = Items.FindIndex(x => x.Id == source.Id);
			if (sourceIndex > -1)
			{
				Items.Insert(sourceIndex + 1, _copyNode(source));
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
		var context = new TucQuickFilterManagementContext
		{
			Item = ModelHelpers.GetPresetById(Items, presetId),
			Parents = _getParents().ToList(),
			DataProvider = DataProvider
		};
		var dialog = await DialogService.ShowDialogAsync<TfQuickFilterManageDialog>(
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
			var record = (TucSpaceViewPreset)result.Data;
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
				TucSpaceViewPreset currentParent = null;
				TucSpaceViewPreset newParent = null;
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
			ToastService.ShowSuccess(LOC("Quick Filter updated!"));

		}
	}

	private IEnumerable<TucSpaceViewPreset> _getParents()
	{
		var parents = new List<TucSpaceViewPreset>();
		foreach (var item in Items)
		{
			_fillParents(parents, item);
		}
		return parents.AsEnumerable();
	}

	private void _fillParents(List<TucSpaceViewPreset> parents, TucSpaceViewPreset current)
	{
		if (current.IsGroup) parents.Add(current);
		foreach (var item in current.Pages) _fillParents(parents, item);
	}

	private List<TucSpaceViewPreset> _removeNode(List<TucSpaceViewPreset> nodes, Guid nodeId)
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

	private List<TucSpaceViewPreset> _moveNode(List<TucSpaceViewPreset> nodes, Guid nodeId, bool isUp)
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

	private TucSpaceViewPreset _copyNode(TucSpaceViewPreset item)
	{
		var newItem = item with { Id = Guid.NewGuid() };
		var newNodes = new List<TucSpaceViewPreset>();
		foreach (var node in item.Pages)
		{
			newNodes.Add(_copyNode(node));
		}
		newItem.Pages = newNodes;
		return newItem;
	}
}

public enum TfQuickFilterItemType
{
	[Description("quick filter")]
	QuickFilter = 0,
	[Description("filter group")]
	Group = 1
}
