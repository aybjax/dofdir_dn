using Dapper;
using dofdir_komek.DB;
using dofdir_komek.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace dofdir_komek.Repository
{
    public sealed class UserRepository : IUserRespository
    {
        private readonly AppDBContext _dbContext;
        private readonly string _connectionString;
        
        public UserRepository(IConfiguration configuration, AppDBContext dbContext)
        {
            _dbContext = dbContext;
            _connectionString = configuration["ConnectionStrings:mysql"];
        }

        public Task<User?> GetUserByCredentialsAsync(string email, string password)
        {
            // using var connection = new MySqlConnection(_connectionString);
            // connection.Open();
            //
            // return connection.QueryFirstOrDefaultAsync<User?>(
            //     @"SELECT * FROM users WHERE email = @Email AND password = @Password LIMIT 1",
            //     new
            //     {
            //         Email = email, Password = password
            //     });

            return _dbContext.Users.SingleOrDefaultAsync(user => 
                user.Email == email && user.Password == password);
        }

        public async Task<bool> SaveUserAsync(User user)
        {
            // using var connection = new MySqlConnection(_connectionString);
            // connection.Open();
            //
            // try
            // {
            //     await connection.ExecuteAsync(
            //         @"INSERT INTO users(name, email, password, role) VALUES(@Name, @Email, @Password, @Role)",
            //         new
            //         {
            //             Name = user.Name,
            //             Email = user.Email,
            //             Password = user.Password,
            //             Role = user.Role,
            //         });
            // }
            // catch (MySqlException e)
            // {
            //     Console.WriteLine(e);
            //     
            //     return false;
            // }
            //
            // return true;

            _dbContext.Users.Add(new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Role = user.Role,
            });

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                return false;
            }
                
            return true;
        }
    }
}
