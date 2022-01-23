using System;
using System.Linq;

using ChatApp.DB;
using ChatApp.Models;

namespace ChatApp.Repositories
{
    public class BanInfoRepository : IBanInfoRepository
    {
        private readonly ChatDbContext _context;
        public BanInfoRepository(ChatDbContext context)
        {
            _context = context;
        }
        public BanInfo GetBanInfo(User user, Room room)
        {
            if (user is null || room is null) return null;
            return _context.BanInfos.FirstOrDefault(b => b.Room.Equals(room) && b.User.Equals(user));
        }
        public void AddBanInfo(DateTime dateTime, string reason, string punisherName, User user, Room room)
        {
            var banInfo = new BanInfo { Until = dateTime, User = user, Room = room, Reason = reason, PunisherName =punisherName };
            _context.BanInfos.Add(banInfo);
            _context.SaveChanges();
        }
        public void RemoveBanInfo(BanInfo banInfo)
        {
            _context.BanInfos.Remove(banInfo);
            _context.SaveChanges();
        }
    }
}