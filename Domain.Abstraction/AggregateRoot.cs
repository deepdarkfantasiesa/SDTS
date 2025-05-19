namespace Domain.Abstraction
{
    /// <summary>
    /// 聚合根接口
    /// </summary>
    public interface IAggregateRoot
    {
        /// <summary>
        /// 领域事件
        /// </summary>
        IReadOnlyList<IDomainEvent> DomainEvents { get; }

        /// <summary>
        /// 添加一个领域事件
        /// </summary>
        /// <param name="eventItem">领域事件</param>
        public void AddDomainEvent(IDomainEvent eventItem);

        /// <summary>
        /// 移除一个领域事件
        /// </summary>
        /// <param name="eventItem">领域事件</param>
        public void RemoveDomainEvent(IDomainEvent eventItem);

        /// <summary>
        /// 清除所有领域事件
        /// </summary>
        public void ClearDomainEvents();
    }

    /// <summary>
    /// 聚合根基类
    /// </summary>
    public class BaseAggregateRoot : BaseEntity, IAggregateRoot
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
    }

    /// <summary>
    /// 聚合根
    /// </summary>
    /// <typeparam name="TKey">强类型id类型</typeparam>
    public class AggregateRoot<TKey>: BaseAggregateRoot
        where TKey : notnull,IEntityTypeId
    {
        /// <summary>
        /// id
        /// </summary>
        public TKey Id { get; protected set; }
    }
}
