using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.UseCases.Recipe;
internal partial class RecipeUseCase
{
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<RecipeUseCase> LOC;

	public RecipeUseCase(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>();
		_metaService = serviceProvider.GetService<ITfMetaService>();
		LOC = serviceProvider.GetService<IStringLocalizer<RecipeUseCase>>();
	}
}
