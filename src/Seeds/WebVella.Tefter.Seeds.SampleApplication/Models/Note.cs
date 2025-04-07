using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Seeds.SampleApplication.Models;

public class Note
{
	public Guid Id { get; set; }
	public string NoteText { get; set; }
	public DateTime CreatedOn { get; set; }
	public Note()
	{
		Id = Guid.NewGuid();
		CreatedOn = DateTime.UtcNow;
	}
}
