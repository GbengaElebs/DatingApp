using System.Collections.Generic;
using System.Threading.Tasks;
using datingapp.api.Helpers;
using datingapp.api.Models;

namespace datingapp.api.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T:class;

         Task<bool> SaveAll();

         Task<PagedList<User>> GetUsers(UserParams UserParams);

         Task<User> GetUser(int id);

         Task<Photos> GetPhoto(int id);

          Task<Photos> GetMainPhotoForUser(int id);

          Task<Like> GetLike(int userId,int recipientId);

          Task<Message> GetMessage (int id);
          Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);

          Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
          
    }
}