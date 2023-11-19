using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MySqlConnector;
using System.Text.Json;
using System.Text.Json.Nodes;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.API.Application.Queries
{
    public class UserQueries : IUserQueries
    {
        private readonly string _connectionstr;
        private readonly IDistributedCache _distributedCache;
 
        public UserQueries(string connectionstr,IDistributedCache distributedCache)
        {
            _connectionstr = connectionstr;
            _distributedCache = distributedCache;
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
                var res = await connection
                    .QueryAsync<dynamic>(@"SELECT * FROM userdb.User where Id=@id", new { id });
                var result = res.First();
                User user = new User()
                {
                    Id = result.Id,
                    Name = result.Address_State,
                    Email = result.Address_ZipCode
                };
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
