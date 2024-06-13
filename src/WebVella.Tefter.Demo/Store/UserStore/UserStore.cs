namespace WebVella.Tefter.Demo.Store;

[FeatureState]
public record UserStore
{
	public bool IsLoading { get; init; }
	public bool IsSidebarExpanded { get; init; } = true;
	public User User { get; init; }

	private UserStore(){ }



	//Different constructors to create the State?
	//public UserState(User user)
	//{
	//	Id = user.Id;
	//	FirstName = user.FirstName;
	//	LastName = user.LastName;	
	//	Email = user.Email;
	//	ThemeMode = user.ThemeMode;
	//	ThemeColor = user.ThemeColor;
	//	Initials = user.Initials;
	//}
}
