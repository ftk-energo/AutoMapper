using System;
using System.Linq.Expressions;

namespace AutoMapper
{
	public interface IMappingExpression<TSource, TDestination>
	{
		IMappingExpression<TSource, TDestination> ForMember(Expression<Func<TDestination, object>> destinationMember, Action<IMemberConfigurationExpression<TSource>> memberOptions);
		void ForAllMembers(Action<IMemberConfigurationExpression<TSource>> memberOptions);
		IMappingExpression<TSource, TDestination> Include<TOtherSource, TOtherDestination>() where TOtherSource : TSource where TOtherDestination : TDestination;
		IMappingExpression<TSource, TDestination> WithProfile(string profileName);
		void ConvertUsing(Func<TSource, TDestination> mappingFunction);
		void ConvertUsing(Func<ITypeConverter> converter);
		void ConvertUsing(ITypeConverter converter);
	}

	public interface IMemberConfigurationExpression<TSource>
	{
		void SkipFormatter<TValueFormatter>() where TValueFormatter : IValueFormatter;
		IFormatterCtorExpression<TValueFormatter> AddFormatter<TValueFormatter>() where TValueFormatter : IValueFormatter;
		IFormatterCtorExpression AddFormatter(Type valueFormatterType);
		void AddFormatter(IValueFormatter formatter);
		void FormatNullValueAs(string nullSubstitute);
		IResolverConfigurationExpression<TSource, TValueResolver> ResolveUsing<TValueResolver>() where TValueResolver : IValueResolver;
		IResolverConfigurationExpression<TSource> ResolveUsing(Type valueResolverType);
		IResolutionExpression<TSource> ResolveUsing(IValueResolver valueResolver);
		void MapFrom(Func<TSource, object> sourceMember);
		void Ignore();
	}

	public interface IResolutionExpression<TSource>
	{
		IResolutionExpression<TSource> FromMember(Func<TSource, object> sourceMember);		
	}

	public interface IResolverConfigurationExpression<TSource, TValueResolver>
		where TValueResolver : IValueResolver
	{
		IResolverConfigurationExpression<TSource, TValueResolver> FromMember(Func<TSource, object> sourceMember);
		IResolverConfigurationExpression<TSource, TValueResolver> ConstructedBy(Func<TValueResolver> constructor);
	}

	public interface IResolverConfigurationExpression<TSource> : IResolutionExpression<TSource>
	{
		IResolutionExpression<TSource> ConstructedBy(Func<IValueResolver> constructor);
	}
}