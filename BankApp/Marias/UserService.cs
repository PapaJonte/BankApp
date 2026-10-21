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

        //Det måste finnas minst en admin från start, skapar med testvärden för den enda admin-användaren som skapas automatiskt när programmet startar.
        public UserService()
        {
            AddUser("000000-0000", "adminpassword123", "admin@gmail.com", "0729216167", UserRole.Admin);
        }

        private void AddUser (string username, string password, string email, string PhoneNumber, UserRole role)
        {
            User newUser = new User(_nextUserId, username, password, email, phoneNumber, role);
            _users.Add(newUser);
            _nextUserId++;
        }


        }
       

    }

