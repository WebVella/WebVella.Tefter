namespace WebVella.Tefter.Demo.Components;
public partial class WvState : ComponentBase
{
	
	public void SetSpaceViewBookmarkState(Guid id, bool isBookmark){ 
		var newSpaceView =  WvService.SetSpaceViewBookmarkState(id, isBookmark);
		_spaceViewDict[newSpaceView.Id] = newSpaceView;
		if (newSpaceView.Id == _activeSpaceViewId){
			var meta = GetActiveSpaceMeta();
			ActiveSpaceDataChanged?.Invoke(this, new StateActiveSpaceDataChangedEventArgs
			{
				Space = meta.Space,
				SpaceData = meta.SpaceData,
				SpaceView = meta.SpaceView,
			});
		}


	}
}
