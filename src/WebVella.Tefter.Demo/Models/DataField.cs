using Bogus;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.Demo.Models;

public class DataField
{
	public DataFieldType Type { get; set; } //Should use DbType in the future
	public object Value { get; set; }

}

public enum DataFieldType
{
	Id,
	AutoIncrement,
	Number,
	Boolean,
	Date,
	DateTime,
	Text,
	Guid
}