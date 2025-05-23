﻿using DocumentFormat.OpenXml.Office.CustomUI;
using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucUser
{
	public Guid Id { get; init; }
	public string Email { get; init; }
	public string FirstName { get; init; }
	public string LastName { get; init; }
	public string Password { get; init; }
	public string Names
	{
		get
		{
			var sb = new List<string>();
			if (!String.IsNullOrWhiteSpace(FirstName)) sb.Add(FirstName);
			if (!String.IsNullOrWhiteSpace(LastName)) sb.Add(LastName);

			return String.Join(" ", sb);
		}
	}
	[Required]

	public bool Enabled { get; init; }
	public DateTime CreatedOn { get; init; }
	public TucUserSettings Settings { get; init; } = new();
	public ReadOnlyCollection<TucRole> Roles { get; init; }

	public bool IsAdmin
	{
		get
		{
			if (Roles is not null && Roles.Any(x => x.Id == TfConstants.ADMIN_ROLE_ID))
				return true;
			return false;
		}
	}
	public TucUser() { }

	public TucUser(TfUser model)
	{
		Id = model.Id;
		Email = model.Email;
		FirstName = model.FirstName;
		LastName = model.LastName;
		Password = model.Password;
		Enabled = model.Enabled;
		CreatedOn = model.CreatedOn;
		Settings = new TucUserSettings(model.Settings);
		Roles = model.Roles is null ? null : model.Roles.Select(x => new TucRole(x)).ToList().AsReadOnly();
	}

	public TfUser ToModel()
	{
		return new TfUser
		{
			Id = Id,
			Email = Email,
			FirstName = FirstName,
			LastName = LastName,
			Password = Password,
			Enabled = Enabled,
			CreatedOn = CreatedOn,
			Settings = Settings.ToModel(),
			Roles = Roles is null ? null : Roles.Select(x => x.ToModel()).ToList().AsReadOnly()
		};
	}
}
