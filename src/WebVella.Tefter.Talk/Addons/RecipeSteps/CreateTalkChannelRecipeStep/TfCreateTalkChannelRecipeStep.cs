
namespace WebVella.Tefter.Talk.Addons;
public class TfCreateTalkChannelRecipeStep : ITfRecipeStepAddon
{
	public Guid AddonId { get; init; } = new Guid("3291ab39-0d89-4e5c-8f20-65e5c8588dd5");
	public string AddonName { get; init; } = "CreateTalkChannelRecipeStep";
	public string AddonDescription { get; init; } = "creates talk channel recipe step";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public Type FormComponent { get; set; } = typeof(TfCreateTalkChannelRecipeStepForm);	
	public TfRecipeStepInstance Instance { get; set; }
	public ITfRecipeStepAddonData Data { get; set; }
	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
	{
		ITalkService talkService = serviceProvider.GetService<ITalkService>();

		if (addon.GetType().FullName != this.GetType().FullName)
			throw new Exception("Wrong addon type provided for application");

		if (addon.Data.GetType().FullName != typeof(TfCreateTalkChannelRecipeStepData).FullName)
			throw new Exception("Wrong data model type provided for application");

		var step = (TfCreateTalkChannelRecipeStepData)addon.Data;

		var allSharedColumns = talkService.GetAllSharedColumns();
		var columnName = step.CountSharedColumnName?.Trim();
		var column = allSharedColumns.FirstOrDefault(x=> x.DbName == columnName);
		if(column is null)
			allSharedColumns.FirstOrDefault(x=> x.DbName.ToLowerInvariant() == columnName.ToLowerInvariant());
		if(column is null) throw new Exception($"Shared column not found with the name: '{step.CountSharedColumnName}'");

		var channel = new TalkChannel{ 
			Id = step.ChannelId != Guid.Empty ? step.ChannelId : Guid.NewGuid(),
			DataIdentity = column.DataIdentity,
			CountSharedColumnName = column.DbName,
			Name = step.Name,
		};
		talkService.CreateChannel(channel);
		return Task.CompletedTask;
	}
}

public class TfCreateTalkChannelRecipeStepData : ITfRecipeStepAddonData
{ 
	public Guid ChannelId { get; set; }
	public string JoinKey { get; set; }
	public string CountSharedColumnName { get; set; }
	public string Name { get; set; }
}