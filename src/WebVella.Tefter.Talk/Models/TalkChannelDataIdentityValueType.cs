using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Talk.Models;

public enum TalkChannelDataIdentityValueType
{
	[Description("connect to page")]
	SpacePageId = 0,
	[Description("connect to space")]
	SpaceId = 1,
	[Description("connect to custom string")]
	CustomString = 2
}
