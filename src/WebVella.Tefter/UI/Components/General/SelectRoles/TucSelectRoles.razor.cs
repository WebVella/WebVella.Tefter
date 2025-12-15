namespace WebVella.Tefter.UI.Components;

public partial class TucSelectRoles : TfBaseComponent
{
	[Parameter]
	public List<Guid> Values { get; set; } = new();

	[Parameter]
	public EventCallback<List<Guid>> ValuesChanged { get; set; }

	[Parameter]
	public bool ReadOnly { get; set; } = false;

	private List<Guid> _values = new();
	private List<TfRole> _allRoles = new();
	private Dictionary<Guid, TfRole> _roleDict = new();
	private List<TfRole> _options = new();
	private TfRole? _selectedRole = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var allRoles = TfService.GetRoles();
		_roleDict = allRoles.ToDictionary(x => x.Id);
		_allRoles = allRoles.Where(x => x.Id != TfConstants.ADMIN_ROLE_ID).ToList();
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		_init();
	}

	private void _init()
	{
		if (ReadOnly) return;
		_values = Values.ToList();
		_selectedRole = null;
		_options = _allRoles.Where(x => !Values.Contains(x.Id)).ToList();
	}
	private void _checkFixMissingRoles()
	{
		_values = _values.Where(x=> _roleDict.ContainsKey(x)).ToList();
	}

	private async Task _addOption(TfRole role)
	{
		if (_values.Contains(role.Id)) return;
		_checkFixMissingRoles();
		_values.Add(role.Id);
		await ValuesChanged.InvokeAsync(_values);
	}
	private async Task _removeOption(string type, Guid roleId)
	{
		if (!_values.Contains(roleId)) return;
		_checkFixMissingRoles();
		_values.Remove(roleId);
		await ValuesChanged.InvokeAsync(_values);
	}

}