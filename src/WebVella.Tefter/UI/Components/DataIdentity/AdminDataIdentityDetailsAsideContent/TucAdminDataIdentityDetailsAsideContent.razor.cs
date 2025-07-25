﻿namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataIdentityDetailsAsideContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfDataIdentityUIService TfDataIdentityUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfDataIdentityUIService.DataIdentityCreated -= On_DataIdentityCreated;
		TfDataIdentityUIService.DataIdentityUpdated -= On_DataIdentityUpdated;
		TfDataIdentityUIService.DataIdentityDeleted -= On_DataIdentityDeleted;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		TfDataIdentityUIService.DataIdentityCreated += On_DataIdentityCreated;
		TfDataIdentityUIService.DataIdentityUpdated += On_DataIdentityUpdated;
		TfDataIdentityUIService.DataIdentityDeleted += On_DataIdentityDeleted;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_DataIdentityCreated(object? caller, TfDataIdentity user)
	{
		await _init();
	}

	private async void On_DataIdentityUpdated(object? caller, TfDataIdentity user)
	{
		await _init();
	}

	private async void On_DataIdentityDeleted(object? caller, TfDataIdentity user)
	{
		await _init();
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		try
		{
			_search = navState.SearchAside;
			var roles = TfDataIdentityUIService.GetDataIdentities(_search).ToList();
			_items = new();
			foreach (var role in roles)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.AdminDataIdentityDetailsPageUrl, role.DataIdentity),
					Description = role.IsSystem ? "system created" : "user created",
					Text = TfConverters.StringOverflow(role.DataIdentity, _stringLimit),
					Selected = navState.DataIdentityId == role.DataIdentity
				});
			}
		}
		finally
		{
			UriInitialized = navState.Uri;
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}