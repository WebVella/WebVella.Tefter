namespace WebVella.Tefter.Talk.Models;

public class TalkThread
{
	public Guid Id { get; set; }
	public string IdentityRowId { get { return Id.ToSha1(); }  }
	public Guid ChannelId { get; set; }
	public Guid? ThreadId { get; set; }
	public TalkThread ParentThread { get; set; } = null;
	public TalkThreadType Type { get; set; }
	public string Content { get; set; }
	public bool VisibleInChannel { get; set; }	
	public  TfUser User { get; set; }
	public DateTime CreatedOn { get; set; }
	public DateTime? LastUpdatedOn { get; set; } = null;
	public DateTime? DeletedOn { get; set; } = null;
	public long ConnectedDataIdentityValuesCount { get; set; } = 0;
    public List<TalkThread> SubThread { get; set; }
}

public class CreateTalkThreadWithRowIdModel
{
	public Guid ChannelId { get; set; }
	public TalkThreadType Type { get; set; }
	public string Content { get; set; }
	public Guid UserId { get; set; }
	public List<Guid> RowIds { get; set; } = new();
	public Guid DataProviderId { get; set; } = default!;
}

public class CreateTalkThreadWithDataIdentityModel
{
	public Guid ChannelId { get; set; }
	public TalkThreadType Type { get; set; }
	public string Content { get; set; }
	public Guid UserId { get; set; }
	public List<string> DataIdentityValues { get; set; }
}

public class CreateTalkSubThread
{
	public Guid ThreadId { get; set; }
	public bool VisibleInChannel { get; set; }
	public string Content { get; set; }
	public Guid UserId { get; set; }
}
