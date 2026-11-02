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

    internal class User
    {
        public const int MaxFailedLoginAttempts = 3;

        //Identity. Set only via constructor, never changeable.
        public int UserID { get; }
        public string Username { get; }
        public UserRole Role { get; }

        //Contact information, can be read freely, but only changed via UpdateUserInfo (user story 16).
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }

        /* Login security - can be read freely (menu needs to show if user is locked out),
        can only be changed via RegisterFailedLogin, ResetFailedLogins and Unlock (user story 3).*/
        public int FailedLoginAttempts { get; private set; }
        public bool IsLockedOut { get; private set; }

        // Password is never stored in clear text, only the hash and salt are stored.
        private byte[] _passwordHash;
        private byte[] _passwordSalt;

        // Failed login attempts are counted to lock out the user after 3 failed attempts.
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

        //Password

        private static byte[] HashPassword(string password, byte[] salt)
        {
            // New in .NET 7: Rfc2898DeriveBytes.Pbkdf2() - använder SHA256 och 100 000 iterationer.
            return Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        }

        // Jämför ett inskrivet lösenord mot den sparade hashen.
        // Lösenordet i klartext finns bara kvar i minnet under själva anropet.
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

        // Failed Login Attempts (user story 3)

        public void RegisterFailedLogin()
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= MaxFailedLoginAttempts)
                IsLockedOut = true;
        }

        public void ResetFailedLogins()
        {
            FailedLoginAttempts = 0;
        }

        // Used by admin via AuthService.UnlockUser.
        public void Unlock()
        {
            IsLockedOut = false;
            FailedLoginAttempts = 0;
        }

        /* User information (user story 16)
        Validation is handled by UserService before this method is called.*/

        public void UpdateUserInfo(string newEmail, string newPhone)
        {
            Email = newEmail;
            PhoneNumber = newPhone;
        }
    }
}


