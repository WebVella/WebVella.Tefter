﻿namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{

	internal Task InitForData()
	{
		IsBusy = true;
		return Task.CompletedTask;
	}



}
