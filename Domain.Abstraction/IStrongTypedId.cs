using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Loader;

namespace Domain.Abstraction
{
    /// <summary>
    /// 实体类Id基本接口
    /// </summary>
    public interface IEntityTypeId { }

    /// <summary>
    /// 泛型实体类Id基本接口
    /// </summary>
    /// <typeparam name="T">id类型</typeparam>
    public interface IEntityTypeId<T> : IEntityTypeId
    {
        T Value { get; }
    }

    /// <summary>
    /// 强类型id转换器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StronglyTypedIdConverter<T> : TypeConverter where T : IEntityTypeId<Guid>
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string stringValue && Guid.TryParse(stringValue, out var guid))
            {
                return Activator.CreateInstance(typeof(T), guid);
            }

            throw new NotSupportedException($"Invalid format for {typeof(T).Name}.");
        }
    }

    /// <summary>
    /// 拓展方法
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// 注册强类型id转换器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblyName">程序集名称</param>
        /// <returns></returns>
        public static IServiceCollection AddStrongTypeConverter(this IServiceCollection services, string assemblyName)
        {
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
            var types = assembly.GetTypes();

            // 查找实现了 IEntityTypeId<Guid> 接口的类型
            var stronglyTypedIdTypes = types
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeId<>) &&
                    i.GetGenericArguments()[0] == typeof(Guid)) // 确保泛型参数是 Guid
                );

            foreach (var type in stronglyTypedIdTypes)
            {
                TypeDescriptor.AddAttributes(type, new TypeConverterAttribute(typeof(StronglyTypedIdConverter<>).MakeGenericType(type)));
            }

            return services;
        }
    }
}
