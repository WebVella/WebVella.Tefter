namespace WebVella.Tefter.Demo.Components;
public partial class WvLocation : WvBaseComponent, IDisposable
{
	private Space _space;
	private SpaceItem _spaceItem;
	private bool _settingsMenuVisible = false;
	public void Dispose()
	{
		WvState.ActiveSpaceDataChanged -= OnSpaceDataChanged;
	}

	protected override void OnInitialized()
	{
		WvState.ActiveSpaceDataChanged += OnSpaceDataChanged;
	}

	protected void OnSpaceDataChanged(object sender, StateActiveSpaceDataChangedEventArgs args)
	{
		_space = args.Space;
		_spaceItem = args.SpaceItem;
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