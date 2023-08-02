using System.Linq.Expressions;
using AutoMapper;
namespace RichWebApi.Mappers;

public static class MappingExpressionExtensions
{
	public static IMappingExpression<TSource, TDestination> IgnoreMember<TSource, TDestination, TDestinationMember>(
		this IMappingExpression<TSource, TDestination> expr,
		Expression<Func<TDestination, TDestinationMember>> memberExpr)
		where TSource : class
		where TDestination : class
		=> expr.ForMember(memberExpr, x => x.Ignore());
}