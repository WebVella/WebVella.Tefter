using WebVella.Tefter.UseCases.State;

namespace WebVella.Tefter.Web.Store.CultureState;

public record InitCultureStateAction
{
	internal StateUseCase UseCase { get; }
	public TucCultureOption Culture { get; }

	internal InitCultureStateAction(
		StateUseCase useCase,
		TucCultureOption culture)
	{
		UseCase = useCase;
		Culture = culture;
	}
}
