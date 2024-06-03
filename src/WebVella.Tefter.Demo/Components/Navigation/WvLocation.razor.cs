namespace WebVella.Tefter.Demo.Components;
public partial class WvLocation : WvBaseComponent, IDisposable
{
	private Space _space;
	private SpaceItem _spaceItem;
	private SpaceItemView _spaceItemView;
	private bool _settingsMenuVisible = false;
	private int _ellipsisCount = 20;
	public void Dispose()
	{
		WvState.ActiveSpaceDataChanged -= OnSpaceDataChanged;
	}

	protected override void OnInitialized()
	{
		var (space, spaceItem, spaceItemView) = WvState.GetActiveSpaceData();
		_space = space;
		_spaceItem = spaceItem;
		_spaceItemView = spaceItemView;

		WvState.ActiveSpaceDataChanged += OnSpaceDataChanged;
	}

	protected void OnSpaceDataChanged(object sender, StateActiveSpaceDataChangedEventArgs args)
	{
		_space = args.Space;
		_spaceItem = args.SpaceItem;
		_spaceItemView = args.SpaceItemView;
		StateHasChanged();
	}

	private void onDetailsClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for details about the space item");
	}
	private void onRemoveClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for removing space item");
	}
	private void onRenameClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will show a dialog for renaming");
	}

	private void onAccessClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Shows current access");
	}


}