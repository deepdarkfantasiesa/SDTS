namespace Infrastructure.Core.Extension
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举类型的所有值并返回数组
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>枚举类型的数组</returns>
        public static T[] GetAllValuesAsArray<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }
    }
}
