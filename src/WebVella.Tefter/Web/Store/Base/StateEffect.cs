namespace WebVella.Tefter.Web.Store;
public partial class TfStateEffect
{
	public readonly StateEffectsUseCase UseCase;
	public readonly IToastService ToastService;
	public readonly IMessageService MessageService;

	public TfStateEffect(
		StateEffectsUseCase useCase, 
		IToastService toastService, 
		IMessageService messageService)
	{
		UseCase = useCase;
		ToastService = toastService;
		MessageService = messageService;
	}
}
