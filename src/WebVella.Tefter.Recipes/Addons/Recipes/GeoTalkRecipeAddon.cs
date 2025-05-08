using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebVella.Tefter.Addons;
using WebVella.Tefter.DataProviders.Csv;
using WebVella.Tefter.DataProviders.Csv.Addons;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.Recipes.Addons.Recipes;
public class GeoTalkRecipeAddon : ITfRecipeAddon
{
	public Guid Id { get; init; } = new Guid("b8f2c8ae-cc42-4c90-9f22-289498b4dffb");
	public string Name { get; init; } = "Geo Talk";
	public string Description { get; init; } = "team collaboration around world cities";
	public string FluentIconName { get; init; } = "Globe";
	public int SortIndex { get; init; } = 100;
	public string Author { get; init; } = "WebVella";
	public string Website { get; init; } = "https://tefter.webvella.com";
	public List<TfRecipeStepBase> Steps { get; init; } = new();

	public GeoTalkRecipeAddon()
	{
		var regularRoleId = new Guid("26dfd164-f1d7-4078-85e7-7f3f852f6577");
		var dataProviderId = new Guid("9ba6d290-8de0-475e-92f7-bda8d03667c7");
		var csvFileName = "worldcities.csv";

		var step11 = new TfInfoRecipeStep
		{
			Visible = true,
			StepId = new Guid("0ea1122d-1377-4160-b6b6-2874ee9bbf9a"),
			StepMenuTitle = "Get started",
			StepContentTitle = "Geo Talk Sample application",
			HtmlContent =
			@"<p>This is a sample application that shows some of the tools of Tefter.bg in a practical environment. The use case is the need for a team to collaborate around the data of the world cities, with tools as discussions and export to templates</p>
			"
		};

		var step12 = new TfInfoRecipeStep
		{
			Visible = true,
			StepId = new Guid("f6197980-7a29-4a24-9c30-3886b59b456c"),
			StepContentTitle = "User accounts",
			HtmlContent =
			@"<p>The recipe will create 3 user accounts. One administrator with access to all pages and two regular users with access only to the 'World Cities' space.</p>
			<p>NOTE: Write down these details as you will need them.</p>
			<table>
				<thead>
					<tr>
						<th>email</th>
						<th>password</th>
						<th>role</th>
					</tr>
				</thead>
				<tbody>
					<tr>
						<td>admin@test.com</td>
						<td>1</td>
						<td>administrator</td>
					</tr>
					<tr>
						<td>user1@test.com</td>
						<td>1</td>
						<td>first regular user</td>
					</tr>
					<tr>
						<td>user2@test.com</td>
						<td>1</td>
						<td>second regular user</td>
					</tr>
				</tbody>
			</table>
			"
		};

		var step13 = new TfInfoRecipeStep
		{
			Visible = true,
			StepId = new Guid("7a93a020-fd7b-4f2b-8178-a0a9c1080522"),
			StepContentTitle = "Data source",
			HtmlContent =
			@"<p>City data will be imported from the uploaded <strong>world-cities.csv</strong> file. This file's data will be accessible through a CSV Data provider named <strong>WorldCities</strong>.</p>
			"
		};

		var step14 = new TfInfoRecipeStep
		{
			Visible = true,
			StepId = new Guid("301e0e07-21fe-484e-9c59-8599c4f53b32"),
			StepContentTitle = "Space",
			HtmlContent =
			@"<p>Regular users can interact with the data through the space <strong>World Cities</strong>. To achieve this, we will create the required a dataset, view and page.</p>
			"
		};

		var step15 = new TfInfoRecipeStep
		{
			Visible = true,
			StepId = new Guid("c65d1532-93f0-4029-8b31-c7bce26cf1e7"),
			StepContentTitle = "Data discussions",
			HtmlContent =
			@"<p>Discussions related to the data will be managed by the <strong>Talk addon applications</strong>, for which we will configure specific discussion channels.</p>
			"
		};

		var step16 = new TfInfoRecipeStep
		{
			Visible = true,
			StepId = new Guid("8a73d43f-a580-40fe-a441-d007141cda6b"),
			StepContentTitle = "Data export",
			HtmlContent =
			@"<p>Data and data selection will be exportable in a generic spreadsheet file as well as in several templates we will create for the purpose.</p>
			"
		};

		Steps.Add(new TfGroupRecipeStep
		{
			Visible = true,
			StepId = new Guid("5d8318bb-8427-425d-b4ee-9d4fe29977a5"),
			StepMenuTitle = "Get started",
			StepContentTitle = "Geo Talk Sample application",
			Steps = new List<TfRecipeStepBase>() {
				step11,step12,step13,step14,step15, step16
			}
		});

		Steps.Add(new TfGroupRecipeStep
		{
			Visible = false,
			StepId = new Guid("a0c78375-3a1d-4707-b04a-94ccf3a4ab14"),
			StepMenuTitle = "Regular Role",
			Steps = new List<TfRecipeStepBase>()
			{
				new TfCreateUserRecipeStep
				{
					Visible = true,
					StepId = new Guid("241ae707-0b8f-4b1c-88d3-5f8d438ab3f4"),
					StepMenuTitle = "System Administrator",
					StepContentTitle = "Create administrative account",
					StepContentDescription = "This user will be designated as the system superuser, automatically assigned the administrator role, and granted access to all system areas.",
					Email = "admin@test.com",
					Password = "1",
					FirstName = "System",
					LastName = "Administrator",
					Roles = new List<Guid> { TfConstants.ADMIN_ROLE_ID }
				},
				new TfCreateRoleRecipeStep
				{
					Visible = false,
					StepId = new Guid("89759f9b-062a-4a96-9b45-9b7007078e18"),
					StepMenuTitle = "Regular Role",
					StepContentTitle = "Regular Role",
					StepContentDescription = "created the 'Regular' role",
					RoleId = regularRoleId,
					Name = "regular"
				},
				new TfCreateUserRecipeStep
				{
					Visible = false,
					StepId = new Guid("7ab6070d-46a7-4bb0-8538-3637898bb69b"),
					StepMenuTitle = "Regular User 1",
					StepContentTitle = "Regular User 1",
					StepContentDescription = "Has access only to the created user space.",
					Email = "user1@test.com",
					Password = "1",
					FirstName = "User",
					LastName = "1",
					Roles = new List<Guid> { regularRoleId }
				},
				new TfCreateUserRecipeStep
				{
					Visible = false,
					StepId = new Guid("e5b36d26-c9ab-4efe-9d2d-6399d501bc89"),
					StepMenuTitle = "Regular User 2",
					StepContentTitle = "Regular User 2",
					StepContentDescription = "Has access only to the created user space.",
					Email = "user2@test.com",
					Password = "1",
					FirstName = "User",
					LastName = "2",
					Roles = new List<Guid> { regularRoleId }
				},
			}
		});

		Steps.Add(new TfCreateRepositoryFileRecipeStep
		{
			Visible = false,
			StepId = new Guid("ad312b7d-d2a4-446b-8913-2fa8a8a8f44a"),
			StepMenuTitle = "Upload file",
			FileName = csvFileName,
			EmbeddedResourceName = $"WebVella.Tefter.Recipes.Files.{csvFileName}",
			Assembly = this.GetType().Assembly,
		});

		Steps.Add(new TfCreateDataProviderRecipeStep
		{
			Visible = false,
			StepId = new Guid("c86fcc0a-980f-4c2f-b527-e74d467a6f1e"),
			StepMenuTitle = "Data Provider",
			StepContentTitle = "Creating the Data provider",
			DataProviderId = dataProviderId,
			Name = "World Cities",
			Type = new CsvDataProvider(),
			SettingsJson = JsonSerializer.Serialize(new CsvDataProviderSettings
			{
				Filepath = $"tefter://fs/repository/{csvFileName}"
			}),
			Columns = new List<TfDataProviderColumn>()
			{
				new TfDataProviderColumn{
				Id = new Guid("b706b5b1-c296-4adb-a799-0f40993f6cfb"),
				CreatedOn = DateTime.Now,
				DataProviderId = dataProviderId,
				SourceName = "city",
				SourceType = "TEXT",
				DbName = "city",
				DbType = Database.TfDatabaseColumnType.Text,
				DefaultValue = "city",
				IsNullable = false,
				IsSearchable = true,
				IncludeInTableSearch = true,
				PreferredSearchType = TfDataProviderColumnSearchType.Contains,
				IsSortable = true,
				IsUnique = false
				},
				new TfDataProviderColumn{
				Id = new Guid("0dbc9155-3f1c-4c69-8af8-4b0d91f1301d"),
				CreatedOn = DateTime.Now,
				DataProviderId = dataProviderId,
				SourceName = "country",
				SourceType = "TEXT",
				DbName = "country",
				DbType = Database.TfDatabaseColumnType.Text,
				DefaultValue = "country",
				IsNullable = false,
				IsSearchable = true,
				IncludeInTableSearch = true,
				PreferredSearchType = TfDataProviderColumnSearchType.Contains,
				IsSortable = true,
				IsUnique = false
				},
				new TfDataProviderColumn{
				Id = new Guid("af549f23-0a3a-4cfc-9033-7a4cbdbdce16"),
				CreatedOn = DateTime.Now,
				DataProviderId = dataProviderId,
				SourceName = "population",
				SourceType = "LONG_INTEGER",
				DbName = "population",
				DbType = Database.TfDatabaseColumnType.LongInteger,
				DefaultValue = "0",
				IsNullable = false,
				IsSearchable = false,
				IncludeInTableSearch = false,
				PreferredSearchType = TfDataProviderColumnSearchType.Equals,
				IsSortable = true,
				IsUnique = false
				},
				new TfDataProviderColumn{
				Id = new Guid("22e9ed31-fe26-49d6-84e0-2c7f8fd13344"),
				CreatedOn = DateTime.Now,
				DataProviderId = dataProviderId,
				SourceName = "id",
				SourceType = "LONG_INTEGER",
				DbName = "id",
				DbType = Database.TfDatabaseColumnType.LongInteger,
				DefaultValue = "0",
				IsNullable = false,
				IsSearchable = false,
				IncludeInTableSearch = false,
				PreferredSearchType = TfDataProviderColumnSearchType.Equals,
				IsSortable = false,
				IsUnique = true
				}
			},
			ShouldSynchronizeData = true
		});
	}

}
