using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Models;
public class TfUserSettings
{
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; init; } = OfficeColor.Windows;
	public bool IsSidebarOpen { get; init; } = true;
	public string CultureName { get; init; } = string.Empty;
	public string StartUpUrl { get; init; } = null;
	public int? PageSize { get; init; } = null;
}