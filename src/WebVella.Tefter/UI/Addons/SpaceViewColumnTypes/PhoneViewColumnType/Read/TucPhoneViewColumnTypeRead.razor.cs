﻿namespace WebVella.Tefter.UI.Addons;

public partial class TucPhoneViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadModeContext Context { get; set; } = null!;
	[Parameter] public List<string?>? Value { get; set; }
}
