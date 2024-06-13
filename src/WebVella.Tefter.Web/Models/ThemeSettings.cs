using System.Text.Json.Serialization;

namespace WebVella.Tefter.Web.Models;

public record ThemeSettings
{
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; init; } = OfficeColor.Excel;
}
