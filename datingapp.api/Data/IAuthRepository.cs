using System.Threading.Tasks;
using datingapp.api.Models;

namespace datingapp.api.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user,string password);

        Task<User> Login(string userNmae,string password);

        Task<bool> UserExists(string useName);
    }
}