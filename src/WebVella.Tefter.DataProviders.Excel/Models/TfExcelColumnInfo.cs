using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.DataProviders.Excel.Models;

internal class TfExcelColumnInfo
{
    public string? Header { get; set; }      // column title (A1, B1 …)
    public TfExcelColumnType Type { get; set; }
}
