using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.UseCases.Models;

public record TucInitState
{
	public TucUser User { get; set; }
	public List<TucSpace> UserSpaces { get; set; } = new();
	public TucCultureOption CultureOption { get; set; }
}
