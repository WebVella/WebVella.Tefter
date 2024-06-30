using WebVella.Tefter.UseCases.State;

namespace WebVella.Tefter.Web.Store.CultureState;

public record SetCultureAction
{
	internal StateUseCase  UseCase { get;}
	public Guid UserId { get; }
	public TucCultureOption Culture { get; }

	internal SetCultureAction(
		StateUseCase  useCase,
		Guid userId,
		TucCultureOption culture)
	{
		UseCase = useCase;
		UserId = userId;
		Culture = culture;
	}
}
