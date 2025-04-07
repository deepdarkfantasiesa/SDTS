using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Core
{
    public interface IDbTransaction
    {
        IDbContextTransaction GetCurrentTransaction();

        bool HasActiveTransaction { get; }

        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<IDbContextTransaction> BeginTransactionAsyncTest(ICapPublisher publisher);

        Task CommitTransactionAsync(IDbContextTransaction transaction);

        void RollbackTransaction();
    }
}
