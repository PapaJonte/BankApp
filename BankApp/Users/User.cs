using System;
using System.Security.Cryptography;

namespace BankApp
{
    public enum UserRole
    {
        Customer,
        Admin,
        Owner
    }
    /*
      The class protects its own data: most properties have a private setter,
     so they can only be changed through the methods in this class.
      */
    internal class User
    {
        //Number of failed login attempts allowed before the account is locked (user story 3).
        public const int MaxFailedLoginAttempts = 3;

        //Internal unique ID, given by UserService. Used to link bank accounts to the user
        public int UserID { get; }
        public string Username { get; }
        public UserRole Role { get; }

        //Contact information,can be read by anyone, but only changed via UpdateUserInfo (user story 16).
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }

        /* Login security - can be read freely (menu needs to show if user is locked out),
        can only be changed via RegisterFailedLogin, ResetFailedLogins and Unlock (user story 3).*/
        public int FailedLoginAttempts { get; private set; }
        public bool IsLockedOut { get; private set; }

        // Password is never stored in clear text, only the hash and salt are stored.
        private byte[] _passwordHash;
        //// 16 random bytes, unique for each user. Makes sure two users with the same password still get different hashes.
        private byte[] _passwordSalt;

       //Constructor. Creates a new user.
        public User(int userId, string username, string password,
                    string email, string phoneNumber, UserRole role = UserRole.Customer)
        {
            UserID = userId;
            Username = username;
            Email = email;
            PhoneNumber = phoneNumber;
            Role = role;

            _passwordSalt = RandomNumberGenerator.GetBytes(16);
            _passwordHash = HashPassword(password, _passwordSalt);

            IsLockedOut = false;
            FailedLoginAttempts = 0;
        }

        /*
        Turns a password and a salt into a 32-byte hash using PBKDF2
        with SHA256 and 100,000 iterations.
       */
        private static byte[] HashPassword(string password, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        }

        /*
        Checks if an entered password is correct.
        The entered password is hashed with the user's stored salt, and the
        result is compared with the stored hash. If they match, the password is correct.
       */
        public bool CheckPassword(string password)
        {
            byte[] attemptedHash = HashPassword(password, _passwordSalt);
            return CryptographicOperations.FixedTimeEquals(attemptedHash, _passwordHash);
        }
        public void ChangePassword(string newPassword)
        {
            _passwordSalt = RandomNumberGenerator.GetBytes(16);
            _passwordHash = HashPassword(newPassword, _passwordSalt);
        }


        /*
         Called by AuthService when the wrong password is entered.
         Adds one to the counter, and locks the account when the counter
         reaches MaxFailedLoginAttempts (3).
        */
        public void RegisterFailedLogin()
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= MaxFailedLoginAttempts)
                IsLockedOut = true;
        }

        /*
       Called by AuthService after a successful login.
       Resets the counter, so only failed attempts IN A ROW lead to a lockout.
      */
        public void ResetFailedLogins()
        {
            FailedLoginAttempts = 0;
        }

        /*
        Unlocks a locked account and resets the failed-attempt counter.
        Called by an Admin or the Owner via AuthService.UnlockUser,
        which checks the permission before calling this method.
       */
        public void Unlock()
        {
            IsLockedOut = false;
            FailedLoginAttempts = 0;
        }

        /*
        Updates the user's email and phone number.
        This method does not validate anything. UserService.UpdateUserInfo
        checks the format of the email and phone number before calling it.
       */
        public void UpdateUserInfo(string newEmail, string newPhone)
        {
            Email = newEmail;
            PhoneNumber = newPhone;
        }
    }
}


