using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    //Resultatet av ett inloggningsförsök, kan användas i menyn.
    public enum LoginResult
    {
        Success,
        WrongCredentials,
        LockedOut
    }
    internal class AuthService
    {
        private readonly UserService _userService;

        //Den som är inloggad just nu.
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

            if (user.Password != password)
            {
                user.RegisterFailedLogin();

                if (user.IsLockedOut)
                    return LoginResult.LockedOut;

                return LoginResult.WrongCredentials; //fixar bugg.
            }

            user.FailedLoginAttempts = 0;
            LoggedInUser = user;
            return LoginResult.Success;
        }
        public void Logout()
        {
            LoggedInUser = null;
        }

        //Autherisation
        public bool IsAdmin(User? user)
        {
            return user != null &&
                (user.Role == UserRole.Admin || user.Role == UserRole.Owner);
        }

        public bool IsLoggedInUserAdmin()
        {
            return IsAdmin(LoggedInUser);
        }

        //Only owner can create new Admin accounts.
        public bool CanCreateAdminAccount()
        {
            return LoggedInUser != null && LoggedInUser.Role == UserRole.Owner;
        }
        //Unlocks a locked account.
        public bool UnlockUser(string username)
        {
            if (!IsLoggedInUserAdmin()) //Confirms that the person that gets to unlock is an admin.
                return false;

            User? user = _userService.GetUserByUsername(username); //Finds the user that needs to be unlocked.
            if (user == null)
                return false;

            user.IsLockedOut = false; //Unlocking.
            user.FailedLoginAttempts = 0; //resets the timer.
            return true;
        }
    }
}



