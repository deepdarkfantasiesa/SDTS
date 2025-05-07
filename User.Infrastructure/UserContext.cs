using Domain.Abstraction;
using DotNetCore.CAP;
using Infrastructure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.Infrastructure
{
    public class UserContext : DbContext, IDbTransaction
    {
        private readonly IMediator _mediator;
        private readonly ICapPublisher _capBus;

        public UserContext(DbContextOptions<UserContext> options, IMediator mediator, ICapPublisher capBus) : base(options)
        {
            _mediator = mediator;
            _capBus = capBus;
        }

        public DbSet<Users> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(e => typeof(Entity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType).Property<bool>(nameof(Entity.IsDeleted));
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var body = Expression.Equal(Expression.Call(typeof(EF), nameof(EF.Property), [typeof(bool)], parameter, Expression.Constant(nameof(Entity.IsDeleted))), Expression.Constant(false));
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));
            }

            //modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
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
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                return;

            //开启并与cap共享事务
            _currentTransaction = await Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, _capBus, autoCommit: false);
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
