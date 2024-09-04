namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	internal Type SpaceViewColumnComponent { get; set; }
	internal TucSpaceViewColumn SpaceViewColumnForm { get; set; }
	internal List<TucSpaceViewColumnType> AvailableColumnTypes { get; set; }
	internal Task InitSpaceViewColumnManage()
	{
		SpaceViewColumnForm = new();
		AvailableColumnTypes = _spaceManager.GetAvailableSpaceViewColumnTypes().Value.Select(x=> new TucSpaceViewColumnType(x)).ToList();


		return Task.CompletedTask;
	}


}