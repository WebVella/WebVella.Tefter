namespace WebVella.Tefter.Demo.Components;
public partial class WvSpace : WvBaseComponent,IDisposable
{
	private IQueryable<DataSource> _dataSource = default!;
	private bool _isLoading = true;

	public void Dispose()
	{
		WvState.ActiveSpaceDataChanged -= onSpaceDataLocation;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_dataSource = SampleData.GetDataSource().AsQueryable();
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
			WvState.ActiveSpaceDataChanged+= onSpaceDataLocation;
		}
	}

	protected void onSpaceDataLocation(object sender, StateActiveSpaceDataChangedEventArgs e)
	{
		_isLoading = true;
		StateHasChanged();

		_dataSource = SampleData.GetDataSource().AsQueryable();
		_isLoading = false;
		StateHasChanged();
	}
	
}