namespace WebVella.Tefter.EmailSender.Models;
public enum EmailSenderLogDataIdentityValueType
{
	[Description("connect to space")]
	SpaceId = 0,
	[Description("connect to dataset")]
	Dataset = 1,
	[Description("connect to custom id")]
	CustomId = 2
}
