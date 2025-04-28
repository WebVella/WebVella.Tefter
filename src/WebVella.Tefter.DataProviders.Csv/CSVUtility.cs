using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebVella.Tefter.DataProviders.Csv;
internal static class CSVUtility
{
	internal static string ToSourceColumnName(this string sourceColumnName)
	{
		string pattern = "[^ -~]+";
		Regex reg_exp = new Regex(pattern);
		return reg_exp.Replace(sourceColumnName.Trim(), "");
	}
}
