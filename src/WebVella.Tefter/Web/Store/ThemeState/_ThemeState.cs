﻿namespace WebVella.Tefter.Web.Store.ThemeState;
using System.Drawing;
using SystemColor = System.Drawing.Color;

[FeatureState]
public record ThemeState
{
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; init; } = OfficeColor.Excel;

	public string ThemeColorString
	{
		get
		{
			return ThemeColor.ToAttributeValue();
		}
	}

	public SystemColor ThemeColorObject
	{
		get
		{
			return (SystemColor)System.Drawing.ColorTranslator.FromHtml(ThemeColorString);
		}
	}

	public string ThemeBackgkroundColor => $"{ThemeColorString}25";

	public string ThemeBorderColor => $"{ThemeColorString}75";

	public string ThemeBackgroundAccentColor => $"{ThemeColorString}35";

	public string ThemeSidebarStyle => $"background-color:{ThemeBackgkroundColor} !important; border-color:{ThemeBorderColor} !important";

}
