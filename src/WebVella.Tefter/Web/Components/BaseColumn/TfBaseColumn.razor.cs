﻿namespace WebVella.Tefter.Web.Components;
public partial class TfBaseColumn : TfBaseComponent
{
	[Parameter]
	public SpaceViewColumn Meta { get; set; }

	[Parameter]
	public DemoDataRow Data { get; set; }

	[Parameter]
	public Action<(string,object)> ValueChanged { get; set; }

	
}