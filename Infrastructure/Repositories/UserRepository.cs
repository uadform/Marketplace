using Core.Models;
using Core.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> CreateUserAsync(int externalUserId)
        {
            var sql = @"
            INSERT INTO users (external_user_id)
            VALUES (@ExternalUserId)
            RETURNING user_id;";

            var userId = await _dbConnection.ExecuteScalarAsync<int>(sql, new { ExternalUserId = externalUserId });
            return userId;
        }

    }
}
