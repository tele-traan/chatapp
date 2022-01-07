using Microsoft.EntityFrameworkCore;
using ChatApp.Models;
namespace ChatApp.DB
{
    public class ChatDbContext : DbContext
    {   
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasMany(ru => ru.ManagedRooms)
                .WithMany(r => r.Admins)
                .UsingEntity(e => e.ToTable("RoomsAdmins"));

            builder.Entity<User>()
                .HasOne(re => re.RoomUser)
                .WithOne(ro => ro.User)
                .IsRequired()
                .HasForeignKey<RoomUser>(ro => ro.UserId);

            builder.Entity<User>()
                .HasOne(re => re.GlobalChatUser)
                .WithOne(gc => gc.User)
                .IsRequired()
                .HasForeignKey<GlobalChatUser>(gc => gc.UserId);

            builder.Entity<User>().Navigation(u => u.GlobalChatUser).AutoInclude();
            builder.Entity<User>().Navigation(u => u.RoomUser).AutoInclude();
            builder.Entity<User>().Navigation(u => u.ManagedRooms).AutoInclude();
            //"Cycle detected while auto-including navigations: 'User.RoomUser', 'RoomUser.Room', 'Room.Admins'. */
            builder.Entity<Room>()
                .HasMany(r => r.Users)
                .WithOne(ru => ru.Room)
                .IsRequired()
                .HasForeignKey(r => r.RoomId);

            //builder.Entity<Room>().Navigation(r => r.Users).AutoInclude();

            builder.Entity<RoomUser>()
                .HasOne(ru => ru.Room)
                .WithMany(r => r.Users)
                .IsRequired()
                .HasForeignKey(ru => ru.RoomId);

            builder.Entity<RoomUser>().Navigation(ru => ru.User).AutoInclude();
            builder.Entity<RoomUser>().Navigation(ru => ru.Room).AutoInclude();

            builder.Entity<GlobalChatUser>().Navigation(gc => gc.User).AutoInclude();
        }
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<GlobalChatUser> GlobalChatUsers { get; set; }
        public DbSet<RoomUser> RoomUsers { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}