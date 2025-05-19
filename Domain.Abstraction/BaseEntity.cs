namespace Domain.Abstraction
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class BaseEntity
    {
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
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// 软删除；因为软删除可能会有事件，所以设置IsDeleted必须通过方法
        /// </summary>
        public virtual void SoftDelete()
        {
            IsDeleted = true;
        }
    }
}
