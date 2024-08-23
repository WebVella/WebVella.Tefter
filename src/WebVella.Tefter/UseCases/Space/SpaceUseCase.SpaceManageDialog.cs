namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	internal TucSpace SpaceManageForm { get; set; }
	internal List<string> AllIcons { get; set; } = new();
	internal Task InitSpaceManageDialog()
	{
		if (AllIcons.Count == 0)
		{
			foreach (var item in Icons.AllIcons)
			{
				if (item.Size == TfConstants.IconSize
				&& item.Variant == TfConstants.IconVariant
				&& item.Name.Length <= 5)
					AllIcons.Add(item.Name);
			}
		}

		SpaceManageForm = new();
		return Task.CompletedTask;
	}

}