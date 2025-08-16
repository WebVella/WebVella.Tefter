using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Models;
public record TfUserViewColumnPersonalization
{
	public Guid SpaceViewId { get; init; } = default;
	public Guid SpaceViewColumnId { get; init; } = default;
	public int Width { get; init; } = default!;
}