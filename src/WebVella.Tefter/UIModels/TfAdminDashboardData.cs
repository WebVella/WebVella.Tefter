using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Models;
public class TfAdminDashboardData
{
	public ReadOnlyCollection<TfDataProviderInfo> ProvidersInfo { get; set; } = null!;
	public List<TfDataProviderInfo> SyncInfo { get; set; } = new();
}
