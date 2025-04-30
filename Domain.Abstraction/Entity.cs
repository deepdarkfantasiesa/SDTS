namespace Domain.Abstraction
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class Entity
    {

        /// <summary>
        /// 领域事件集合
        /// </summary>
        private List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        /// <summary>
        /// 领域事件集合
        /// </summary>
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// 添加一个领域事件
        /// </summary>
        /// <param name="eventItem">领域事件</param>
        public void AddDomainEvent(IDomainEvent eventItem) => _domainEvents.Add(eventItem);

        /// <summary>
        /// 移除一个领域事件
        /// </summary>
        /// <param name="eventItem">领域事件</param>
        public void RemoveDomainEvent(IDomainEvent eventItem) => _domainEvents.Remove(eventItem);

        /// <summary>
        /// 清除所有领域事件
        /// </summary>
        public void ClearDomainEvents() => _domainEvents.Clear();

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateAt { get; set; }

        /// <summary>
        /// 是否被删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// 泛型实体基类
    /// </summary>
    /// <typeparam name="TKey">强类型id继承类</typeparam>
    public abstract class Entity<TKey> : Entity
        where TKey : notnull, IEntityTypeId
    {
        /// <summary>
        /// id
        /// </summary>
        public TKey Id { get; protected set; }
    }
}
