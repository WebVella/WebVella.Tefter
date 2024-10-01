namespace WebVella.Tefter.Web.Models;

public record TucDataProviderSyncTaskInfo
{
	public Guid Id { get; set; }

	public Guid TaskId { get; set; }

	public DateTime CreatedOn { get; set; } = DateTime.Now;

	public int? TfRowIndex { get; set; } = null;

	public Guid? TfId { get; set; } = null;

	public string Info { get; set; } = null;

	public string Warning { get; set; } = null;

	public string Error { get; set; } = null;

	public TucDataProviderSyncTaskInfo() { }
	public TucDataProviderSyncTaskInfo(TfDataProviderSynchronizeResultInfo model)
	{
		Id = model.Id;
		TaskId = model.TaskId;
		CreatedOn = model.CreatedOn;
		TfRowIndex = model.TfRowIndex;
		TfId = model.TfId;
		Info = model.Info;
		Warning = model.Warning;
		Error = model.Error;

	}
}
