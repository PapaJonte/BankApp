using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    // Result of a login attempt. Can be used in menu.
    public enum LoginResult
    {
        Success,
        WrongCredentials,
        LockedOut
    }

    internal class AuthService
    {
        private readonly UserService _userService;

        // Currently logged in user.
        public User? LoggedInUser { get; private set; }

        public AuthService(UserService userService)
        {
            _userService = userService;
        }

        public LoginResult Login(string username, string password)
        {
            User? user = _userService.GetUserByUsername(username);

            if (user == null)
                return LoginResult.WrongCredentials;

            if (user.IsLockedOut)
                return LoginResult.LockedOut;

            if (!user.CheckPassword(password))
            {
                user.RegisterFailedLogin();

                if (user.IsLockedOut)
                    return LoginResult.LockedOut;

                return LoginResult.WrongCredentials;
            }

            user.ResetFailedLogins();
            LoggedInUser = user;
            return LoginResult.Success;
        }

        public void Logout()
        {
            LoggedInUser = null;
        }

        /* BEHÖRIGHET
        Admin och Owner får göra "admin-saker" (t.ex. skapa användare,
        låsa upp konton) - IsAdmin/IsLoggedInUserAdmin.
        Owner har DÄRUTÖVER saker bara Owner får göra - IsOwner/IsLoggedInUserOwner. */

        public bool IsAdmin(User? user)
        {
            return user != null &&
                (user.Role == UserRole.Admin || user.Role == UserRole.Owner);
        }

        public bool IsLoggedInUserAdmin()
        {
            return IsAdmin(LoggedInUser);
        }

        // Owner är en EGEN roll, inte "extra admin" - en Admin är INTE Owner.
        public bool IsOwner(User? user)
        {
            return user != null && user.Role == UserRole.Owner;
        }

        public bool IsLoggedInUserOwner()
        {
            return IsOwner(LoggedInUser);
        }

        // Konkret skillnad enligt era user stories: bara Owner får skapa Admin-konton.
        // En vanlig Admin kan bara skapa Customer-konton (kontrolleras i UserService.CreateUser).
        public bool CanCreateAdminAccount()
        {
            return LoggedInUser != null && LoggedInUser.Role == UserRole.Owner;
        }

        // Unlocks a locked account.
        public bool UnlockUser(string username)
        {
            if (!IsLoggedInUserAdmin())
                return false;

            User? user = _userService.GetUserByUsername(username);
            if (user == null)
                return false;

            user.Unlock();
            return true;
        }

        // Checks that a bank account belongs to the given user.
        public bool OwnsAccount(User? user, BankAccount? account)
        {
            if (user == null || account == null)
                return false;

            return account.GetUserId() == user.UserID;
        }

        public bool LoggedInUserOwns(BankAccount? account)
        {
            return OwnsAccount(LoggedInUser, account);
        }
    }
}

