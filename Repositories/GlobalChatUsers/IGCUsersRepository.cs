using System.Collections.Generic;

using ChatApp.Models;

namespace ChatApp.Repositories
{    
    //Note - all interactions related with creating or updating GlobalChatUser instance
    //should be performed using User instance's GlobalChatUser property since there's one-to-one relationship between them in the database,
    //thus no methods performing those actions are described in this interface. s
    public interface IGCUsersRepository
    {
        /// <summary>
        /// Returns a collection that contains every user that is in global chat now.
        /// </summary>
        /// <returns>Collection containing all GlobalChatUsers in the database.</returns>
        IEnumerable<GlobalChatUser> GetAllUsers();
        /// <summary>
        /// Returns GlobalChatUser instance with given ID.
        /// </summary>
        /// <param name="id">ID of the needed user.</param>
        /// <returns>GlobalChatUser's instance from the database; null if there is no such user with given ID.</returns>
        GlobalChatUser GetUser(int id);
        /// <summary>
        /// Returns GlobalChatUser instance with given username.
        /// </summary>
        /// <param name="userName">Username of the needed user.</param>
        /// <returns>GlobalChatUser's instance from the database; null if there is no such user with given username.</returns>
        GlobalChatUser GetUser(string userName);
        void RemoveUser(GlobalChatUser user);
    }
}
