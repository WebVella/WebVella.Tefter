﻿namespace WebVella.Tefter.TemplateProcessors.TextFile.Models;

public class TextFileTemplateResult : ITfTemplateResult
{
	public List<TextFileTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}