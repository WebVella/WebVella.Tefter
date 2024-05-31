namespace WebVella.Tefter.Demo.Components;
public partial class WvLocation : WvBaseComponent, IDisposable
{
	private Space _space;
	private SpaceItem _spaceItem;
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
		Console.WriteLine($"**** {_space.Id} {_spaceItem.Id}");
		Console.WriteLine($"**** {_space.Name} {_spaceItem.Name}");
		StateHasChanged();
	}
}