﻿using System.Text;

namespace WebVella.Tefter.Web.Models;

public record RouteData
{
	public Guid? SpaceId { get; set; }
	public Guid? SpaceDataId { get; set; }
	public Guid? SpaceViewId { get; set; }
	public Guid? UserId { get; set; }
	public Guid? DataProviderId { get; set; }

}
