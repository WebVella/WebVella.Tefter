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
	public bool ShowWelcome
	{
		get
		{
			if (ProvidersCount == 0)
				return true;
			else if (SpacesCount == 0)
				return true;
			else if (SpacePagesCount == 0)
				return true;

			return false;

		}
	}
	public int ProvidersCount { get; set; } = 0;
	public int SpacesCount { get; set; } = 0;
	public int SpacePagesCount { get; set; } = 0;
}
