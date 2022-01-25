using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ChatApp.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        private string _passwordHash;
        public string PasswordHash 
        { 
            get => _passwordHash;
            set
            {
                if (value is not null)
                {
                    _passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: value,
                        salt: Salt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8));
                }
            }
        }
        public byte[] Salt { get; set; }
        public List<RoomUser> RoomUsers { get; set; }
        public List<User> BannedUsers { get; set; }
        public List<BanInfo> BanInfos { get; set; }
        public List<User> Admins { get; set; }
        public List<Message> LastMessages { get; set; }
        public int? CreatorId { get; set; }
        public User Creator { get; set; }
        public Room()
        {
            RoomUsers = new();
            Admins = new();
            BanInfos = new();
            BannedUsers = new();

            Salt = new byte[128 / 8];
            using var rngCsp = new RNGCryptoServiceProvider() ;
            rngCsp.GetNonZeroBytes(Salt);
        }
        public override bool Equals(object obj)
        {
            if (obj is not Room) return false;
            Room other = obj as Room;
            return other.RoomId == RoomId
                && other.Name == Name;
        }
        public override int GetHashCode() => RoomId.GetHashCode();
    }
}