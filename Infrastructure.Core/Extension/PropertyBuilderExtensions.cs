using Domain.Abstraction;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Extension
{
    public static class PropertyBuilderExtensions
    {
        /// <summary>
        /// 强类型Id配置转换器的扩展方法
        /// </summary>
        /// <typeparam name="TStronglyTypedId">强类型Id的类型</typeparam>
        /// <param name="propertyBuilder">属性构建器</param>
        /// <returns>属性构建器</returns>
        public static PropertyBuilder<TStronglyTypedId> HasStronglyTypedIdConversion<TStronglyTypedId>(
            this PropertyBuilder<TStronglyTypedId> propertyBuilder)
            where TStronglyTypedId : GuidEntityTypeId
        {
            return propertyBuilder.HasConversion(
                id => id.Value, // 强类型 ID 转换为 Guid
                value => (TStronglyTypedId)Activator.CreateInstance(typeof(TStronglyTypedId), value)); // Guid 转换为强类型 ID
        }

        /// <summary>
        /// 强类型Id生成策略配置方法
        /// </summary>
        /// <typeparam name="TStronglyTypedId"></typeparam>
        /// <param name="propertyBuilder"></param>
        /// <returns></returns>
        public static PropertyBuilder<TStronglyTypedId> HasStronglyTypedIdValueGenerator<TStronglyTypedId>(
            this PropertyBuilder<TStronglyTypedId> propertyBuilder)
            where TStronglyTypedId : GuidEntityTypeId
        {
            return propertyBuilder.HasValueGenerator<StrongTypedIdValueGenerator<TStronglyTypedId>>();
        }

    }
}
