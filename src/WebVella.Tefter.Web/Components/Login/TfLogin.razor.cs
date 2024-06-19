using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Components;
public partial class TfLogin : TfFromBaseComponent
{
	internal TfLoginModel _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.InitForm(_form);
	}

	internal void _submit()
	{
		try
		{
			throw new Exception("boz");
		}
		catch (Exception ex)
		{
			ResponseUtils.ProcessResponse(this, ex,ToastService);
		}
	}
}

internal class TfLoginModel
{
	[Required(ErrorMessage = "email required")]
	[EmailAddress(ErrorMessage = "invalid email")]
	public string Email { get; set; }

	[Required(ErrorMessage = "password required")]
	[MinLength(5, ErrorMessage = "at least 5 chars required")]
	public string Password { get; set; }
}