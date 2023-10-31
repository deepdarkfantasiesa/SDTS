using User.Domain.AggregatesModel.UserAggregate;

namespace User.API.Application.Queries
{
    public interface IUserQueries
    {
        Task<User> GetUserAsync(int id);

        Task<IEnumerable<User>> GetAllUsers();
    }
}
