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
        public DbSet<GlobalChatUser> GlobalChatUsers { get; set; }
        public DbSet<RoomUser> RoomUsers { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}