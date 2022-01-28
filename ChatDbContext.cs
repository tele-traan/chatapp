using Microsoft.EntityFrameworkCore;
using ChatApp.Models;

namespace ChatApp.DB
{
    public class ChatDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasMany(u => u.ManagedRooms)
                .WithMany(r => r.Admins)
                .UsingEntity(e => e.ToTable("RoomsAdmins"));
            builder.Entity<User>()
                .HasMany(u => u.RoomsWhereIsBanned)
                .WithMany(r => r.BannedUsers)
                .UsingEntity(e => e.ToTable("RoomsBans"));
            builder.Entity<User>()
                .HasMany(u => u.BanInfos)
                .WithOne(b => b.User)
                .IsRequired(false)
                .HasForeignKey(b => b.UserId);
            builder.Entity<User>()
                .HasOne(u => u.RoomUser)
                .WithOne(ru => ru.User)
                .IsRequired()
                .HasForeignKey<RoomUser>(ru => ru.UserId);
            builder.Entity<User>()
                .HasOne(u => u.GlobalChatUser)
                .WithOne(gcu => gcu.User)
                .IsRequired()
                .HasForeignKey<GlobalChatUser>(gc => gc.UserId);
            builder.Entity<User>()
                .HasMany(u => u.RoomsCreated)
                .WithOne(r => r.Creator)
                .HasForeignKey(r => r.CreatorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Room>()
                .HasMany(r => r.RoomUsers)
                .WithOne(ru => ru.Room)
                .IsRequired()
                .HasForeignKey(r => r.RoomId);
            builder.Entity<Room>()
                .HasMany(r => r.BanInfos)
                .WithOne(b => b.Room)
                .IsRequired()
                .HasForeignKey(b => b.RoomId);
            builder.Entity<Room>()
                .HasMany(r => r.LastMessages)
                .WithOne(m => m.Room)
                .HasForeignKey(m => m.RoomId);

            builder.Entity<RoomUser>()
                .HasOne(ru => ru.Room)
                .WithMany(r => r.RoomUsers)
                .IsRequired()
                .HasForeignKey(ru => ru.RoomId);

            builder.Entity<BanInfo>()
                .HasOne(b => b.Room)
                .WithMany(r => r.BanInfos)
                .IsRequired()
                .HasForeignKey(b => b.RoomId);
            builder.Entity<BanInfo>()
                .HasOne(b => b.User)
                .WithMany(u => u.BanInfos)
                .IsRequired()
                .HasForeignKey(u => u.UserId);

            builder.Entity<BanInfo>().Navigation(b => b.User).AutoInclude();
            builder.Entity<BanInfo>().Navigation(b => b.Room).AutoInclude();

            /*builder.Entity<RoomUser>().Navigation(ru => ru.User).AutoInclude();
            builder.Entity<RoomUser>().Navigation(ru => ru.Room).AutoInclude();

            builder.Entity<GlobalChatUser>().Navigation(gc => gc.User).AutoInclude();

            builder.Entity<User>().Navigation(u => u.GlobalChatUser).AutoInclude();
            builder.Entity<User>().Navigation(u => u.RoomUser).AutoInclude();
            builder.Entity<User>().Navigation(u => u.ManagedRooms).AutoInclude();
            builder.Entity<User>().Navigation(u => u.RoomsWhereIsBanned).AutoInclude();
            builder.Entity<User>().Navigation(u => u.BanInfos).AutoInclude();

            builder.Entity<Room>().Navigation(r => r.LastMessages).AutoInclude();*/

        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.EnableSensitiveDataLogging();
        }
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<BanInfo> BanInfos { get; set; }
        public DbSet<GlobalChatUser> GlobalChatUsers { get; set; }
        public DbSet<RoomUser> RoomUsers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<GCMessage> GCMessages { get; set; }
    }
}