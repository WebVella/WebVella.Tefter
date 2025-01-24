using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Assets.Models;

public enum AssetsFolderViewType
{
	[Description("Assets list")]
	List = 0,
	[Description("Single Assets")]
	Single = 1,
	[Description("Single Assets with attachments")]
	SingleWithExtras = 2
}
