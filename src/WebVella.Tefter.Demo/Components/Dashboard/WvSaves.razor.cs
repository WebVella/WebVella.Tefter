
namespace WebVella.Tefter.Demo.Components;
public partial class WvSaves : WvBaseComponent
{
	private List<SpaceView> _saves;
	[Inject] protected IState<UserStore> UserState { get; set; }


}