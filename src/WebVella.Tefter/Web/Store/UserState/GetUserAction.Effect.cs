namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
    [EffectMethod]
    public async Task HandleGetUserAction(GetUserAction action, IDispatcher dispatcher)
    {
        User user = null;
        if (action.UserId.HasValue)
        {
            var result = await IdentityManager.GetUserAsync(action.UserId.Value);
            if(result.IsSuccess) user = result.Value;
        }
        dispatcher.Dispatch(new SetUserAction(user));
    }

}

