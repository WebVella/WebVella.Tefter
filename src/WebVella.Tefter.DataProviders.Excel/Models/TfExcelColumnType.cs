using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.DataProviders.Excel.Models;

internal enum TfExcelColumnType
{
    [Description("TEXT")]
    Text = 0,
    [Description("NUMBER")]
    Number = 1,
    [Description("DATETIME")]
    DateTime = 2,
    [Description("BOOLEAN")]
    Boolean = 3,
    [Description("GUID")]
    Guid = 4,
    [Description("CURRENCY")]
    Currency = 5,
    [Description("PERCENTAGE")]
    Percentage = 6
}