using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Models;
public class TfHomeDashboardData
{
	public List<TfTag> HomeTags { get; set; } = new();
	public List<TfBookmark> HomeBookmarks { get; set; } = new();
	public List<TfBookmark> HomeSaves { get; set; } = new();
	public List<TfSpaceView> HomeViews { get; set; } = new();
	public List<TfSearchResult> HomeSearchResults { get; set; } = new();
}
