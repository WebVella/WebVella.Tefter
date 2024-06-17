namespace WebVella.Tefter.Web.Store.ScreenState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static ScreenState RemoveComponentFromRegionAction(ScreenState state, RemoveComponentToRegionAction action)
	{
		switch (action.Region)
		{
			case ScreenRegionType.SpaceViewActions:
				{
					if (!state.SpaceViewActionsRegion.Any(x => x.Id == action.ScreenComponentId)) return state;
					var list = state.SpaceViewActionsRegion;
					list = list.Where(x => x.Id != action.ScreenComponentId).ToList();
					return state with
					{
						SpaceViewActionsRegion = list,
						SpaceViewActionsRegionTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
					};
				}
			case ScreenRegionType.SpaceViewMenuItems:
				{
					if (!state.SpaceViewMenuItemsRegion.Any(x => x.Id == action.ScreenComponentId)) return state;
					var list = state.SpaceViewMenuItemsRegion;
					list = list.Where(x => x.Id != action.ScreenComponentId).ToList();
					return state with
					{
						SpaceViewMenuItemsRegion = list,
						SpaceViewMenuItemsRegionTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
					};
				}
			default:
				break;
		}
		return state;
	}
}
