namespace WebVella.Tefter.UI.Components;

public partial class TucPresetFilterManageDialog : TfFormBaseComponent,
	IDialogContentComponent<TfPresetFilterManagementContext>
{
	[Parameter] public TfPresetFilterManagementContext Content { get; set; } = null!;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private TfSpaceViewPreset _form = new();
	private TfSpaceViewPreset? _selectedParent = null;
	private bool _isSubmitting = false;
	private bool _isCreate = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = null!;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.SpaceView is null) throw new Exception("SpaceView is null");

		if (Content.Item is not null)
		{
			_form = new TfSpaceViewPreset
			{
				Id = Content.Item.Id,
				IsGroup = Content.Item.IsGroup,
				Name = Content.Item.Name,
				Search = Content.Item.Search,
				Filters = Content.Item.Filters.ToList(),
				SortOrders = Content.Item.SortOrders.ToList(),
				Presets = Content.Item.Presets.ToList(),
				ParentId = Content.Item.ParentId,
				Color = Content.Item.Color,
				Icon = Content.Item.Icon
			};
			if (_form.ParentId != null)
			{
				_selectedParent = Content.SpaceView.PresetsWithParents.FirstOrDefault(x => x.Id == _form.ParentId);
			}
		}
		else
		{
			_isCreate = true;
			var navState = TfAuthLayout.GetState().NavigationState;
			var viewColumns = TfService.GetSpaceViewColumnsList(Content.SpaceView!.Id);
			var providers = TfService.GetDataProviders();
			var search = navState.Search;
			var sharedColumns = TfService.GetSharedColumns();
			var filters = navState.Filters.ConvertQueryFilterToList(
				viewColumns: viewColumns,
				providers: providers.ToList(),
				sharedColumns: sharedColumns
			);
			var sorts = navState.Sorts.ConvertQuerySortToList(columns: viewColumns);

			//join any preset settings
			if (navState.SpaceViewPresetId is not null)
			{
				var preset = Content.SpaceView.Presets.FirstOrDefault(x => x.Id == navState.SpaceViewPresetId.Value);
				if (preset is not null)
				{
					if (!String.IsNullOrWhiteSpace(preset.Search))
					{
						search = search.JoinUserSearchQueries(preset.Search);
					}

					if (preset.Filters.Count > 0)
					{
						if (filters.Count == 0)
						{
							filters = preset.Filters;
						}
						else
						{
							var group1 = new TfFilterAnd(filters.ToArray());
							var group2 = new TfFilterAnd(preset.Filters.ToArray());
							filters =
							[
								group1,
								group2
							];
						}
					}

					if (preset.SortOrders.Count > 0)
					{
						if (sorts.Count == 0)
						{
							sorts = preset.SortOrders;
						}
						else
						{
							var newSorts = new List<TfSort>();
							foreach (var item in preset.SortOrders)
							{
								//new order can override the old one
								if (sorts.All(x => x.ColumnName != item.ColumnName))
									newSorts.Add(item);
							}

							foreach (var item in sorts)
								newSorts.Add(item);
							sorts = newSorts;
						}
					}
				}
			}

			_form = new TfSpaceViewPreset
			{
				Id = Guid.Empty,
				IsGroup = false,
				Name = String.Empty,
				Search = search,
				Filters = filters,
				SortOrders = sorts,
				Presets = new(),
				ParentId = null,
				Color = null,
				Icon = string.Empty,
			};
		}

		_title = _isCreate ? LOC("Create Preset Filter") : LOC("Manage Preset Filter");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add")! : TfConstants.GetIcon("Save")!;		
		InitForm(_form);
	}


	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _save()
	{
		MessageStore.Clear();
		if (string.IsNullOrWhiteSpace(_form.Name) && string.IsNullOrWhiteSpace(_form.Icon))
			MessageStore.Add(EditContext.Field(nameof(_form.Name)), LOC("required when icon is not provided"));

		if (!EditContext.Validate()) return;

		_form.ParentId = _selectedParent?.Id;
		try
		{
			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);
			//Create
			if (_form.Id == Guid.Empty)
			{
				_form.Id = Guid.NewGuid();
				await TfService.AddSpaceViewPreset(
					spaceViewId: Content.SpaceView!.Id,
					preset: _form);
				ToastService.ShowSuccess(LOC("Preset Filter created!"));
			}
			//Update
			else
			{
				await TfService.UpdateSpaceViewPreset(
					spaceViewId: Content.SpaceView!.Id,
					preset: _form);					
				ToastService.ShowSuccess(LOC("Preset Filter updated!"));
			}
			await Dialog.CloseAsync(_form);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}