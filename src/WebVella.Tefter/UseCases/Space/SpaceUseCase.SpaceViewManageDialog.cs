namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	internal TucSpaceView SpaceViewManageForm { get; set; }
	internal Task InitSpaceViewManageDialog()
	{

		SpaceViewManageForm = new();
		return Task.CompletedTask;
	}

}