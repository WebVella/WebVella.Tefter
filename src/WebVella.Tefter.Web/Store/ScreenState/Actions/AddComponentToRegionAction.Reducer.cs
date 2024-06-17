namespace WebVella.Tefter.Web.Store.ScreenState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static ScreenState AddComponentToRegion(ScreenState state, AddComponentToRegionAction action)
	{
		if (action.ScreenComponents is null || action.ScreenComponents.Count == 0) return state;
		foreach (var comp in action.ScreenComponents)
		{
			switch (comp.Region)
			{
				case ScreenRegionType.SpaceViewActions:
					{
						if (state.SpaceViewActionsRegion.Any(x => x.Id == comp.Id)) return state;
						var list = state.SpaceViewActionsRegion;
						list.Add(comp);
						list = list.OrderBy(x => x.Position).ToList();

						state = state with
						{
							SpaceViewActionsRegion = list,
							SpaceViewActionsRegionTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
						};
					}
					break;
				case ScreenRegionType.SpaceViewMenuItems:
					{
						if (state.SpaceViewMenuItemsRegion.Any(x => x.Id == comp.Id)) return state;
						var list = state.SpaceViewMenuItemsRegion;
						list.Add(comp);
						list = list.OrderBy(x => x.Position).ToList();

						state = state with
						{
							SpaceViewMenuItemsRegion = list,
							SpaceViewMenuItemsRegionTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
						};
					}
					break;
				default:
					break;
			}
		}

		return state;
	}
}
