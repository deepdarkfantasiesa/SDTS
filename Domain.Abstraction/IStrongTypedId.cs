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
        T Id { get; init; }
    }

    /// <summary>
    /// guid类型实体Id
    /// </summary>
    public record GuidEntityTypeId : IEntityTypeId<Guid>
    {
        public Guid Id { get; init; }

        public GuidEntityTypeId(Guid id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// 长整型实体Id
    /// </summary>
    public interface ILongIntEntityTypeId : IEntityTypeId<long> { }
}
