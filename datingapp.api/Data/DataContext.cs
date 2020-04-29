using datingapp.api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace datingapp.api.Data
{
    public class DataContext : IdentityDbContext<User, Role, int, 
    IdentityUserClaim<int>,UserRole,IdentityUserLogin<int>
    ,IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        public  DataContext(DbContextOptions<DataContext> options): base (options)
        {

        }

        public DbSet<Value> Values { get; set; }

        public DbSet<Photos> photos { get; set; }

        public DbSet<Like> Likes {get; set;}

        public DbSet<Message> Messages {get; set;}


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

             builder.Entity<UserRole>(userRole => 
             {
                userRole.HasKey(ur => new {ur.UserId,ur.RoleId});
                
                userRole.HasOne(ur => ur.Roles)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();
                
                userRole.HasOne(ur => ur.Users)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.UserId)
                        .IsRequired();

             });
                

            builder.Entity<Like>()
                .HasKey(k => new {k.LikerId, k.LikeeId});

            builder.Entity<Like>()
                .HasOne(u => u.Likee)
                .WithMany(u => u.Liker)
                .HasForeignKey(u => u.LikeeId)//foreign key references a primary table ensuring that this id exists in the primary table which is users
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(u => u.Liker)
                .WithMany(u => u.Likee)
                .HasForeignKey(u => u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}