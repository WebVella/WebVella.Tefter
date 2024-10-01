using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucInitState
{
	public TucUser User { get; set; }
	public TucCultureOption CultureOption { get; set; }
	public List<TucSpace> UserSpaces { get; set; }
}
