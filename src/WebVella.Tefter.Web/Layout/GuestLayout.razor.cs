using WebVella.Tefter.Web.Store.UserState;

namespace WebVella.Tefter.Web.Layout;
public partial class GuestLayout : FluxorLayout
{
	[Inject] protected ITfService TfService { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] protected IState<UserState> UserState { get; set; }
	[Inject] protected IDispatcher dispatcher { get; set; }

	private bool _firstRender = true;
	private bool _isLoading = true;

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		UserState.StateChanged -= UserState_StateChanged;
		return base.DisposeAsyncCore(disposing);
	}

	// Overridden to provide an async method that is run before any rendering
	public override async Task SetParametersAsync(ParameterView parameters)
	{
		// Apply the supplied Parameters to the Conponent
		// Must do this first
		parameters.SetParameterProperties(this);

		// Run the Pre Render code
		if (_firstRender)
		{
			await this.PreFirstRenderAsync();
			_firstRender = false;
		}

		// Run the base SetParametersAsync providing it with an empty ParameterView
		// We have already applied the parameters and then may already be stale
		// This runs the normal lifecycle methods
		await base.SetParametersAsync(ParameterView.Empty);
	}

	// Separate out this component's code to make it clear what we're doing
	// We use a ValueTask because it's cheaper if no real async await occurs
	public ValueTask PreFirstRenderAsync()
	{
		// Your async code goes here
		// You can run sync code too if you wish
		dispatcher.Dispatch(new GetUserAction());
		UserState.StateChanged += UserState_StateChanged;
		return ValueTask.CompletedTask;
	}

	private void UserState_StateChanged(object sender, EventArgs e)
	{
		InvokeAsync(async() =>
		{
			if (UserState.Value.IsLoading) return;

			if (UserState.Value.User is not null)
			{
				_isLoading = true;
				Navigator.NavigateTo(TfConstants.HomePageUrl);
				return;
			}
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		});
	}

}