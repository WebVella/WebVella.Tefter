﻿namespace WebVella.Tefter.Web.Store;

public partial record TfAppState
{
	public List<ITfTemplateProcessor> AdminTemplateProcessors { get; init; } = new();
	public List<TucTemplate> AdminTemplateList { get; init; } = new();
	public TucTemplate AdminTemplateDetails { get; init; } = null;
}
