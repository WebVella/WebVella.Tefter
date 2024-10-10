namespace WebVella.Tefter.Talk.Models;

public class TalkThread
{
	public Guid Id { get; set; }
	public Guid ChannelId { get; set; }
	public Guid? ThreadId { get; set; }
	public TalkThreadType Type { get; set; }
	public string Content { get; set; }
	public bool VisibleInChannel { get; set; }	
	public  User User { get; set; }
	public DateTime CreatedOn { get; set; }
	public DateTime? LastUpdatedOn { get; set; } = null;
	public DateTime? DeletedOn { get; set; } = null;
	public Dictionary<Guid, string> RelatedSK { get; set; }
	public List<TalkThread> SubThread { get; set; }
}


public class CreateTalkThread
{
	public Guid ChannelId { get; set; }
	public TalkThreadType Type { get; set; }
	public string Content { get; set; }
	public Guid UserId { get; set; }
	public List<Guid> RowIds { get; set; }
	public Guid DataProviderId { get; set; }
}

public class CreateTalkSubThread
{
	public Guid ThreadId { get; set; }
	public bool VisibleInChannel { get; set; }
	public string Content { get; set; }
	public Guid UserId { get; set; }
}
