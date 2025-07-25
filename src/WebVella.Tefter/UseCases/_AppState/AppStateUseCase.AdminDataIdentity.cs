﻿using Microsoft.AspNetCore.Mvc;

namespace WebVella.Tefter.UseCases.AppState;
internal partial class AppStateUseCase
{
	internal async Task<(TfAppState, TfAuxDataState)> InitAdminDataIdentityAsync(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		if (
			!(newAppState.Route.HasNode(RouteDataNode.Admin, 0)
			&& newAppState.Route.HasNode(RouteDataNode.DataIdentities, 1))
			)
		{
			newAppState = newAppState with
			{
				AdminDataIdentities = new(),
				AdminDataIdentity = null,
			};
			return (newAppState, newAuxDataState);
		}

		//AdminDataIdentities
		if (
			newAppState.AdminDataIdentities.Count == 0
			|| (newAppState.Route.DataIdentityId is not null
				&& !newAppState.AdminDataIdentities.Any(x => x.Name == newAppState.Route.DataIdentityId))
			)
			newAppState = newAppState with
			{
				AdminDataIdentities = await GetDataIdentitiesAsync()
			};

		//DataIdentityId
		if (!String.IsNullOrWhiteSpace(newAppState.Route.DataIdentityId))
		{
			var adminIdentity = await GetDataIdentityAsync(newAppState.Route.DataIdentityId);
			newAppState = newAppState with { AdminDataIdentity = adminIdentity };
		}

		return (newAppState, newAuxDataState);
	}

	internal virtual Task<List<TucDataIdentity>> GetDataIdentitiesAsync(
		string search = null,
		int? page = null,
		int? pageSize = null)
	{
		try
		{
			var identities = _tfService.GetDataIdentities();
			if (identities is null)
				return Task.FromResult(new List<TucDataIdentity>());

			var orderedResults = identities.OrderBy(x => x.DataIdentity);

			var records = new List<TfDataIdentity>();
			if (!String.IsNullOrWhiteSpace(search))
			{
				var searchProcessed = search.Trim().ToLowerInvariant();
				foreach (var item in orderedResults)
				{
					bool hasMatch = false;
					if (item.DataIdentity.ToLowerInvariant().Contains(searchProcessed)) hasMatch = true;
					if (hasMatch) records.Add(item);
				}
			}
			else
			{
				records = orderedResults.ToList();
			}

			if (page is null || pageSize is null)
			{
				return Task.FromResult(records
					.Select(x => new TucDataIdentity(x))
					.ToList());
			}

			return Task.FromResult(records
					.Skip(TfConverters.CalcSkip(page.Value, pageSize.Value))
					.Take(pageSize.Value)
					.Select(x => new TucDataIdentity(x))
					.ToList());
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
					exception: ex,
					toastErrorMessage: "Unexpected Error",
					toastValidationMessage: "Invalid Data",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
			return Task.FromResult(new List<TucDataIdentity>());
		}
	}

	internal virtual Task<TucDataIdentity> GetDataIdentityAsync(
			string identityName)
	{
		try
		{
			var provider = _tfService.GetDataIdentity(identityName);
			if (provider is null)
				return Task.FromResult((TucDataIdentity)null);

			return Task.FromResult(new TucDataIdentity(provider));
		}
		catch (Exception ex)
		{
			ResultUtils.ProcessServiceException(
				exception: ex,
				toastErrorMessage: "Unexpected Error",
				toastValidationMessage: "Invalid Data",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult((TucDataIdentity)null);

		}
	}

	internal virtual Task DeleteDataIdentityAsync(
		string identityName)
	{
		_tfService.DeleteDataIdentity(identityName);
		return Task.CompletedTask;
	}

	internal virtual TucDataIdentity CreateDataIdentity(
		TucDataIdentity form)
	{
		var submitForm = form.ToModel();

		var provider = _tfService.CreateDataIdentity(submitForm);
		if (provider is null)
			throw new TfException("CreateDataIdentity returned null object");

		return new TucDataIdentity(provider);
	}

	internal virtual TucDataIdentity UpdateDataIdentity(
		TucDataIdentity form)
	{
		var submitForm = form.ToModel();

		var provider = _tfService.UpdateDataIdentity(submitForm);
		if (provider is null)
			throw new TfException("UpdateDataIdentity returned null object");

		return new TucDataIdentity(provider);
	}

	internal virtual async Task<List<TucDataProvider>> GetDataProvidersImplementingIdentity(string identityName){ 
		var result = new List<TucDataProvider>();
		var allProviders = await GetDataProvidersAsync();

		foreach (var item in allProviders)
		{
			if(item.Identities is null || item.Identities.Count == 0) continue;
			if(!item.Identities.Any(x=> x.Name == identityName)) continue;
			result.Add(item);
		}

		return result.OrderBy(x=> x.Name).ToList();
	}

}
