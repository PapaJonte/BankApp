using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    public enum UserRole
    {
        Customer,
        Admin,
        Owner
    }
    internal class User
    {
        //User info
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        //Role
        public UserRole Role { get; set; }

        //Login Security
        public int FailedLoginAttempts { get; set; }
        public bool IsLockedOut { get; set; }

        //Constructor
        public User(int userId, string username, string password,
                    string email, string phoneNumber, UserRole role = UserRole.Customer)

        {
            UserID = userId;
            Username = username;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            Role = role;

            //New users are not locked from the beginning.
            IsLockedOut = false;
            FailedLoginAttempts = 0;

        }

        //Registering failed logins.
        public void RegisterFailedLogin()
        {
            FailedLoginAttempts++;
            if (FailedLoginAttempts >= 3)
                IsLockedOut = true;
        }

        //Method for updating the user´s information.
        public void UpdateUserInfo(string newEmail, string NewPhone)
        {
            Email = newEmail;
            PhoneNumber = NewPhone;
        }


    }





}
