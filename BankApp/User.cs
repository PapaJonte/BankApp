using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    internal class User
    {
        public int UserID { get; set; }
        public int AdminID { get; set; }
        public int OwnerID { get; set; }

        /*
         // User Info
        public int UserID { get; set; } 
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } 

        //Login Security

        public int FailedLoginAttempts { get; set; }
        public bool IsLockedOut { get; set; }

        //konstruktor
         public User(int userId, string username, string password,
                    string email, string phoneNumber, bool isAdmin = false)
        {
            UserID = userId;
            Username = username;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            IsAdmin = isAdmin;
       // (new users don´t get locked out, only existing users can be locked out)
            IsLockedOut = false;
            FailedLoginAttempts = 0;
        }

        //ska jag lägga till? En hjälpmetod för att registrera ett misslyckat försök
public void RegisterFailedLogin()
{
    FailedLoginAttempts++;
    if (FailedLoginAttempts >= 3)
        IsLockedOut = true;
}
*/

    }
}