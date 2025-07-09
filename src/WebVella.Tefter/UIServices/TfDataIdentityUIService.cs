namespace WebVella.Tefter.UIServices;

public partial interface ITfDataIdentityUIService
{
	//Events
	event EventHandler<TfDataIdentity> DataIdentityCreated;
	event EventHandler<TfDataIdentity> DataIdentityUpdated;
	event EventHandler<TfDataIdentity> DataIdentityDeleted;

	//Data provider
	TfDataIdentity GetDataIdentity(string identity);
	List<TfDataIdentity> GetDataIdentities(string? search = null);
	TfDataIdentity CreateDataIdentity(TfDataIdentity identity);
	TfDataIdentity UpdateDataIdentity(TfDataIdentity identity);
	void DeleteDataIdentity(string identity);
}
public partial class TfDataIdentityUIService : ITfDataIdentityUIService
{
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfDataIdentityUIService> LOC;

	public TfDataIdentityUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfDataIdentityUIService>>() ?? default!;
	}

	#region << Events >>
	public event EventHandler<TfDataIdentity> DataIdentityCreated = default!;
	public event EventHandler<TfDataIdentity> DataIdentityUpdated = default!;
	public event EventHandler<TfDataIdentity> DataIdentityDeleted = default!;
	#endregion

	#region << Data Provider>>
	public List<TfDataIdentity> GetDataIdentities(string? search = null)
		=> _tfService.GetDataIdentities(search);
	public TfDataIdentity GetDataIdentity(string identity)
		=> _tfService.GetDataIdentity(identity);
	public TfDataIdentity CreateDataIdentity(TfDataIdentity identity)
	{
		var dataIdentity = _tfService.CreateDataIdentity(identity);
		DataIdentityCreated?.Invoke(this, dataIdentity);
		return dataIdentity;
	}
	public TfDataIdentity UpdateDataIdentity(TfDataIdentity identity)
	{
		var dataIdentity = _tfService.UpdateDataIdentity(identity);
		DataIdentityUpdated?.Invoke(this, dataIdentity);
		return dataIdentity;
	}
	public void DeleteDataIdentity(string identity)
	{
		var deletedIdentity = GetDataIdentity(identity);
		_tfService.DeleteDataIdentity(identity);
		DataIdentityDeleted?.Invoke(this, deletedIdentity);
	}
	#endregion


}
