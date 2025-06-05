using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json.Serialization;
using System.Text.Json;

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
    /// 强类型 ID 转换器
    /// </summary>
    /// <typeparam name="T">强类型 ID 类型</typeparam>
    public class StronglyTypedIdConverterV2<T> : TypeConverter where T : IEntityTypeId<Guid>
    {
        private static readonly Func<Guid, T> _factory;

        static StronglyTypedIdConverterV2()
        {
            // 使用表达式树缓存构造函数，避免反射性能开销
            var constructor = typeof(T).GetConstructor(new[] { typeof(Guid) });
            if (constructor == null)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} must have a constructor with a single Guid parameter.");
            }

            var parameter = Expression.Parameter(typeof(Guid), "guid");
            var newExpression = Expression.New(constructor, parameter);
            _factory = Expression.Lambda<Func<Guid, T>>(newExpression, parameter).Compile();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(Guid) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue && Guid.TryParse(stringValue, out var guid))
            {
                return _factory(guid);
            }

            if (value is Guid guidValue)
            {
                return _factory(guidValue);
            }

            throw new NotSupportedException($"Cannot convert from {value?.GetType().Name ?? "null"} to {typeof(T).Name}.");
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || destinationType == typeof(Guid) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is T stronglyTypedId)
            {
                if (destinationType == typeof(string))
                {
                    return stronglyTypedId.Value.ToString();
                }

                if (destinationType == typeof(Guid))
                {
                    return stronglyTypedId.Value;
                }
            }

            throw new NotSupportedException($"Cannot convert from {typeof(T).Name} to {destinationType.Name}.");
        }
    }

    /// <summary>
    /// 强类型Id的json转换器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StronglyTypedIdJsonConverter<T> : JsonConverter<T> where T : IEntityTypeId<Guid>
    {
        private static readonly Func<Guid, T> _factory;

        static StronglyTypedIdJsonConverter()
        {
            // 使用表达式树缓存构造函数，避免反射性能开销
            var constructor = typeof(T).GetConstructor(new[] { typeof(Guid) });
            if (constructor == null)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} must have a constructor with a single Guid parameter.");
            }

            var parameter = Expression.Parameter(typeof(Guid), "guid");
            var newExpression = Expression.New(constructor, parameter);
            _factory = Expression.Lambda<Func<Guid, T>>(newExpression, parameter).Compile();
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && Guid.TryParse(reader.GetString(), out var guid))
            {
                return _factory(guid);
            }

            throw new JsonException($"Invalid JSON value for {typeof(T).Name}. Expected a GUID string.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString());
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
                TypeDescriptor.AddAttributes(type, new TypeConverterAttribute(typeof(StronglyTypedIdConverterV2<>).MakeGenericType(type)));
            }

            return services;
        }
    }
}
