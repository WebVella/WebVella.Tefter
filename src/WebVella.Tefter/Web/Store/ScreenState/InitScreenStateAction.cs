using WebVella.Tefter.UseCases.State;

namespace WebVella.Tefter.Web.Store.ScreenState;

public record InitScreenStateAction
{
	internal StateUseCase UseCase { get;}
	public bool SidebarExpanded { get; }

	internal InitScreenStateAction(
		StateUseCase useCase,
		bool sidebarExpanded)
	{
		UseCase = useCase;
		SidebarExpanded = sidebarExpanded;
	}
}
