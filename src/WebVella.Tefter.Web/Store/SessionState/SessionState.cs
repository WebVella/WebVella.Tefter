namespace WebVella.Tefter.Web.Store.SessionState;

[FeatureState]
public record SessionState
{
    public bool IsSidebarExpanded { get; init; }

	public SessionState() { 
		IsSidebarExpanded = true;
	}

    public SessionState(User user)
    {
		IsSidebarExpanded = true;
	}
}
