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

    }
}
