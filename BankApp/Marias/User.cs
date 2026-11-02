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

        // Identitet - sätts bara i konstruktorn, kan aldrig ändras efteråt.
        public int UserID { get; }
        public string Username { get; }
        public UserRole Role { get; }

        // Kontaktuppgifter - går att läsa fritt, men bara ändra via UpdateUserInfo.
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }

        // Login security - går att läsa fritt (Robin behöver visa låsningsstatus),
        // men bara ändra via RegisterFailedLogin / ResetFailedLogins / Unlock.
        public int FailedLoginAttempts { get; private set; }
        public bool IsLockedOut { get; private set; }

        // Lösenordet lagras ALDRIG i klartext - bara som hash + salt.
        private byte[] _passwordHash;
        private byte[] _passwordSalt;

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

        // INLOGGNINGSSPÄRR (user story 3)

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

        // PERSONUPPGIFTER (user story 16)
        // Validering sköts av UserService innan den här anropas.

        public void UpdateUserInfo(string newEmail, string newPhone)
        {
            Email = newEmail;
            PhoneNumber = newPhone;
        }
    }
}

