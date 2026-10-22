using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    public enum LoginResult
    {
        Success,
        WrongCredentials,
        LockedOut
    }
    internal class AuthService
    {
        private readonly UserService _userService;
        public User? LoggedInUser { get; private set; }

        public AuthService(UserService userService)
        {
            _userService = userService;
        }


        //Kollar om personummer + lösenord stämmer och hanterar misslycakde försök.
        public LoginResult ValidateCredentials(string username, string password)
        {
            User? user = GetUserByUsername(username);

            if (user == null)
                return LoginResult.WrongCredentials;

            if (user.IsLockedOut)
                return LoginResult.LockedOut;

            if (user.Password != password)
            {
                user.RegisterFailedLogin();

                if (user.IsLockedOut)
                    return LoginResult.LockedOut;

                return LoginResult.WrongCredentials;

            }

            user.FailedLoginAttempts = 0;
            return LoginResult.Success;


        }

    }
}
}
