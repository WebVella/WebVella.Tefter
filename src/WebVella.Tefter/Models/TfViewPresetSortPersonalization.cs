using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Models;
public record TfViewPresetSortPersonalization
{
	public Guid SpaceViewId { get; init; } = default;
	public Guid? PresetId { get; init; } = null;
	public List<TfSort> Sorts { get; init; } = new();
}