namespace WebVella.Tefter.UI.Components;
public partial class TucPresetFilterManageDialog : TfFormBaseComponent, IDialogContentComponent<TfPresetFilterManagementContext>
{
	[Parameter] public TfPresetFilterManagementContext Content { get; set; } = null!;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private TfSpaceViewPreset _form = new();
	private TfSpaceViewPreset? _selectedParent = null;
	private bool _isSubmitting = false;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if(Content is null) throw new Exception("Content is null");
		if(Content.SpaceView is null) throw new Exception("SpaceView is null");
		
		if (Content.Item is not null)
		{
			_form = new TfSpaceViewPreset
			{
				Id = Content.Item.Id,
				IsGroup = Content.Item.IsGroup,
				Name = Content.Item.Name,
				Search = null,
				Filters = Content.Item.Filters.ToList(),
				SortOrders = Content.Item.SortOrders.ToList(),
				Presets = Content.Item.Presets.ToList(),
				ParentId = Content.Item.ParentId,
				Color = Content.Item.Color,
				Icon = Content.Item.Icon
			};
			if (_form.ParentId != null)
			{
				_selectedParent = Content.Parents.FirstOrDefault(x => x.Id == _form.ParentId);
			}
		}
		else
		{
			var navState = TfAuthLayout.GetState().NavigationState;
			var viewColumns = TfService.GetSpaceViewColumnsList(Content.SpaceView!.Id);
			var providers = TfService.GetDataProviders();
			var sharedColumns = TfService.GetSharedColumns();
			var filters = navState.Filters.ConvertQueryFilterToList(
				viewColumns:viewColumns,
				providers:providers.ToList(),
				sharedColumns:sharedColumns
				);
			var sorts = navState.Sorts.ConvertQuerySortToList(columns:viewColumns);
			_form = new TfSpaceViewPreset
			{
				Id = Guid.Empty,
				IsGroup = false,
				Name = String.Empty,
				Search = navState.Search,
				Filters = filters,
				SortOrders = sorts,
				Presets = new(),
				ParentId = null,
				Color = null,
				Icon = string.Empty,
				
			};						
		}

		base.InitForm(_form);
	}


	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _save()
	{
		if (_form.Id != Guid.Empty)
		{
			_form.ParentId = _selectedParent?.Id;
			await Dialog.CloseAsync(_form);
			return;
		}

		try
		{
			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);
			_form.Id = Guid.NewGuid();
			await TfService.AddSpaceViewPreset(
				spaceViewId: Content!.SpaceView!.Id,
				preset: _form);
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
