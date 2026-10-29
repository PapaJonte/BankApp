using System.Collections.Generic;
using System.Linq;

namespace BankApp
{

    internal class UserService
    {
        //All users are stored in a dictionary, with UserID as the key.
        private readonly Dictionary<int, User> _users =
             new Dictionary<int, User>();
        //Gives every user a unique internal ID (1, 2, 3 ...).
        private int _nextUserId = 1;

        //There should be at least one admin and owner from the beginning, skapar med testvärden för den enda admin-användaren som skapas automatiskt när programmet startar.
        public UserService()
        {
            AddUser("00000000-0000", "ownerpassword123", "owner@gmail.com", "0729216167", UserRole.Owner);
            AddUser("11111111-1111", "adminpassword123", "admin@gmail.com", "0729216167", UserRole.Admin);
        }

        //Privat hjälpmetod som skapar och lagrar en ny User. Används av konstruktorn (default-admin) och av CreateUser.
        private void AddUser(string username, string password, string email, string phoneNumber, UserRole role)
        {
            string normalizedUsername = NormalizeUsername(username);

            User newUser = new User(_nextUserId, normalizedUsername, password, email, phoneNumber, role);
            _users.Add(newUser.UserID, newUser);
            _nextUserId++;
        }

        /*
         NORMALISERA ANVÄNDARNAMN (personnummer)
         Tar bort bindestreck och mellanslag så att "900101-1234",
         "9001011234" och liknande skrivsätt alltid blir samma text
         internt. Används varje gång vi sparar eller letar efter en
         användare, så att samma person inte kan registreras flera
         gånger med olika skrivsätt.
        */
        private string NormalizeUsername(string username)
        {
            if (username == null)
                return string.Empty;

            return username.Replace("-", "").Replace(" ", "");
        }

        //Formats "199001011234" as "19900101-1234" for display in menus.
        public string FormatPersonalNumber(string username)
        {
            string digits = NormalizeUsername(username);

            if (digits.Length != 12)
                return digits;

            return digits.Insert(8, "-");
        }

        /* Hämta användare.
         Uppfyller: user story 1 (unikt användarnamn)
            */
        public User? GetUserByUsername(string username)
        {
            string normalizedUsername = NormalizeUsername(username);
            return _users.Values.FirstOrDefault(u => u.Username == normalizedUsername);
        }

        //Using TryGetValue
        public User? GetUserById(int userId)
        {
            if (_users.TryGetValue(userId, out User? user))
            {
                return user;
            }

            return null;
        }

        //
        public IReadOnlyList<User> GetAllUsers()
        {
            return _users.Values.ToList().AsReadOnly();
        }


        //Validering. Kollar om personnummret redan finns registrerade bland användare.
        public bool UsernameExists(string username)
        {
            return GetUserByUsername(username) != null;
        }

        //Formatkontroll av personnumret: inte tomt, exakt 12 siffror och bara siffror inga bokstäver.
        public bool IsValidPersonalNumberFormat(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            string digitsOnly = NormalizeUsername(username);

            if (digitsOnly.Length != 12)
                return false;

            return digitsOnly.All(char.IsDigit);
        }

        /*SKAPA ANVÄNDARE
       Uppfyller: user story 2 ("administratören ska kunna skapa
       nya användare i systemet"). callerIsAdmin skickas in utifrån
         (från AuthService), eftersom UserService inte vet vem som
         är inloggad.
        */

        public bool CreateUser(bool callerIsAdmin, bool callerIsOwner, string username,
            string password, string email, string phoneNumber,
            UserRole role = UserRole.Customer)
        {
            // Måste vara minst admin för att skapa någon användare
            if (!callerIsAdmin)
                return false;

            // No one can create a new Owner.
            if (role == UserRole.Owner)
                return false;

            // Only onwer can create Admin.
            if (role == UserRole.Admin && !callerIsOwner)
                return false;

            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (!IsValidPersonalNumberFormat(username))
                return false;

            if (UsernameExists(username))
                return false;

            AddUser(username, password, email, phoneNumber, role);
            return true;
        }

        /*
          UPPDATERA PERSONUPPGIFTER
         Uppfyller: user story 16, extra ("uppdatera sina personliga
         uppgifter, t.ex. e-postadress, telefonnummer").
         user skickas in direkt (från AuthService.LoggedInUser i Main),
         eftersom UserService inte vet vem som är inloggad.
        */
        public bool UpdateUserInfo(User user, string newEmail, string newPhone)
        {
            if (user == null)
                return false;

            if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains("@"))
                return false;

            if (string.IsNullOrWhiteSpace(newPhone))
                return false;

            user.UpdateUserInfo(newEmail, newPhone);
            return true;
        }

    }

}