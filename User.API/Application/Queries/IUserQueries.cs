namespace User.API.Application.Queries
{
    public interface IUserQueries
    {
        Task<UserViewModel> GetUserAsync(int id);

        Task<IEnumerable<UserViewModel>> GetAllUsers();
    }
}
