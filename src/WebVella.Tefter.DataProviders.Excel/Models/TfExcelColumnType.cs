using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.DataProviders.Excel.Models;

internal enum TfExcelColumnType
{
    [Description("UNKNOWN")]
    Unknown = 0,
    [Description("TEXT")]
    Text = 1,
    [Description("NUMBER")]
    Number = 2,
    [Description("DATETIME")]
    DateTime = 3,
    [Description("BOOLEAN")]
    Boolean = 4,
    [Description("GUID")]
    Guid = 5,
    [Description("CURRENCY")]
    Currency = 6,
    [Description("PERCENTAGE")]
    Percentage = 7
}