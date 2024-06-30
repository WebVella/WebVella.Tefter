using WebVella.Tefter.UseCases.State;

namespace WebVella.Tefter.Web.Store.UserState;

public record InitUserStateAction {

	internal StateUseCase UseCase { get; }
	public TucUser User { get; }
	internal InitUserStateAction(
		StateUseCase useCase,
		TucUser user)
	{
		UseCase = useCase;
		User = user;
	}
}
