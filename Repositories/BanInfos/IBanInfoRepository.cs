using System;

using ChatApp.Models;
namespace ChatApp.Repositories
{
    public interface IBanInfoRepository
    {
        BanInfo GetBanInfo(User user, Room room);
        void AddBanInfo(DateTime dateTime, string reason, string punisherName, User user, Room room);
        void RemoveBanInfo(BanInfo banInfo);
    }
}
