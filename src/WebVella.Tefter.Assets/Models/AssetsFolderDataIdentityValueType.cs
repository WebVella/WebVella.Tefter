using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Assets.Models;

public enum AssetsFolderDataIdentityValueType
{
	[Description("connect to page")]
	SpaceNodeId = 0,
	[Description("connect to space")]
	SpaceId = 1,
	[Description("connect to custom string")]
	CustomString = 2
}
