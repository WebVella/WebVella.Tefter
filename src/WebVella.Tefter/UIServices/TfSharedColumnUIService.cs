namespace WebVella.Tefter.UIServices;

public partial interface ITfSharedColumnUIService
{
	//Events
	event EventHandler<TfSharedColumn> SharedColumnCreated;
	event EventHandler<TfSharedColumn> SharedColumnUpdated;
	event EventHandler<TfSharedColumn> SharedColumnDeleted;

	//Shared Column
	List<TfSharedColumn> GetSharedColumns(string? search = null);
	TfSharedColumn GetSharedColumn(Guid id);
	TfSharedColumn CreateSharedColumn(TfSharedColumn identity);
	TfSharedColumn UpdateSharedColumn(TfSharedColumn identity);
	void DeleteSharedColumn(Guid id);
	ReadOnlyCollection<TfDataProvider> GetSharedColumnConnectedDataProviders(Guid columnId);

}
public partial class TfSharedColumnUIService : ITfSharedColumnUIService
{
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfSharedColumnUIService> LOC;

	public TfSharedColumnUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfSharedColumnUIService>>() ?? default!;
	}

	#region << Events >>
	public event EventHandler<TfSharedColumn> SharedColumnCreated = default!;
	public event EventHandler<TfSharedColumn> SharedColumnUpdated = default!;
	public event EventHandler<TfSharedColumn> SharedColumnDeleted = default!;
	#endregion

	#region << Shared Column >>
	public List<TfSharedColumn> GetSharedColumns(string? search = null)
		=> _tfService.GetSharedColumns(search);
	public TfSharedColumn GetSharedColumn(Guid id)
		=> _tfService.GetSharedColumn(id);
	public TfSharedColumn CreateSharedColumn(TfSharedColumn item)
	{
		var column = _tfService.CreateSharedColumn(item);
		SharedColumnCreated?.Invoke(this, column);
		return column;
	}
	public TfSharedColumn UpdateSharedColumn(TfSharedColumn item)
	{
		var column = _tfService.UpdateSharedColumn(item);
		SharedColumnUpdated?.Invoke(this, column);
		return column;
	}
	public void DeleteSharedColumn(Guid id)
	{
		var column = GetSharedColumn(id);
		_tfService.DeleteSharedColumn(id);
		SharedColumnDeleted?.Invoke(this, column);
	}


	public ReadOnlyCollection<TfDataProvider> GetSharedColumnConnectedDataProviders(Guid columnId)
		=> _tfService.GetSharedColumnConnectedDataProviders(columnId);
	#endregion


}
