namespace WebVella.Tefter;

public partial interface ITfDataProviderManager
{
}

public partial class TfDataProviderManager : ITfDataProviderManager
{
	internal class TfDataProviderCreateValidator
		: AbstractValidator<TfDataProviderModel>
	{
		public TfDataProviderCreateValidator(
			IDboManager dboManager,
			ITfDataProviderManager providerManager)
		{
			RuleFor(provider => provider.Id)
					.NotEmpty()
					.WithMessage("The data provider id is required.");

			RuleFor(provider => provider.Name)
					.NotEmpty()
					.WithMessage("The data provider name is required.");

				RuleFor(provider => provider.ProviderType)
					.NotEmpty()
					.WithMessage("The data provider type is required.");

			RuleFor(provider => provider.Id)
					.Must(id => { return providerManager.GetProvider(id).Value == null; })
					.WithMessage("There is already existing data provider with specified identifier.");

			RuleFor(provider => provider.Name)
					.Must(name => { return providerManager.GetProvider(name).Value == null; })
					.WithMessage("There is already existing data provider with specified name.");

		}

		public ValidationResult ValidateCreate(TfDataProviderModel provider)
		{
			if (provider == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The data provider model is null.") });

			return this.Validate(provider);
		}
	}
}
