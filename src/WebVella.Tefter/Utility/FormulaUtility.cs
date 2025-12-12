namespace WebVella.Tefter.Utility;

public static class FormulaUtility
{
	public static DateTime? GetDateFromFormulaString(this string formula, DateTime? defaultDate = null)
	{
		if (String.IsNullOrWhiteSpace(formula)) return defaultDate;
		var result = defaultDate;
		#region<<Process NOW >>
		{
			var pattern = @"now\(.*?\)";
			var match = formula.GetMatchFromString(pattern);
			if (!String.IsNullOrWhiteSpace(match))
			{
				var offsetString = match.ToLowerInvariant().Replace("now(", "").Replace(")", "").Replace(",", ".").Trim();

				if (String.IsNullOrWhiteSpace(offsetString))
				{
					return DateTime.Now;
				}

				if (double.TryParse(offsetString, new CultureInfo("en-US"), out double outDbl))
				{
					return DateTime.Now.AddHours(outDbl);
				}
			}
		}
		#endregion

		#region<<Process DAY >>
		{
			var pattern = @"day\(.*?\)";
			var match = formula.GetMatchFromString(pattern);
			if (!String.IsNullOrWhiteSpace(match))
			{
				var offsetString = match.ToLowerInvariant().Replace("day(", "").Replace(")", "").Replace(",", ".").Trim();
				var today = DateTime.Today;
				if (String.IsNullOrWhiteSpace(offsetString))
				{
					return today;
				}

				if (double.TryParse(offsetString, new CultureInfo("en-US"), out double outDbl))
				{
					return today.AddDays(outDbl);
				}
			}
		}
		#endregion

		#region<<Process MONTH >>
		{
			var pattern = @"month\(.*?\)";
			var match = formula.GetMatchFromString(pattern);
			if (!String.IsNullOrWhiteSpace(match))
			{
				var offsetString = match.ToLowerInvariant().Replace("month(", "").Replace(")", "").Replace(",", ".").Trim();
				var today = DateTime.Today;
				if (String.IsNullOrWhiteSpace(offsetString))
				{
					return new DateTime(today.Year, today.Month, 1);
				}

				if (double.TryParse(offsetString, new CultureInfo("en-US"), out double outDbl))
				{
					var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
					var wholeMonths = (int)Math.Abs(outDbl);
					var monthFraction = Math.Abs(outDbl) - wholeMonths;
					var days = (int)(daysInMonth * monthFraction);
					var multiplier = outDbl >= 0 ? 1 : -1;
					return today.AddMonths(multiplier * wholeMonths).AddDays(multiplier * days);
				}
			}
		}
		#endregion

		#region<<Process YEAR >>
		{
			var pattern = @"year\(.*?\)";
			var match = formula.GetMatchFromString(pattern);
			if (!String.IsNullOrWhiteSpace(match))
			{
				var offsetString = match.ToLowerInvariant().Replace("year(", "").Replace(")", "").Replace(",", ".").Trim();
				var today = DateTime.Today;
				if (String.IsNullOrWhiteSpace(offsetString))
				{
					return new DateTime(today.Year, 1, 1);
				}

				if (double.TryParse(offsetString, new CultureInfo("en-US"), out double outDbl))
				{

					var wholeYears = (int)Math.Abs(outDbl);
					var yearFraction = Math.Abs(outDbl) - wholeYears;
					var days = (int)(365 * yearFraction);
					var multiplier = outDbl >= 0 ? 1 : -1;
					return today.AddYears(multiplier * wholeYears).AddDays(multiplier * days);
				}
			}
		}
		#endregion

		#region << Just date >>
		{
			if (DateTime.TryParse(formula, Thread.CurrentThread.CurrentCulture, out DateTime outDateTime))
			{
				return outDateTime;
			}
		}
		#endregion

		return result;
	}

	private static string GetMatchFromString(this string formula, string pattern)
	{
		if (String.IsNullOrWhiteSpace(formula)) return null;
		Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
		MatchCollection matches = r.Matches(formula);
		if (matches.Count > 0)
		{
			foreach (Match match in matches)
			{
				if (match.Success) return match.Value;
			}
		}
		return null;
	}

}
