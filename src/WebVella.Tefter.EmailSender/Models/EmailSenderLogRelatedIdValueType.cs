namespace WebVella.Tefter.EmailSender.Models;
public enum EmailSenderLogRelatedIdValueType
{
	[Description("show all emails")]
	NoFilter = 0,	
	[Description("by related space")]
	SpaceId = 1,
	[Description("by related dataset")]
	DatasetId = 2,
	[Description("any related id")]
	AnyRelatedId = 3
}
