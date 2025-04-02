using Azure.Core;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MySqlConnector;
using Npgsql;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        public async Task<IEnumerable<UserViewModel>> GetAllUsers()
        {
            using (var connection = new MySqlConnection(_connectionstr))
            {
                connection.Open();
                var result = await connection.QueryAsync<dynamic>("SELECT * FROM userdb.User");
                return MapToUser(result);
            }
        }

        public async Task<UserViewModel> GetUserAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionstr))
            {
                connection.Open();
                var res = await connection
                    .QueryAsync<UserViewModel>(@"SELECT 
                        u.""Id"",
                        u.""Name"" 
                       FROM ""User"" AS u 
                        WHERE u.""Id""=@Id",
                new
                {
                    Id = id
                });
                var result = res.First();

                return result;
            }
        }

        private IEnumerable<UserViewModel> MapToUser(dynamic dusers)
        {
            List<UserViewModel> users = new List<UserViewModel>();
            foreach (var item in dusers)
            {
                UserViewModel user = new UserViewModel()
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
