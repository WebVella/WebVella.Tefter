namespace WebVella.Tefter.EmailSender.Models;

public class EmailAddress
{
	public string Name { get; set; } = string.Empty;
	public string Address { get; set; } = string.Empty;
	public EmailAddress() { }
	public EmailAddress(string address) => Address = address;
	public EmailAddress(string name, string address)
	{
		Name = name;
		Address = address;
	}

	public static EmailAddress FromMailboxAddress(MailboxAddress mbAddress)
	{
		return new EmailAddress { Name = mbAddress.Name, Address = mbAddress.Address };
	}
}