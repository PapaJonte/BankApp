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

        //Behörighet
        public bool IsAdmin(User? user)
        {
            return user != null &&
                (user.Role == UserRole.Admin || user.Role == UserRole.Owner);
        }

        public bool IsLoggedInUserAdmin()
        {
            return IsAdmin(LoggedInUser);
        }

        //Bara Owner (bankägaren) får skapa nya Admin Konton. Förhindrar andra från att skapa fler admin-konto.
        public bool CanCreateAdminAccount()
        {
            return LoggedInUser != null && LoggedInUser.Role == UserRole.Owner;
        }


        //Låser upp ett låst konto.
        //Låser upp ett låst konto.
        public bool UnlockUser(string username)
        {
            User? caller = LoggedInUser;

            //Måste vara inloggad och ha admin- eller ownerrättigheter.
            if (caller == null || !IsAdmin(caller))
                return false;

            User? target = _userService.GetUserByUsername(username);
            if (target == null)
                return false;

            //Admin får bara låsa upp kunder. Owner får låsa upp Admin också.
            if (target.Role != UserRole.Customer && caller.Role != UserRole.Owner)
                return false;

            target.IsLockedOut = false;
            target.FailedLoginAttempts = 0;
            return true;
        }
    }
}

