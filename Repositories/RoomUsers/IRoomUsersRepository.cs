using System.Collections.Generic;

using ChatApp.Models;

namespace ChatApp.Repositories
{
    //Note - all interactions related with creating, updating or deleting RoomUser
    //should be performed using User instance's RoomUser property since there's one-to-one relationship between them in the database,
    //therefore no methods performing those actions are described in this interface.
    public interface IRoomUsersRepository
    {
        /// <summary>
        /// Returns a collection that contains every user that is in any room now.
        /// </summary>
        /// <returns>Collection containing all RoomUsers in the database.</returns>
        public IEnumerable<RoomUser> GetAllUsers();
        /// <summary>
        /// Returns RoomUser instance with given ID.
        /// </summary>
        /// <param name="id">ID of the needed user.</param>
        /// <returns>RoomUser's instance from the database; null if there is no such user with given ID.</returns>
        public RoomUser GetUser(int id);
        /// <summary>
        /// Returns RoomUser instance with given username.
        /// </summary>
        /// <param name="userName">Username of the needed user.</param>
        /// <returns>RoomUser's instance from the database; null if there is no such user with given username.</returns>
        public RoomUser GetUser(string userName);
    }
}