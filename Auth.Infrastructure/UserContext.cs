using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Domain.AggregatesModel.UserAggregate;
using Domain.Abstraction;
using DotNetCore.CAP;
using Infrastructure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace Auth.Infrastructure
{
    public class UserContext : DbContext, IDbTransaction
    {
        private readonly IMediator _mediator;

        public UserContext(DbContextOptions<UserContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Role { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //为所有继承自Entity的实体配置Entity下的字段
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(e => typeof(Entity).IsAssignableFrom(e.ClrType)))
            {
                //配置创建字段
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(Entity.CreateAt))
                    .HasColumnName("create_at")
                    .IsRequired(true);

                //配置更新字段
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(Entity.UpdateAt))
                    .HasColumnName("update_at")
                    .IsRequired(false);

                //配置软删除字段
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(Entity.IsDeleted))
                    .HasColumnName("is_deleted")
                    .HasDefaultValue(false)
                    .IsRequired(true);

                // 配置全局查询过滤器
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var filter = Expression.Lambda(
                    Expression.Equal(
                        Expression.Property(parameter, nameof(Entity.IsDeleted)),
                        Expression.Constant(false)
                    ),
                    parameter
                );
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        #region SaveChange

        /// <summary>
        /// 先分发领域事件再持久化实体（未显示开启事务时调用）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (HasActiveTransaction)
                throw new ArgumentException("已显示开启事务，请调用SaveEntitiesAsync");

            //分发领域事件
            await _mediator.DispatchDomainEventsAsync(this, cancellationToken);

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 先持久化实体到数据库并分发领域事件（已显示开启事务时调用）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            if (!HasActiveTransaction)
                throw new ArgumentException("未显示开启事务，请调用SaveChangesAsync");

            //先持久化聚合，以防后面的command因为没有Id取不到之前的聚合
            var result = await base.SaveChangesAsync(cancellationToken);

            //分发领域事件
            await _mediator.DispatchDomainEventsAsync(this, cancellationToken);

            return true;
        }

        #endregion

        #region 事务

        /// <summary>
        /// 事务对象
        /// </summary>
        private IDbContextTransaction _currentTransaction;

        /// <summary>
        /// 是否开启事务
        /// </summary>
        public bool HasActiveTransaction => _currentTransaction != null;

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public void BeginTransaction(ICapPublisher capPublisher, CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                return;

            //开启并与cap共享事务（这里有个惊天大坑，一定不要用异步的，如果用异步的会导致领域事件注入的cap发布者没有事务对象）
            _currentTransaction = Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead, capPublisher, autoCommit: false);

        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (!HasActiveTransaction)
                return;

            await _currentTransaction.CommitAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        public async Task RollbackTransaction(CancellationToken cancellationToken = default)
        {
            if (!HasActiveTransaction)
                return;

            try
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        #endregion
    }
}
