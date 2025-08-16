using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Models;
public record TfUserPresetSortPersonalization
{
	public Guid SpaceViewId { get; init; } = default;
	public Guid SpaceViewPresetId { get; init; } = default;
	public List<TfSort> Sorts { get; init; } = new();
}