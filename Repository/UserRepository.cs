using Dapper;
using dofdir_komek.Models;
using MySqlConnector;

namespace dofdir_komek.Repository
{
    public sealed class UserRepository : IUserRespository
    {
        private readonly string _connectionString;
        public UserRepository(IConfiguration configuration) {
            _connectionString = configuration["ConnectionStrings:mysql"];
        }

        public Task<User?> GetUserByCredentialsAsync(string email, string password)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            return connection.QueryFirstOrDefaultAsync<User?>(
                @"SELECT * FROM users WHERE email = @Email AND password = @Password LIMIT 1",
                new
                {
                    Email = email, Password = password
                });
        }

        public async Task<bool> SaveUserAsync(User user)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            try
            {
                await connection.ExecuteAsync(
                    @"INSERT INTO users(name, email, password, role) VALUES(@Name, @Email, @Password, @Role)",
                    new
                    {
                        Name = user.Name,
                        Email = user.Email,
                        Password = user.Password,
                        Role = user.Role,
                    });
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
                
                return false;
            }
            
            return true;
        }
    }
}
