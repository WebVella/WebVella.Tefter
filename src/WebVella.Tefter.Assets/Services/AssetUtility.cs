using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Assets.Services;

internal static class AssetUtility
{
    internal static Dictionary<Guid, Dictionary<string, object>>? ApplyCountChange(this TfDataTable? data, Dictionary<Guid, Dictionary<string, long>>? countChange)
    {
        if (data is null || countChange is null || countChange.Keys.Count == 0) return null;
        var dataChange = new Dictionary<Guid, Dictionary<string, object>>();
        foreach (var rowId in countChange.Keys)
        {
            var row = data.Rows[rowId];
            if (row is null) continue;
            foreach (var columnName in countChange[rowId].Keys)
            {
                var column = data.Columns[columnName];
                if (column is null) continue;
                if (column.DbType != TfDatabaseColumnType.ShortInteger
                && column.DbType != TfDatabaseColumnType.Integer
                && column.DbType != TfDatabaseColumnType.LongInteger
                && column.DbType != TfDatabaseColumnType.Number) continue;

                if (!dataChange.ContainsKey(rowId))
                    dataChange[rowId] = new();
                dataChange[rowId][columnName] = (row[columnName].ToLong() ?? 0) + countChange[rowId][columnName];
            }
        }

        return dataChange;

    }
}
