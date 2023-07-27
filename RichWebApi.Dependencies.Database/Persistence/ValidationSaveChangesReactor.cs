using FluentValidation;
using FluentValidation.Results;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RichWebApi.Extensions;
using RichWebApi.Persistence.Internal;

namespace RichWebApi.Persistence;

[UsedImplicitly]
public class ValidationSaveChangesReactor : ISaveChangesReactor
{
	private readonly ILogger<ValidationSaveChangesReactor> _logger;
	private readonly IServiceProvider _serviceProvider;

	public ValidationSaveChangesReactor(ILogger<ValidationSaveChangesReactor> logger, IServiceProvider serviceProvider)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
	}

	public uint Order => 2;

	public async ValueTask ReactAsync(RichWebApiDbContext context, CancellationToken cancellationToken = default)
	{
		var failures = await _logger.TimeAsync(async () =>
		{
			var validatorsProvider = _serviceProvider.GetRequiredService<IEntityValidatorsProvider>();
			var entriesByType = context.ChangeTracker.Entries()
				.GroupBy(x => x.Metadata.ClrType)
				.ToDictionary(x => x.Key, x => x.Select(e => e.Entity).ToArray());
			var failures = new Dictionary<Type, IList<ValidationResult>>(entriesByType.Count);

			foreach (var group in entriesByType)
			{
				var asyncValidator = validatorsProvider.GetAsyncValidator(_serviceProvider, group.Key);

				if (asyncValidator is null)
				{
					continue;
				}

				var tasks = group.Value.Select(x => asyncValidator(x, cancellationToken));
				var groupResult = await Task.WhenAll(tasks);
				var groupFailures = groupResult.Where(x => !x.IsValid).ToArray();

				if (groupFailures.Length != 0)
				{
					failures[group.Key] = groupFailures;
				}
			}

			return failures;
		}, "Validate tracked db entities");

		if (failures.Count != 0)
		{
			throw new AggregateException("Entity entries validation failed",
				failures.SelectMany(x => x.Value.Select(v => new ValidationException(x.Key.Name, v.Errors, true))));
		}
	}
}