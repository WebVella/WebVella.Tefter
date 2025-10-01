﻿namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterRegionComponents method as a case in order to be discovered
/// </summary>
public class TfTemplateProcessorDisplaySettingsScreenRegionContext : TfBaseScreenRegionContext
{
	public TfTemplate Template { get; set; } = null!;
}
