using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.Tefter.DataProviders.Excel.Models;

namespace WebVella.Tefter.DataProviders.Excel;

internal static class ExcelDataProviderUtility
{
    /// <summary>
    /// Heuristic: look at the first 10 non‑empty cells and decide the type.
    /// </summary>
    internal static TfExcelColumnType InferColumnType(this IEnumerable<IXLCell> cells)
    {
        var sample = cells.Take(10).Select(c => c.GetValue<string>()).Where(v => !string.IsNullOrWhiteSpace(v)).ToArray();

        if (!sample.Any())
            return TfExcelColumnType.Unknown;

        // 1️⃣ Try GUID
        if (sample.All(v => Guid.TryParse(v, out _)))
            return TfExcelColumnType.Guid;

        // 2️⃣ Try numeric
        if (sample.All(v => double.TryParse(v, NumberStyles.Float | NumberStyles.AllowThousands,
                                           CultureInfo.InvariantCulture, out _)))
            return TfExcelColumnType.Number;

        // 3️⃣ Try DateTime
        if (sample.All(v => DateTime.TryParse(v, CultureInfo.InvariantCulture, DateTimeStyles.None, out _)))
            return TfExcelColumnType.DateTime;

        // 4️⃣ Try Boolean
        var boolValues = new[] { "true", "false", "1", "0" };
        if (sample.All(v => boolValues.Contains(v.Trim().ToLowerInvariant())))
            return TfExcelColumnType.Boolean;

        // 5️⃣ Currency / Percentage (look for % or currency symbols)
        if (sample.Any(v => v.Contains('%')))
            return TfExcelColumnType.Percentage;
        if (sample.Any(v => v.StartsWith("$") || v.StartsWith("€") || v.StartsWith("£")))
            return TfExcelColumnType.Currency;

        // 6️⃣ Default to text
        return TfExcelColumnType.Text;
    }
}
