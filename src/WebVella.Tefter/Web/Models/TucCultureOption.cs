﻿namespace WebVella.Tefter.Web.Models;

public record TucCultureOption
{
	public string CultureCode { get; set; }
	[JsonIgnore]
	public CultureInfo CultureInfo { get => CultureInfo.GetCultureInfo(CultureCode); }
	public string IconUrl { get; set; }
	public string Name { get; set; }
}
