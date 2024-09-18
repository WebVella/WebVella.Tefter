﻿namespace WebVella.Tefter.Web.Store;

[FeatureState]
public partial record TfAppState
{
	public Guid Hash { get; init; } = Guid.NewGuid();
	public bool IsBusy { get; init; } = new();
}