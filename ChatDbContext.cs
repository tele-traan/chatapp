using Microsoft.EntityFrameworkCore;
using ChatApp.Models;
namespace ChatApp.DB
{
    public class ChatDbContext : DbContext
    {   
        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<RegularUser>()
                .HasOne(re => re.RoomUser)
                .WithOne(ro => ro.RegularUser)
                .IsRequired()
                .HasForeignKey<RoomUser>(ro => ro.RegularUserId);

            builder.Entity<RegularUser>()
                .HasOne(re => re.GlobalChatUser)
                .WithOne(gc => gc.RegularUser)
                .IsRequired()
                .HasForeignKey<GlobalChatUser>(gc => gc.RegularUserId);

            builder.Entity<Room>()
                .HasMany(r => r.Users)
                .WithOne(ru => ru.Room)
                .IsRequired()
                .HasForeignKey(r => r.RoomId);

            builder.Entity<RoomUser>()
                .HasOne(ru => ru.Room)
                .WithMany(r => r.Users)
                .IsRequired()
                .HasForeignKey(ru => ru.RoomId);
            
        }
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<RegularUser> RegularUsers { get; set; }
        public DbSet<GlobalChatUser> GlobalChatUsers { get; set; }
        public DbSet<RoomUser> RoomUsers { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}