using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;

namespace BankApp
{

   internal class UserService
    {
        //alla användare sparas i en lista i minnet.
        private readonly List<User> _users = new List<User>();
        //Ger varje ny användare ett unikt internt ID (1, 2, 3 ...).
        private int _nextUserId = 1;

        //Den som är inloggad just nu (null = ingen är inloggad). Resten av gruppen använder denna för att veta VEM som gör något
        public User? LoggedInUser { get; private set; }

        //Det måste finnas minst en admin från start, skapar med testvärden för den enda admin-användaren som skapas automatiskt när programmet startar.
        public UserService()
        {
            AddUser("000000-0000", "adminpassword123", "admin@gmail.com", "0729216167", UserRole.Admin);
        }

        //Skapa användare
        public bool UsernameExists(string username)
        {
            return GetUserByUsername(username) != null;
        }

        public bool IsValidPersonalNumberFormat (string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            string digitsOnly = username.Replace("-", ""); // tar bort - för att räkna hur många siffror totalt

            //Giltiga längder
            if (digitsOnly.Length != 10 && digitsOnly.Length != 12)
                return false;

            return digitsOnly.All(char.IsDigit);
        }

        }
       

    }

