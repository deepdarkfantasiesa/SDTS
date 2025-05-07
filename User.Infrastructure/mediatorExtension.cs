using Domain.Abstraction;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace User.Infrastructure
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
            var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent, cancellationToken);
        }
    }
}
