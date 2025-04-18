﻿namespace WebVella.Tefter.Database;

public record TfDatabaseUpdateLogRecord
{
	public DateTime CreatedOn { get; internal set; }
	public string Statement { get; internal set; }
	public bool Success { get; internal set; }
	public string SqlError { get; internal set; }
}
