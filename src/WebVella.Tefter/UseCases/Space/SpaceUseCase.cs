using WebVella.Tefter.Web.Components.SpaceManageDialog;
using WebVella.Tefter.Web.Components.SpaceStateManager;

namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	private readonly IIdentityManager _identityManager;
	private readonly ITfSpaceManager _spaceManager;
	private readonly NavigationManager _navigationManager;
	private readonly IDispatcher _dispatcher;
	private readonly IToastService _toastService;
	private readonly IMessageService _messageService;
	private readonly IStringLocalizer<SpaceUseCase> _loc;

	public SpaceUseCase(
			IIdentityManager identityManager,
			ITfSpaceManager spaceManager,
			NavigationManager navigationManager,
			IDispatcher dispatcher,
			IToastService toastService,
			IMessageService messageService,
			IStringLocalizer<SpaceUseCase> loc
			)
	{
		_identityManager = identityManager;
		_spaceManager = spaceManager;
		_navigationManager = navigationManager;
		_dispatcher = dispatcher;
		_toastService = toastService;
		_messageService = messageService;
		_loc = loc;
	}

	internal async Task Init(Type type)
	{
		if(type == typeof(TfSpaceStateManager)) await InitForState();
		else if(type == typeof(TfSpaceManageDialog)) await InitSpaceManageDialog();
		else throw new Exception($"Type: {type.Name} not supported in SpaceUseCase");
	}

	internal Result<TucSpace> CreateSpaceWithForm(TucSpace space) {
		var result = _spaceManager.CreateSpace(space.ToModel());
		if (result.IsFailed) return Result.Fail(new Error("CreateSpaceWithFormAsync failed").CausedBy(result.Errors));
		return Result.Ok(new TucSpace(result.Value));
	}

	internal Result<TucSpace> UpdateSpaceWithForm(TucSpace space) {
		var result = _spaceManager.UpdateSpace(space.ToModel());
		if (result.IsFailed) return Result.Fail(new Error("UpdateSpaceWithForm failed").CausedBy(result.Errors));
		return Result.Ok(new TucSpace(result.Value));
	}
}
