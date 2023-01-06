namespace dofdir_komek.Repository
{
    public interface IUserRespository
    {
        public Task<bool> SaveUserAsync(Models.User user);

        public Task<Models.User?> GetUserByCredentialsAsync(string email, string password);
    }
}
