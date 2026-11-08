using BankApp.GustafsClasses;
using BankApp.Transaction_Layer.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace BankApp
{
    /*
     Result from CreateUser
     Instead of only true/false the menu gets to know why it failed, 
    so that the correct error message can be displayed.
    */
    public enum CreateUserResult
    {
        Success,
        NotAllowed,
        InvalidPersonalNumber,
        UsernameTaken,
        InvalidPassword,
        InvalidEmail,
        InvalidPhone
    }

    internal class UserService
    {
        // All users are stored in memory in a dictionary, with the internal UserID as key.
        private readonly Dictionary<int, User> _users = new Dictionary<int, User>();
       
        // Gives every user a unique internal ID, starting from 1. The ID is never reused, even if a user is deleted.
        private int _nextUserId = 1;

        // Minimum password length. Used in validation and error messages.
        private const int MinPasswordLength = 6;

        /*
         Ther has to be an owner and admin from the beginning, or else no one can create new users (user story 2).
        */

        private readonly AccountManager _accountManager;
        public UserService(AccountManager accountManager)
        {
            _accountManager = accountManager;

            AddUser("00000000-0000", "ownerpassword123", "owner@gmail.com", "0729216167", UserRole.Owner);
            AddUser("11111111-1111", "adminpassword123", "admin@gmail.com", "0729216167", UserRole.Admin);

        }

        /*
         Method that creates a new user and adds it to the dictionary. It does not check permissions or validate input, so it is private.
        */
        private User AddUser(string username, string password, string email, string phoneNumber, UserRole role)
        {
            string normalizedUsername = NormalizeUsername(username);

            User newUser = new User(_nextUserId, normalizedUsername, password, email, phoneNumber, role);
            _users.Add(newUser.UserID, newUser);
            _nextUserId++;

            return newUser;
        }


        /*Removes all dashes and spaces from a username.
        This means "19900101-1234", "1990 0101 1234" and "199001011234" are all

        treated as the same person, so nobody can register twice by typing
        their personal number in a different format.
        Returns an empty string if the username is null, so callers never get a null back.*/
        private string NormalizeUsername(string username)
        {
            if (username == null)
                return string.Empty;

            return username.Replace("-", "").Replace(" ", "");
        }

        /* Formats a personal number for display (YYYYMMDD-XXXX).
         First normalizes the input. If the result is exactly 12 characters,
         a dash is inserted before the last 4 digits (at index 8).
         Otherwise the normalized string is returned without a dash.*/
        public string FormatPersonalNumber(string username)
        {
            string digits = NormalizeUsername(username);

            if (digits.Length != 12)
                return digits;

            return digits.Insert(8, "-");
        }

        /*
  Checks if a username is a valid personal number.
  Valid means: after removing dashes and spaces, it is exactly 12 characters
   and every character is a digit.
    Returns false if the username is null, empty or only whitespace.
*/
        public bool IsValidPersonalNumberFormat(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            string digitsOnly = NormalizeUsername(username);

            if (digitsOnly.Length != 12)
                return false;

            return digitsOnly.All(char.IsDigit);
        }

        /* Other validation methods used by both CreateUser and UpdateUserInfo.
        These are public so that the menu can check before calling the service, to avoid unnecessary error messages.*/
        public bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= MinPasswordLength;
        }


        /*Simple email check (not a full email standard check).
        Valid means:
        - there is at least one character before the @
        - there is a dot somewhere after the @, but not directly after it
        - the email does not end with a dot
        Example: "name@example.com" is valid, "name@.com" and "name@example." are not.
       */
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            int at = email.IndexOf('@');

            //Hsould be at least one character before the @, and there should be a dot after it (not directly after).
            return at > 0 && email.IndexOf('.', at) > at + 1 && !email.EndsWith(".");
        }

        /*
        Checks if a phone number is valid:
        - Spaces and dashes are removed first.
        - A leading "+" (country code, e.g. +46) is allowed and removed before checking.
        - The rest must be 7-15 digits.
       */
        public bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            string digits = phone.Replace(" ", "").Replace("-", "");

            if (digits.StartsWith("+"))
                digits = digits.Substring(1);

            return digits.Length >= 7 && digits.Length <= 15 && digits.All(char.IsDigit);
        }

        /* GET USERS
        Covers user story 1 (unique username) and 6 (finding the receiver of a transfer). 
         
        
        Finds a user by username (personal number).
         The input is normalized first, so it works with or without dashes/spaces.
         Returns null if no user is found.*/
        public User? GetUserByUsername(string username)
        {
            string normalizedUsername = NormalizeUsername(username);
            return _users.Values.FirstOrDefault(u => u.Username == normalizedUsername);
        }


        public User? GetUserById(int userId)
        {
            if (_users.TryGetValue(userId, out User? user))
                return user;

            return null;
        }

        //For admin-menu and statistic, returns a read-only list of all users. The menu can then display them in a table.
        public IReadOnlyList<User> GetAllUsers()
        {
            return _users.Values.ToList().AsReadOnly();
        }

        public bool UsernameExists(string username)
        {
            return GetUserByUsername(username) != null;
        }

        /* 
               CREATE USER
        Covers: user story 2 ("the administrator should be able to create new users").
 
         Creates a new user if the caller has permission and all input is valid.
 
         caller      = the user who is currently logged in (AuthService.LoggedInUser).
                       We pass in the actual user object instead of a bool like "isAdmin",
                       so UserService checks the role itself. With a bool, anyone could
                       just pass in "true".
         createdUser = the new user (or null if it failed).
         role        = the role of the new user. Defaults to Customer.
 
         Permission rules:
         - The caller must be logged in and be at least Admin (Customers cannot create users).
         - Nobody can create a new Owner.
         - Only the Owner can create a new Admin.
 
         The checks run in order and stop at the first error, and the returned
         CreateUserResult tells the menu exactly what went wrong.
        */
        public CreateUserResult CreateUser(User? caller, string username, string password,
            string email, string phoneNumber, out User? createdUser,
            UserRole role = UserRole.Customer)
        {
            createdUser = null;

            //Has to be logged in and at least admin.
            if (caller == null || caller.Role == UserRole.Customer)
                return CreateUserResult.NotAllowed;

            //No one can create an Owner account (only the first Owner was created in the constructor).
            if (role == UserRole.Owner)
                return CreateUserResult.NotAllowed;

            //Only owner can create Admin accounts. Admins can only create Customer accounts.
            if (role == UserRole.Admin && caller.Role != UserRole.Owner)
                return CreateUserResult.NotAllowed;

            if (!IsValidPersonalNumberFormat(username))
                return CreateUserResult.InvalidPersonalNumber;

            if (UsernameExists(username))
                return CreateUserResult.UsernameTaken;

            if (!IsValidPassword(password))
                return CreateUserResult.InvalidPassword;

            if (!IsValidEmail(email))
                return CreateUserResult.InvalidEmail;

            if (!IsValidPhone(phoneNumber))
                return CreateUserResult.InvalidPhone;

            createdUser = AddUser(username, password, email, phoneNumber, role);

            if (role == UserRole.Customer)
            {
                //Automatically create a bank account for the new customer user with 0 SEK.
                _accountManager.CreateAccount(createdUser.UserID, 0m, Currency.SEK);
            }

            return CreateUserResult.Success;
        }

        /*
        Translates a CreateUserResult into a readable message (in Swedish, since the app UI is Swedish).
         This way the menu only has to print GetErrorMessage(result), instead of
         writing its own switch on the enum in Menu.cs.
         It is static because it does not use any data from a specific UserService instance.
        */
        public static string GetErrorMessage(CreateUserResult result)
        {
            return result switch
            {
                CreateUserResult.Success => "User Created Successfully",
                CreateUserResult.NotAllowed => "You are not authorized to create this type of account.",
                CreateUserResult.InvalidPersonalNumber => "Your personal number has to be 12 numbers total (example: 199001011234).",
                CreateUserResult.UsernameTaken => "This username is already taken.",
                CreateUserResult.InvalidPassword => $"Password has to be at least {MinPasswordLength} numbers long.",
                CreateUserResult.InvalidEmail => "Email has to contain a @ and a dot after @ (example: name@exempel.com).",
                CreateUserResult.InvalidPhone => "Invalid phone number, please enter 7-15 digits.",
                _ => "Unknown error."
            };
        }

        /*Updates the email and phone number of a user.
          The user is passed in from AuthService.LoggedInUser, so a user can
          only change their OWN information.
          Returns false if no user is logged in, or if the new email or phone is invalid.
          Returns true if the update was saved. */
        public bool UpdateUserInfo(User? user, string newEmail, string newPhone)
        {
            if (user == null)
                return false;

            if (!IsValidEmail(newEmail) || !IsValidPhone(newPhone))
                return false;

            user.UpdateUserInfo(newEmail, newPhone);
            return true;
        }

        /*
         CHANGE PASSWORD (for example when an admin has given the user a start password)
         Changes the password of a user.
         - The user must be logged in (not null).
         - The old password must be correct, so nobody can change the password
           of an unlocked computer without knowing it.
         - The new password must pass the same rules as when creating a user.
         Returns true if the password was changed, otherwise false.
         */
        public bool ChangePassword(User? user, string oldPassword, string newPassword)
        {
            if (user == null)
                return false;

            if (!user.CheckPassword(oldPassword))
                return false;

            if (!IsValidPassword(newPassword))
                return false;

            user.ChangePassword(newPassword);
            return true;
        }
        // Sets a new password for a user, without needing the old password. Used by admins to reset passwords.
        // Admin can reset passwords for Customers, Owner can reset passwords for Admins.
        public bool ResetPassword(User? caller, string username, string newPassword)
        {
            if (caller == null || caller.Role == UserRole.Customer)
                return false;

            if (!IsValidPassword(newPassword))
                return false;

            User? target = GetUserByUsername(username);
            if (target == null || target.Role == UserRole.Owner)
                return false;

            if (target.Role == UserRole.Admin && caller.Role != UserRole.Owner)
                return false;

            target.ChangePassword(newPassword);
            target.Unlock();
            return true;
        }




    }
}
