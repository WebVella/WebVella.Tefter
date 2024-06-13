namespace WebVella.Tefter.Web.Components;

using System;
using WebVella.Tefter.Web.Store.UserState;
public partial class TfLogin : TfBaseComponent
{
	[Inject] protected IState<UserState> UserState { get; set; }

	private string _email = "boz@zashev.com";
	private string _password = "1232";
	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		UserState.StateChanged -= UserState_StateChnanged;
		return base.DisposeAsyncCore(disposing);
	}
	private void _login(){
		Dispatcher.Dispatch(new LoginUserAction(_email,_password));
		UserState.StateChanged += UserState_StateChnanged;

	}

	private void UserState_StateChnanged(object sender, EventArgs e)
	{
		if(UserState.Value.IsLoading) return;

		if(UserState.Value.User is not null)
			Navigator.NavigateTo(TfConstants.HomePageUrl);
	}
}