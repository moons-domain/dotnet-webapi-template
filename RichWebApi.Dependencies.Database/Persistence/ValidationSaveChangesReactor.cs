using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi.Persistence;

public class ValidationSaveChangesReactor : ISaveChangesReactor
{
	private readonly IServiceProvider _serviceProvider;

	public ValidationSaveChangesReactor(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public uint Order => 2;

	public async ValueTask ReactAsync(RichWebApiDbContext context, CancellationToken cancellationToken = default)
	{
		var entriesByType = context.ChangeTracker.Entries()
			.GroupBy(x => x.Metadata.ClrType)
			.ToDictionary(x => x.Key, x => x.ToArray());
		var validatorType = typeof(IValidator<>);
		var failures = new Dictionary<Type, ValidationResult>(entriesByType.Count);
		foreach (var group in entriesByType)
		{
			var concreteValidatorType = typeof(IEnumerable<>).MakeGenericType(validatorType.MakeGenericType(group.Key));
			var validator = (IValidator)_serviceProvider.GetRequiredService(concreteValidatorType);
			var validationResult =
				await validator.ValidateAsync(new ValidationContext<IEnumerable<object>>(group.Value),
					cancellationToken);
			if (!validationResult.IsValid)
			{
				failures[group.Key] = validationResult;
			}
		}

		if (failures.Count != 0)
		{
			throw new AggregateException("Entity entries validation failed",
				failures.Select(x => new ValidationException(x.Key.Name, x.Value.Errors)));
		}
	}
}