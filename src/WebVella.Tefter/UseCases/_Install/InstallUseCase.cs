using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.UseCases.Install;
internal partial class InstallUseCase
{
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<InstallUseCase> LOC;

	public InstallUseCase(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>();
		_metaService = serviceProvider.GetService<ITfMetaService>();
		LOC = serviceProvider.GetService<IStringLocalizer<InstallUseCase>>();
	}
}
