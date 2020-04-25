using System;
using System.Threading.Tasks;
using datingapp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace datingapp.api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context=context;
        }

        public async Task<User> Login(string userName, string password)
        {
            var user= await _context.Users.FirstOrDefaultAsync( x =>x.UserName ==userName);
            if(user ==null)
            return null;
            
            if(!VerifyPasswordHash(password,user.PasswordSalt,user.PasswordHash))
            return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordsalt, byte[] passwordhash)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordsalt))
            {
            var computedHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            for(int i=0;i <computedHash.Length;i++)
            {
                if(computedHash[i] != passwordhash[i]) return false;
                
            }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash=passwordHash;
            user.PasswordSalt=passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
            passwordSalt= hmac.Key;
            passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }

        public async Task<bool> UserExists(string useName)
        {
            if(await _context.Users.AnyAsync(x =>x.UserName ==useName))
            {
                return true;
            }
            return false;
        }
    }
}