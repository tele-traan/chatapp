using Microsoft.EntityFrameworkCore;
using ChatApp.Models;
namespace ChatApp.DB
{
    public class DBContent : DbContext
    {
        public DBContent(DbContextOptions<DBContent> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
       //public DbSet<RoomsUsers> RoomsUsers { get; set; }
    }
}