namespace Domain.Abstraction
{
    /// <summary>
    /// 子实体基类
    /// </summary>
    /// <typeparam name="TKey">强类型id类型</typeparam>
    public abstract class Entity<TKey> : BaseEntity
        where TKey : notnull, IEntityTypeId
    {
        /// <summary>
        /// id
        /// </summary>
        public TKey Id { get; protected set; }
    }
}
