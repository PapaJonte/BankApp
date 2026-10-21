using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Marias
{
    public enum LoginResult
    {
        Success,
        WrongCredentials,
        LockedOut
    }

   internal class UserService
    {
        //alla användare sparas i en lista i minnet.
        private readonly List<User> _users = new List<User>();

        //Ger varje ny användare ett unikt internt ID (1, 2, 3 ...).
        private int _nextUserId = 1;

        //Den som är inloggad just nu (null = ingen är inloggad). Resten av gruppen använder denna för att veta VEM som gör något
        public User? LoggedInUser { get; private set; }
    }
}
