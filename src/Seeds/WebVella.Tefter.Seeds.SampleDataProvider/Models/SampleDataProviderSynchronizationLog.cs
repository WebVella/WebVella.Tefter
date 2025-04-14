using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.Seeds.SampleDataProvider.Models;
internal class SampleDataProviderSynchronizationLog : ITfDataProviderSychronizationLog
{
	public void AddNewLogEntry(string message)
	{
		throw new NotImplementedException();
	}

	public void Log(string message, TfDataProviderSychronizationLogEntryType type = TfDataProviderSychronizationLogEntryType.Info)
	{
		throw new NotImplementedException();
	}

	public void Log(string message, Exception ex)
	{
		throw new NotImplementedException();
	}

	public ReadOnlyCollection<TfDataProviderSychronizationLogEntry> GetEntries()	
	{
		throw new NotImplementedException();
	}
}
