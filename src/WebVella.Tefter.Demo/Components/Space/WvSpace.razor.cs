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
			loadData();
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
			WvState.ActiveSpaceDataChanged+= onSpaceDataLocation;
		}
	}

	protected void onSpaceDataLocation(object sender, StateActiveSpaceDataChangedEventArgs e)
	{
		_isLoading = true;
		StateHasChanged();

		loadData();
		_isLoading = false;
		StateHasChanged();
	}
	private void loadData(){
		var alldata = SampleData.GetDataSource().AsQueryable();
		_dataSource = alldata.AsQueryable();
	}
}