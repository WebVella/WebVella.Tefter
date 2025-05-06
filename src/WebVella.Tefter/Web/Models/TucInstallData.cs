using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucInstallData
{
	
public TucInstallData() { }
	public TucInstallData(TfInstallData model)
	{
	}
	public TfInstallData ToModel()
	{
		return new TfInstallData
		{
		};
	}
}
