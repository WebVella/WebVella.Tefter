﻿namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminUserDetails.TfAdminUserDetails","WebVella.Tefter")]
public partial class TfAdminUserDetails : TfBaseComponent
{
	[Inject] private UserAdminUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }


	
}