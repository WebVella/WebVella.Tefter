﻿namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Models;

public class ExcelFileTemplateResult : ITfTemplateResult
{
	public int ItemWithErrorsCount { get => Items.Where(x=> x.Errors.Count > 0).Count(); }
	public List<ExcelFileTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();

	public string ZipDownloadUrl { get; set; } = null;
}