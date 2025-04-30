using Domain.Abstraction;
using DotNetCore.CAP;
using Infrastructure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using User.Domain.AggregatesModel.UserAggregate;
using User.Infrastructure.EntityConfigurations;

namespace User.Infrastructure
{
    public class UserContext : DbContext, IUnitOfWork, IDbTransaction
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        public UserContext(DbContextOptions<UserContext> options, IMediator mediator, IConfiguration configuration) : base(options)
        {
            _mediator = mediator;
            _configuration = configuration;
            //Console.WriteLine($"context is created ,Id:{this.ContextId.InstanceId}");
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
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //	optionsBuilder.AddInterceptors(new MasterSlaveShiftInterceptor(_configuration.GetSection("master").Value, _configuration.GetSection("slaves").Value));
        //	base.OnConfiguring(optionsBuilder);
        //}

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEventsAsync(this);
            var result = await base.SaveChangesAsync(cancellationToken);

            //var result = await base.SaveChangesAsync(cancellationToken);
            //await _mediator.DispatchDomainEventsAsync(this);
            return true;
        }

        private IDbContextTransaction _currentTransaction;
        public bool HasActiveTransaction => _currentTransaction != null;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                return null;
            // _currentTransaction=await Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
            //_currentTransaction = Database.BeginTransaction(_capBus, autoCommit:false);
            return _currentTransaction;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsyncTest(ICapPublisher capBus)
        {
            if (_currentTransaction != null)
                return null;
            _currentTransaction = Database.BeginTransaction(capBus, autoCommit: false);
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
