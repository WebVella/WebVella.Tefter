namespace WebVella.Tefter.Web.Components;

using System;
using WebVella.Tefter.Web.Store.UserState;
public partial class TfLogin : TfBaseComponent
{
	[Inject] protected IState<UserState> UserState { get; set; }

	private string _email = "boz@zashev.com";
	private string _password = "1232";

    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            UserState.StateChanged -= UserState_StateChnanged;
        }
        await base.DisposeAsyncCore(disposing);
    }

	private void _login()
	{
		Dispatcher.Dispatch(new LoginUserAction(_email, _password));
		UserState.StateChanged += UserState_StateChnanged;

	}

	private void UserState_StateChnanged(object sender, EventArgs e)
	{
		InvokeAsync(() =>
		{
			if (UserState.Value.IsLoading) return;

			if (UserState.Value.User is not null)
				Navigator.NavigateTo(TfConstants.HomePageUrl);
		});
	}
}