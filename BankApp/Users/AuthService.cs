using System;
using System.Collections.Generic;
using System.Text;
using BankApp.GustafsClasses;

namespace BankApp
{
    //Result of a login attempt. Can be used in menu.
    public enum LoginResult
    {
        Success,
        WrongCredentials,
        LockedOut
    }
    /*
 //AuthService handles authentication and permissions:
 It uses UserService to find users, but does not store any users itself.
*/
    internal class AuthService
    {
        private readonly UserService _userService;

        // Currently logged in user.
        public User? LoggedInUser { get; private set; }

        public AuthService(UserService userService)
        {
            _userService = userService;
        }

        public LoginResult Login(string username, string password)
        {
            User? user = _userService.GetUserByUsername(username);

            if (user == null)
                return LoginResult.WrongCredentials;

            if (user.IsLockedOut)
                return LoginResult.LockedOut;

            if (!user.CheckPassword(password))
            {
                user.RegisterFailedLogin();

                if (user.IsLockedOut)
                    return LoginResult.LockedOut;

                return LoginResult.WrongCredentials;
            }

            user.ResetFailedLogins();
            LoggedInUser = user;
            return LoginResult.Success;
        }

        //Logs out the current user by clearing LoggedInUser.
        public void Logout()
        {
            LoggedInUser = null;
        }

        //PERMISSIONS
        public bool IsAdmin(User? user)
        {
            return user != null &&
                (user.Role == UserRole.Admin || user.Role == UserRole.Owner);
        }

        public bool IsLoggedInUserAdmin()
        {
            return IsAdmin(LoggedInUser);
        }

        //Returns true if the currently logged-in user is an Owner. Returns false if no user is logged in.
        public bool IsOwner(User? user)
        {
            return user != null && user.Role == UserRole.Owner;
        }

        public bool IsLoggedInUserOwner()
        {
            return IsOwner(LoggedInUser);
        }

        /*
 Returns true if the logged-in user is allowed to create Admin accounts.
 According to our user stories, only the Owner can create Admins.
 A regular Admin can only create Customer accounts.
 The menu can use this to decide whether to show the "create admin" option.
 The real check is also done in UserService.CreateUser, so the rule holds
 even if the menu forgets to call this method.
           */
        public bool CanCreateAdminAccount()
        {
            return LoggedInUser != null && LoggedInUser.Role == UserRole.Owner;
        }

        //Unlocks a locked account. Only an Admin or Owner can unlock accounts.
        public bool UnlockUser(string username)
        {
            if (!IsLoggedInUserAdmin())
                return false;

            User? user = _userService.GetUserByUsername(username);
            if (user == null)
                return false;

            user.Unlock();
            return true;
        }
        //Checks that a bank account belongs to the given user by comparing the user ID of the account with the user ID of the user.
        public bool OwnsAccount(User? user, BankAccount? account)
        {
            if (user == null || account == null)
                return false;

            return account.GetUserId() == user.UserID;
        }

        public bool LoggedInUserOwns(BankAccount? account)
        {
            return OwnsAccount(LoggedInUser, account);
        }
    }
}

