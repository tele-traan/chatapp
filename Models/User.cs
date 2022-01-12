using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
namespace ChatApp.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public byte[] Salt { get; set; }

        public RoomUser RoomUser { get; set; }
        public GlobalChatUser GlobalChatUser { get; set; }

        public List<Room> ManagedRooms { get; set; }
        public List<Room> RoomsWhereIsBanned { get; set; }
        public List<Room> RoomsCreated { get; set; }
        public List<BanInfo> BanInfos { get; set; }

        public User(string password)
        {
            ManagedRooms = new();
            RoomsWhereIsBanned = new();
            RoomsCreated = new();
            BanInfos = new();
            Salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(Salt);
            }
            PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt:Salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount:100000,
                numBytesRequested: 256/8));
        }
        public override bool Equals(object obj)
        {
            if (obj is not User) return false;

            User other = obj as User;

            return other.UserId == UserId
                && other.UserName == UserName;
        }
        public override int GetHashCode() => UserId.GetHashCode();
    }
}