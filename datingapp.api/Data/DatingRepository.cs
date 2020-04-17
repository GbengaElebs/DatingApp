using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingapp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace datingapp.api.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photos> GetMainPhotoForUser(int userId)
        {
            var getmainphotoforuser= await _context.photos.Where(p  => p.UserId == userId ).FirstOrDefaultAsync(p => p.IsMain);
            return getmainphotoforuser;
        }

        public async Task<Photos> GetPhoto(int id)
        {

            var photo= await _context.photos.FirstOrDefaultAsync(p =>p.Id == id);

            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user= await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id ==id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var user=await _context.Users.Include(p => p.Photos).ToListAsync();

            return user;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}