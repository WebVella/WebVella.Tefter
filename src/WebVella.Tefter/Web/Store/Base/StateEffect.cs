namespace WebVella.Tefter.Web.Store.Base;
public partial class StateEffect
{
	public readonly StateEffectsUseCase UseCase;
	public readonly IToastService ToastService;
	public readonly IMessageService MessageService;

	public StateEffect(
		StateEffectsUseCase useCase, 
		IToastService toastService, 
		IMessageService messageService)
	{
		UseCase = useCase;
		ToastService = toastService;
		MessageService = messageService;
	}
}
