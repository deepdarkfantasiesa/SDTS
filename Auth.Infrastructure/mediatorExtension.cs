using Domain.Abstraction;
using Domain.Abstraction.Mediator;

//using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure
{
    public static class mediatorExtension
    {
        /// <summary>
        /// 分发领域事件
        /// </summary>
        /// <param name="mediator">中介者</param>
        /// <param name="ctx">数据库上下文</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext ctx, CancellationToken cancellationToken = default)
        {
            //查询所有已跟踪且拥有领域事件的聚合根
            var domainEntities = ctx.ChangeTracker
                .Entries()
                .Where(x => x.Entity is IAggregateRoot aggregateRoot
                    && aggregateRoot.DomainEvents.Any())
                .Select(x => (IAggregateRoot)x.Entity);

            //获取所有领域事件
            var domainEvents = domainEntities
                .SelectMany(x => x.DomainEvents)
                .ToList();

            //清空聚合根中所有领域事件
            domainEntities.ToList()
                .ForEach(entity => entity.ClearDomainEvents());

            //分发领域事件
            foreach (var domainEvent in domainEvents)
                await mediator.PublishAsync(domainEvent, cancellationToken);
        }
    }
}
