using FluentAssertions.Equivalency;
using RichWebApi.Entities;
using RichWebApi.Models;

namespace RichWebApi.Tests.FluentAssertions;

public static class EquivalencyAssertionOptionsExtensions
{
	public static EquivalencyAssertionOptions<T> ExcludingAuditableEntityProperties<T>(
		this EquivalencyAssertionOptions<T> options) where T : class
	{
		var auditableEntityType = typeof(IAuditableEntity);
		return options
			.Excluding(info => info.DeclaringType.IsAssignableTo(auditableEntityType)
								 && (info.Name == nameof(IAuditableEntity.CreatedAt)
									 || info.Name == nameof(IAuditableEntity.ModifiedAt)));
	}

	public static EquivalencyAssertionOptions<T> ExcludingSoftDeletableEntityProperties<T>(
		this EquivalencyAssertionOptions<T> options) where T : class
	{
		var softDeletableEntityType = typeof(ISoftDeletableEntity);
		return options
			.Excluding(info => info.DeclaringType.IsAssignableTo(softDeletableEntityType) && info.Name == nameof(ISoftDeletableEntity.DeletedAt));
	}

	public static EquivalencyAssertionOptions<T> ExcludingAuditableDtoProperties<T>(
		this EquivalencyAssertionOptions<T> options) where T : class
	{
		var auditableEntityType = typeof(IAuditableDto);
		return options
			.Excluding(info => info.DeclaringType.IsAssignableTo(auditableEntityType)
							   && (info.Name == nameof(IAuditableDto.CreatedAt)
								   || info.Name == nameof(IAuditableDto.ModifiedAt)));
	}

	public static EquivalencyAssertionOptions<T> ExcludingSoftDeletableDtoProperties<T>(
		this EquivalencyAssertionOptions<T> options) where T : class
	{
		var softDeletableEntityType = typeof(ISoftDeletableDto);
		return options
			.Excluding(info => info.DeclaringType.IsAssignableTo(softDeletableEntityType) && info.Name == nameof(ISoftDeletableDto.DeletedAt));
	}
}