using Microsoft.EntityFrameworkCore;
using ChatApp.Models;
namespace ChatApp.DB
{
    public class DBContent : DbContext
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

            builder.Entity<RoomUser>()
                .HasOne(ro => ro.Room)
                .WithMany(r => r.Users)
                .IsRequired(false);
        }
        public DBContent(DbContextOptions<DBContent> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<RegularUser> RegularUsers { get; set; }
        public DbSet<GlobalChatUser> GlobalChatUsers { get; set; }
        public DbSet<RoomUser> RoomUsers { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}