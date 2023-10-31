using Dapper;
using MySqlConnector;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.API.Application.Queries
{
    public class UserQueries : IUserQueries
    {
        private readonly string _connectionstr;
        public UserQueries(string connectionstr)
        {
            _connectionstr = connectionstr;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            using (var connection = new MySqlConnection(_connectionstr))
            {
                connection.Open();
                var result = await connection.QueryAsync<dynamic>("SELECT * FROM userdb.User");
                return MapToUser(result);
            }
        }

        public async Task<User> GetUserAsync(int id)
        {
            using (var connection = new MySqlConnection(_connectionstr))
            {
                connection.Open();
                var res = await connection.QueryAsync<dynamic>(@"SELECT * FROM userdb.User where Id=@id", new { id });
                var result = res.First();
                User user = new User() { Id = result.Id,Name=result.Address_State,Email=result.Address_ZipCode };
                return user;
            }

        }

        private IEnumerable<User> MapToUser(dynamic dusers)
        {
            List<User> users = new List<User>();
            foreach (var item in dusers)
            {
                User user = new User()
                {
                    Id = item.Id,
                    Name = item.Address_Street,
                    Email = item.Address_City
                };
                users.Add(user);
            }
            return users;
        }
    }
}
