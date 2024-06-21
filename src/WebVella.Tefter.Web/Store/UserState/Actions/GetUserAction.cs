namespace WebVella.Tefter.Web.Store.UserState;

public record GetUserAction
{
    public Guid? UserId { get; }

    public GetUserAction(Guid? userId)
    {
        UserId = userId;
    }
}
