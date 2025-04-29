using Domain.Abstraction;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.Security.Cryptography;

namespace User.Infrastructure
{
    public class StrongTypedIdValueGenerator<TKey> : ValueGenerator<TKey>
    where TKey : IEntityTypeId
    {
        public override TKey Next(EntityEntry entry)
        {
            // 使用时序性 GUID 生成强类型 ID
            return (TKey)Activator.CreateInstance(typeof(TKey), SequentialGuidGenerator.NewGuid());
        }

        public override bool GeneratesTemporaryValues => false;
    }

    public static class SequentialGuidGenerator
    {
        public static Guid NewGuid()
        {
            // 获取当前时间戳（毫秒级）
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // 转换为字节数组
            var timestampBytes = BitConverter.GetBytes(timestamp);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes); // 转换为大端字节序
            }

            // 随机生成剩余的字节
            var randomBytes = RandomNumberGenerator.GetBytes(10);

            // 构造 GUID 字节数组
            var guidBytes = new byte[16];
            Array.Copy(timestampBytes, 2, guidBytes, 0, 6); // 时间戳占前 6 字节
            Array.Copy(randomBytes, 0, guidBytes, 6, 10);   // 随机数占后 10 字节

            // 设置版本号（Version 7）
            guidBytes[6] = (byte)((guidBytes[6] & 0x0F) | 0x70); // 设置高 4 位为 0111
            guidBytes[8] = (byte)((guidBytes[8] & 0x3F) | 0x80); // 设置高 2 位为 10

            return new Guid(guidBytes);
        }
    }
}
