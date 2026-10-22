using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;

namespace BankApp
{

   internal class UserService
    {
        //alla användare sparas i en lista i minnet.
        private readonly List<User> _users = new List<User>();
        //Ger varje ny användare ett unikt internt ID (1, 2, 3 ...).
        private int _nextUserId = 1;

        //Det måste finnas minst en admin från start, skapar med testvärden för den enda admin-användaren som skapas automatiskt när programmet startar.
        public UserService()
        {
            AddUser("000000-0000", "adminpassword123", "admin@gmail.com", "0729216167", UserRole.Admin);
        }

        //Privat hjälpmetod som skapar och lagrar en ny User. Används av konstruktorn (default-admin) och av CreateUser.
        private void AddUser (string username, string password, string email, string phoneNumber, UserRole role)
        {
            User newUser = new User(_nextUserId, username, password, email, phoneNumber, role);
            _users.Add(newUser);
            _nextUserId++;
        }

        /* Hämta användare.
         Uppfyller: user story 1 (unikt användarnamn), används av
         Gustaf/Alex/Jonathan för att koppla konton/lån till rätt
        UserID, och av Robin i menyerna.
            */
        public User? GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public User? GetUserById(int userId)
        {
            return _users.FirstOrDefault(u => u.UserID == userId);
        }

        //Readonly så att ingen utanför kan lägga till/ ta bort direkt i listan.
        public IReadOnlyList<User> GetAllUsers()
        {
            return _users.AsReadOnly();
        }

        //Validering. Kollar om personnummret redan finns registrerade bland användare.
        public bool UsernameExists(string username)
        {
            return GetUserByUsername(username) != null;
        }

        //Formatkontroll av personnumret: inte tomt, rätt antal siffror (10 eller 12) och bara siffror inga bokstäver.
        public bool IsValidPersonalNumberFormat(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;
            string digitsOnly = username.Replace("-", "");

            if (digitsOnly.Length != 10 && digitsOnly.Length != 12)
                return false;
            return digitsOnly.All(char.IsDigit);
        }

        /*SKAPA ANVÄNDARE
       Uppfyller: user story 2 ("administratören ska kunna skapa
       nya användare i systemet"). callerIsAdmin skickas in utifrån
         (från AuthService), eftersom UserService inte vet vem som
         är inloggad.
        */

        //returnerar true om användaren skapades, annars false.
        public bool CreateUser(bool callerIsAdmin, string username, string password, string email, string phoneNumber, UserRole role = UserRole.Customer)
        {
            //bara admin får skapa nya användare.
            if (!callerIsAdmin)
                return false;

            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (!IsValidPersonalNumberFormat(username))
                return false;
            //Personumret måste vara unikt (vilket det är, kanske kommer justera? men ville uppfylla krav)
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

        //Kollar om personummer + lösenord stämmer och hanterar misslycakde försök.
        public LoginResult ValidateCredentials(string username, string password)
        {
            User? user = GetUserByUsername(username);

            if (user == null)
                return LoginResult.WrongCredentials;

            if (user.IsLockedOut)
                return LoginResult.LockedOut;

            if (user.Password != password)
            {
                user.RegisterFailedLogin();

                if (user.IsLockedOut)
                    return LoginResult.LockedOut;

                return LoginResult.WrongCredentials;

            }

            user.FailedLoginAttempts = 0;
            return LoginResult.Success;


        }



        }
       

    }

