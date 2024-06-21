namespace WebVella.Tefter.Web.Store.SessionState;

public partial class SessionStateEffects
{
    private readonly ITfService TfService;

    public SessionStateEffects(ITfService tfService)
    {
        this.TfService = tfService;
    }
}

