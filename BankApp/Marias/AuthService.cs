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


    }
}
}
