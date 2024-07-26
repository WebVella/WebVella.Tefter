﻿namespace WebVella.Tefter.UseCases.Models;
public enum TucSynchronizationStatus
{
	[Description("Pending")]
	Pending = 0,
	[Description("In Progress")]
	InProgress = 1,
	[Description("Completed")]
	Completed = 2,
	[Description("Failed")]
	Failed = 3,
}
