﻿namespace WebVella.Tefter.Models;
public interface ITfTemplateResult
{
	public List<ValidationError> Errors { get; set; }
}