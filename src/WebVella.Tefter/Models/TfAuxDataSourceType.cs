﻿using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public enum TfAuxDataSourceType{ 

	PrimatyDataProvider = 0,
	AuxDataProvider = 1,
	SharedColumn = 2,
	NotFound = 3
}