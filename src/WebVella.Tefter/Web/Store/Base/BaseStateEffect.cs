namespace WebVella.Tefter.Web.Store.Base;
public partial class StateEffect
{
	public readonly StateEffectsUseCase UseCase;

	public StateEffect(StateEffectsUseCase useCase)
	{
		UseCase = useCase;
	}
}
