﻿namespace WebVella.Tefter.Web.Pages;
public partial class SpaceManagePage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public string Menu { get; set; }
}