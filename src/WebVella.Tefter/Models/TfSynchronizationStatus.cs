namespace WebVella.Tefter.Models;
public enum TfSynchronizationStatus
{
	[Description("Pending")]
	[TfColor(name: "gray",variable:"--tf-gray-500",oklch:"oklch(0.554 0.047 264.66)",hex:"#71717a",number:500, selectable: false)]
	Pending = 0,

	[Description("In Progress")]
	[TfColor(name: "blue",variable:"--tf-blue-500",oklch:"oklch(0.573 0.151 262.115)",hex:"#3b82f6",number:500, selectable: true)]
	InProgress = 1,
	
	[Description("Completed")]
	[TfColor(name: "green",variable:"--tf-green-500",oklch:"oklch(0.643 0.187 152.748)",hex:"#22c55e",number:500, selectable: true)]
	Completed = 2,
	
	[Description("Failed")]
	[TfColor(name: "red",variable:"--tf-red-500",oklch:"oklch(0.637 0.237 25.331)",hex:"#ef4444",number:500, selectable: true)]
	Failed = 3,
}
