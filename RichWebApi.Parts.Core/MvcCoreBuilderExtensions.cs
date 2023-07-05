using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi.Parts.Core;

public static class MvcCoreBuilderExtensions
{
	public static IMvcCoreBuilder AddPart<T>(this IMvcCoreBuilder builder)
	{
		var a = typeof(T).Assembly;
		builder.AddApplicationPart(a);
		builder.Services
			.AddValidatorsFromAssembly(a, includeInternalTypes: true)
			.AddMediatR(x => x.RegisterServicesFromAssemblies(a));
		return builder;
	}
}