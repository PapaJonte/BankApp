using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApp
{

    internal class UserService
    {
        //alla användare sparas i en lista i minnet.
        private readonly Dictionary<int, User> _users =
             new Dictionary<int, User>();
        //Ger varje ny användare ett unikt internt ID (1, 2, 3 ...).
        private int _nextUserId = 1;

        //Det måste finnas minst en admin från start, skapar med testvärden för den enda admin-användaren som skapas automatiskt när programmet startar.
        public UserService()
        {
            AddUser("000000-0000", "adminpassword123", "admin@gmail.com", "0729216167", UserRole.Owner);
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

        /* Hämta användare.
         Uppfyller: user story 1 (unikt användarnamn)
            */
        public User? GetUserByUsername(string username)
        {
            string normalizedUsername = NormalizeUsername(username);
            return _users.Values.FirstOrDefault(u => NormalizeUsername(u.Username) == normalizedUsername);
        }

        //Använder TryGetValue
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

        //Formatkontroll av personnumret: inte tomt, exakt 10 siffror och bara siffror inga bokstäver.
        public bool IsValidPersonalNumberFormat(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            string digitsOnly = NormalizeUsername(username);

            if (digitsOnly.Length != 10)
                return false;

            return digitsOnly.All(char.IsDigit);
        }

        /*SKAPA ANVÄNDARE
       Uppfyller: user story 2 ("administratören ska kunna skapa
       nya användare i systemet"). callerIsAdmin skickas in utifrån
         (från AuthService), eftersom UserService inte vet vem som
         är inloggad.
        */

        public bool CreateUser(
            User? caller,
            string username,
            string password,
            string email,
            string phoneNumber,
            UserRole role = UserRole.Customer)
        {
            // Det måste finnas en inloggad användare.
            if (caller == null)
                return false;

            // Endast Admin och Owner får skapa kundkonton.
            if (caller.Role != UserRole.Admin &&
                caller.Role != UserRole.Owner)
                return false;

            // Endast Owner får skapa Admin-konton.
            if (role == UserRole.Admin &&
                caller.Role != UserRole.Owner)
                return false;

            // Owner-konton skapas inte via denna metod.
            if (role == UserRole.Owner)
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
        public bool UpdateUserInfo(User? user, string newEmail, string newPhone)
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

        // Gäller Admin och Owner. Alla använder denna för adminkontroller.
        public static bool HasAdminRights(User? user) =>
            user != null && (user.Role == UserRole.Admin || user.Role == UserRole.Owner);
    }

}