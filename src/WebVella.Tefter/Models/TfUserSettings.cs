using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Models;
public class TfUserSettings
{
	public TfColor Color { get; init; } = TfConstants.DefaultThemeColor;
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	public bool IsSidebarOpen { get; init; } = true;
	public string CultureName { get; init; } = "en-US";
	[JsonIgnore]
	public CultureInfo Culture { get =>  new CultureInfo(String.IsNullOrWhiteSpace(CultureName) ? "en-US" : CultureName); }
	public string? StartUpUrl { get; init; } = null;
	public int? PageSize { get; init; } = null;

	public List<TfViewPresetColumnPersonalization> ViewPresetColumnPersonalizations { get; init; } = new();
	public List<TfViewPresetSortPersonalization> ViewPresetSortPersonalizations { get; init; } = new();
}