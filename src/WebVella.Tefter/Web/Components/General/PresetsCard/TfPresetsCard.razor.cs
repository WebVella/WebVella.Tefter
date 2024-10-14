namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.PresetsCard.TfPresetsCard", "WebVella.Tefter")]
public partial class TfPresetsCard : TfBaseComponent
{
	[Parameter]
	public List<TucSpaceViewPreset> Items { get; set; } = new();

	[Parameter]
	public EventCallback<List<TucSpaceViewPreset>> ItemsChanged { get; set; }

	public bool _submitting = false;
	public TfPresetsCardType _selectedType = TfPresetsCardType.QuickFilter;
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
			Filters = new(),
			SortOrders = new(),
			IsGroup = _selectedType == TfPresetsCardType.Group,
			Name = _selectedName,
			Nodes = new()
		};

		if (_selectedParent is not null)
		{
			TucSpaceViewPreset parentNode = null;
			foreach (var item in Items)
			{
				parentNode = _findParent(item, _selectedParent.Id);
				if (parentNode is not null) break;
			}
			if (parentNode is not null)
				parentNode.Nodes.Add(preset);
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

	private async Task _editPreset(Guid presetId)
	{
		var context = new TucPresetManagementContext
		{
			Item = ModelHelpers.GetPresetById(Items, presetId),
			Parents = _getParents().ToList()
		};
		var dialog = await DialogService.ShowDialogAsync<TfPresetManageDialog>(
		context,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var record = (TucSpaceViewPreset)result.Data;
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
		foreach (var item in current.Nodes) _fillParents(parents, item);
	}

	private TucSpaceViewPreset _findParent(TucSpaceViewPreset current, Guid parentId)
	{
		if (current.Id == parentId)
		{
			return current;
		}
		foreach (var item in current.Nodes)
		{
			var parent = _findParent(item, parentId);
			if (parent is not null) return parent;
		}
		return null;
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
			item.Nodes = _removeNode(item.Nodes, nodeId);
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
			item.Nodes = _moveNode(item.Nodes, nodeId, isUp);
		}

		return nodes;
	}

}

public enum TfPresetsCardType
{
	[Description("quick filter")]
	QuickFilter = 0,
	[Description("filter group")]
	Group = 1
}
