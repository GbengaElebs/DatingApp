using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingapp.api.Helpers;
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

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
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

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var user=_context.Users.Include(p => p.Photos)
            .OrderByDescending(u => u.LastActive).AsQueryable();

            user = user.Where(u => u.Id != userParams.UserId);

            user = user.Where(u => u.Gender == userParams.Gender);
            
            if(userParams.Liker)
            {
                var userlikes = await GetUserLikes(userParams.UserId, userParams.Liker);
                user= user.Where(u => userlikes.Contains(u.Id));
                
            }
            if(userParams.Likee)
            {
                var userlikees = await GetUserLikes(userParams.UserId, userParams.Liker);
                user= user.Where(u => userlikees.Contains(u.Id));
            }
            if(userParams.MinAge !=18 || userParams.MaxAge !=99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge -1);
                var maxDob= DateTime.Today.AddYears(-userParams.MinAge);
                user = user.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch(userParams.OrderBy)
                {
                    case "created":
                        user = user.OrderByDescending(u => u.Created);
                    break;
                    default:
                        user =user.OrderByDescending(u => u.LastActive);
                    break;
                }
            }

            return await PagedList<User>.CreateAsync(user, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.Include(x => x.Liker)
            .Include(x => x.Likee)
            .FirstOrDefaultAsync(u => u.Id == id);

            if(likers)
            {
                return user.Liker.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likee.Where(u => u.LikerId == id).Select(i => i.LikeeId);

            }
        }
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(u => u.Sender)
            .ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .AsQueryable();

            switch(messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
                    break;
                    case "Outbox":
                    messages = messages.Where(u =>u.SenderId == messageParams.UserId && u.SenderDeleted == false);
                    break;

                    default:
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.IsRead == false && u.RecipientDeleted == false
                    );

                    break;

            }

            messages = messages.OrderByDescending(d => d.DateSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber,
            messageParams.PageSize);

        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages.Include(u => u.Sender)
            .ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where(m => m.RecipientId == userId &&m.RecipientDeleted == false && m.SenderId == recipientId 
            || m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted == false).OrderBy(m => m.DateSent)
            .ToListAsync();

            return messages;
        }
    }
}