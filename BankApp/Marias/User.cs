using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    public enum UserRole
    {
        Customer,
        Admin
    }
    internal class User
    {
        //User info
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        //Roll
        public UserRole Role { get; set; }

        //Login Security
        public int FailedLoginAttempts { get; set; }
        public bool IsLockedOut { get; set; }

        //Konstruktor
        public User(int userId, string username, string password,
                    string email, string phoneNumber, UserRole role = UserRole.Customer)

        {
            UserID = userId;
            Username = username;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            Role = role;

            // Nya användare är inte låsta från början
            IsLockedOut = false;
            FailedLoginAttempts = 0;

        }

        // Extra uppgift, uppdatera personuppgifter.
        public void RegisterFailedLogin()
        {
            FailedLoginAttempts++;
            if (FailedLoginAttempts >= 3)
                IsLockedOut = true;
        }


    }


     

    
}