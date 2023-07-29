using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RichWebApi.Config;
using RichWebApi.Extensions;
using RichWebApi.Persistence.Internal;

namespace RichWebApi.Persistence.Interceptors;

internal class ValidationSaveChangesInterceptor : SaveChangesInterceptor, IOrderedInterceptor, IDisposable
{
	private readonly ILogger<ValidationSaveChangesInterceptor> _logger;
	private readonly IOptionsMonitor<DatabaseEntitiesConfig> _configMonitor;
	private readonly IServiceScope _scope;

	public uint Order => 1;

	public ValidationSaveChangesInterceptor(IServiceProvider serviceProvider, ILogger<ValidationSaveChangesInterceptor> logger, IOptionsMonitor<DatabaseEntitiesConfig> configMonitor)
	{
		_logger = logger;
		_configMonitor = configMonitor;
		_scope = serviceProvider.CreateScope();
	}

	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
	                                                                            CancellationToken cancellationToken = default)
	{
		await base.SavingChangesAsync(eventData, result, cancellationToken);
		if (eventData.Context is null)
		{
			_logger.LogWarning("Database context is null");
			return result;
		}

		var context = eventData.Context;
		var sp = _scope.ServiceProvider;
		var validationOption = _configMonitor.CurrentValue.Validation; 
		if (validationOption == EntitiesValidationOption.None)
		{
			return result;
		}
		
		var failures = await _logger.TimeAsync(async () =>
		{
			var validatorsProvider = sp.GetRequiredService<IEntityValidatorsProvider>();
			var entriesByType = context.ChangeTracker.Entries()
				.GroupBy(x => x.Metadata.ClrType)
				.ToDictionary(x => x.Key, x => x.Select(e => e.Entity).ToArray());
			var failures = new Dictionary<Type, IList<ValidationResult>>(entriesByType.Count);

			foreach (var group in entriesByType)
			{
				var asyncValidator = validatorsProvider.GetAsyncValidator(sp, group.Key);
				
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

		return result;
	}

	public void Dispose() => _scope.Dispose();

	public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
		=> throw new NotSupportedException();

	public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
		=> throw new NotSupportedException();

	public override void SaveChangesCanceled(DbContextEventData eventData) 
		=> throw new NotSupportedException();

	public override void SaveChangesFailed(DbContextErrorEventData eventData) 
		=> throw new NotSupportedException();

	public override InterceptionResult ThrowingConcurrencyException(ConcurrencyExceptionEventData eventData,
	                                                                InterceptionResult result)
		=> throw new NotSupportedException();
}