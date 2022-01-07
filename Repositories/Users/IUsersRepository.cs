using System.Collections.Generic;

using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IUsersRepository
    {
        /// <summary>
        /// Returns a collection that contains every user registered.
        /// </summary>
        /// <returns>Collection containing all users in the database.</returns>
        IEnumerable<User> GetAllUsers();
        /// <summary>
        /// Adds new instance of User to the database. 
        /// If the database already contains this user, does not do anything.
        /// </summary>
        /// <param name="user">New user to be added to the database.</param>
        /// <returns>false if the database already contains this user; otherwise, true.</returns>
        bool AddUser(User user);
        /// <summary>
        /// Removes specified user from the database. 
        /// If the database does not contain such user, does not do anything.
        /// </summary>
        /// <param name="user">Specified user to be removed from the database.</param>
        /// <returns>false if the database does not contain this user; otherwise, true.</returns>
        bool RemoveUser(User user);
        /// <summary>
        /// Returns User instance with UserId equal to give ID. 
        /// </summary>
        /// <param name="id">ID of the needed user.</param>
        /// <returns>User's instance from the database; null if there is no such user with given ID.</returns>
        /// <summary>
        /// Returns User instance with given username.
        /// </summary>
        /// <param name="userName">Username of the needed user.</param>
        /// <returns>User's instance from the database; null if there is no such user with given username.</returns>
        User GetUser(string userName);
        /// <summary>
        /// Returns the User with specified username and password. 
        /// If there is a user with given username, but his password differs from given, returns null.
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <param name="password">User's password</param>
        /// <returns>User instance from the database with given username and password. Null if there's no such.</returns>
        User GetUser(string userName, string password);
        /// <summary>
        /// Updates user's instance in the database;
        /// </summary>
        /// <param name="user">User's instance that needs to be updated.</param>
        void UpdateUser(User user);
    }
}