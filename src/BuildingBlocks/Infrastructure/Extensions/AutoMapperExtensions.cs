using System.Reflection;
using AutoMapper;

namespace Infrastructure.Extensions;

public static class AutoMapperExtensions
{
    public static void IgnoreAllNonExisting<TSource, TDestination>
        (this IMappingExpression<TSource, TDestination> expression)
    {
        var flags = BindingFlags.Public | BindingFlags.Instance;
        var sourceType = typeof(TSource);
        var destinationProperties = typeof(TDestination).GetProperties(flags);

        foreach (var property in destinationProperties)
            if (sourceType.GetProperty(property.Name, flags) == null)
                expression.ForMember(property.Name, opt => opt.Ignore());
    }
}