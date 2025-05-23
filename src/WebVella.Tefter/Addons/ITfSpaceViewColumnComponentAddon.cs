﻿namespace WebVella.Tefter.Addons;

public interface ITfSpaceViewColumnComponentAddon : ITfAddon
{
	public List<Guid> SupportedColumnTypes { get; init; }
	public TfSpaceViewColumnScreenRegionContext RegionContext { get; set; }
	public EventCallback<string> OptionsChanged { get; set; }
	public EventCallback<TfDataTable> RowChanged { get; set; }
	void ProcessExcelCell(IServiceProvider serviceProvider, IXLCell excelCell);
}


public class TfSpaceViewColumnComponentAddonMeta
{
	public ITfSpaceViewColumnComponentAddon Instance { get; init; }
}
