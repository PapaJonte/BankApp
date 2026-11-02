using System;
using System.Collections.Generic;
using System.Text;
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
        //User info
        public int UserID { get; }
        public string Username { get; }
        //Role
        public UserRole Role { get; }

        public string Password { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }

        //Login Security
        public int FailedLoginAttempts { get; private set; }
        public bool IsLockedOut { get; private set; }

        //Password hashing and salting for security.
        private byte[] _passwordHash;
        private byte[] _passwordSalt;

        //Constructor
        public User(int userId, string username, string password,
                    string email, string phoneNumber, UserRole role = UserRole.Customer)

        {
            UserID = userId;
            Username = username;
            Email = email;
            PhoneNumber = phoneNumber;
            Role = role;


            //Hashing and salting the password for security.
            _passwordSalt = RandomNumberGenerator.GetBytes(16);
            _passwordHash = HashPassword(password, _passwordSalt);

            //New users are not locked from the beginning.
            IsLockedOut = false;
            FailedLoginAttempts = 0;

        }

        //Method for hashing the password with salt.
        private static byte[] HashPassword(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
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

        // Används av admin via AuthService.UnlockUser.
        public void Unlock()
        {
            IsLockedOut = false;
            FailedLoginAttempts = 0;
        }

        // ---------------------------------------------------------------
        // PERSONUPPGIFTER (user story 16)
        // Validering sköts av UserService innan den här anropas.
        // ---------------------------------------------------------------

        public void UpdateUserInfo(string newEmail, string newPhone)
        {
            Email = newEmail;
            PhoneNumber = newPhone;
        }


    }

}
