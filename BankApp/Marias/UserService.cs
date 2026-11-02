using System.Collections.Generic;
using System.Linq;

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
        // Alla användare lagras i en dictionary med UserID som nyckel.
        private readonly Dictionary<int, User> _users = new Dictionary<int, User>();

        // Ger varje användare ett unikt internt ID (1, 2, 3 ...).
        private int _nextUserId = 1;

        // Minsta längd på lösenord.
        private const int MinPasswordLength = 6;

        /*
         Det måste finnas en Owner och en Admin från start,
         annars kan ingen skapa nya användare (user story 2).
        */
        public UserService()
        {
            AddUser("00000000-0000", "ownerpassword123", "owner@gmail.com", "0729216167", UserRole.Owner);
            AddUser("11111111-1111", "adminpassword123", "admin@gmail.com", "0729216167", UserRole.Admin);
        }

        /*
         Privat hjälpmetod som skapar och lagrar en ny User.
         Returnerar den skapade användaren så att t.ex. ett bankkonto
         direkt kan kopplas till den (Gustaf, user story 2 och 7).
        */
        private User AddUser(string username, string password, string email, string phoneNumber, UserRole role)
        {
            string normalizedUsername = NormalizeUsername(username);

            User newUser = new User(_nextUserId, normalizedUsername, password, email, phoneNumber, role);
            _users.Add(newUser.UserID, newUser);
            _nextUserId++;

            return newUser;
        }
 
        // PERSONNUMMER (användarnamn)

        /*
         Tar bort bindestreck och mellanslag så att "19900101-1234" och
         "199001011234" alltid blir samma text internt. Används varje gång
         vi sparar eller letar efter en användare, så att samma person inte
         kan registreras flera gånger med olika skrivsätt.
        */
        private string NormalizeUsername(string username)
        {
            if (username == null)
                return string.Empty;

            return username.Replace("-", "").Replace(" ", "");
        }

        // Formaterar "199001011234" som "19900101-1234" för visning i menyer.
        public string FormatPersonalNumber(string username)
        {
            string digits = NormalizeUsername(username);

            if (digits.Length != 12)
                return digits;

            return digits.Insert(8, "-");
        }

        // Formatkontroll: inte tomt, exakt 12 tecken och bara siffror.
        public bool IsValidPersonalNumberFormat(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            string digitsOnly = NormalizeUsername(username);

            if (digitsOnly.Length != 12)
                return false;

            return digitsOnly.All(char.IsDigit);
        }

        // ÖVRIG VALIDERING (återanvänds av både CreateUser och UpdateUserInfo)

        public bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= MinPasswordLength;
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            int at = email.IndexOf('@');

            // Måste ha något före @, och en punkt någonstans efter @.
            return at > 0 && email.IndexOf('.', at) > at + 1 && !email.EndsWith(".");
        }

        public bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            string digits = phone.Replace(" ", "").Replace("-", "");

            if (digits.StartsWith("+"))
                digits = digits.Substring(1);

            return digits.Length >= 7 && digits.Length <= 15 && digits.All(char.IsDigit);
        }

        // HÄMTA ANVÄNDARE
        // Uppfyller: user story 1 (unikt användarnamn), 6 (hitta mottagare)

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

        // För admin-menyn och statistik (user story 17).
        public IReadOnlyList<User> GetAllUsers()
        {
            return _users.Values.ToList().AsReadOnly();
        }

        public bool UsernameExists(string username)
        {
            return GetUserByUsername(username) != null;
        }

        /* ---------------------------------------------------------------
         SKAPA ANVÄNDARE
        Uppfyller: user story 2 ("administratören ska kunna skapa nya
        användare i systemet").
        caller = den som är inloggad (AuthService.LoggedInUser).
        Vi skickar in själva användaren istället för bool-värden, så att
        UserService själv kontrollerar rollen. Med bool kunde vem som helst
        bara skicka in "true".
        createdUser = den nya användaren, så att Gustaf direkt kan
        öppna ett konto åt kunden. */

        public CreateUserResult CreateUser(User? caller, string username, string password,
            string email, string phoneNumber, out User? createdUser,
            UserRole role = UserRole.Customer)
        {
            createdUser = null;

            // Måste vara inloggad och minst Admin.
            if (caller == null || caller.Role == UserRole.Customer)
                return CreateUserResult.NotAllowed;

            // Ingen kan skapa en ny Owner.
            if (role == UserRole.Owner)
                return CreateUserResult.NotAllowed;

            // Bara Owner kan skapa Admin.
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
            return CreateUserResult.Success;
        }

        /*
         Översätter CreateUserResult till en läsbar svensk text, så att
        menyn bara behöver skriva ut GetErrorMessage(result) istället
         för att göra en egen switch på enumen i Menu.cs.
        */
        public static string GetErrorMessage(CreateUserResult result)
        {
            return result switch
            {
                CreateUserResult.Success => "Användaren skapades!",
                CreateUserResult.NotAllowed => "Du har inte behörighet att skapa den här typen av konto.",
                CreateUserResult.InvalidPersonalNumber => "Personnumret måste bestå av exakt 12 siffror (t.ex. 199001011234).",
                CreateUserResult.UsernameTaken => "Det här personnumret är redan registrerat.",
                CreateUserResult.InvalidPassword => $"Lösenordet måste vara minst {MinPasswordLength} tecken.",
                CreateUserResult.InvalidEmail => "Ogiltig e-postadress - måste innehålla @ och en punkt efter @ (t.ex. namn@exempel.com).",
                CreateUserResult.InvalidPhone => "Ogiltigt telefonnummer - ange 7-15 siffror.",
                _ => "Okänt fel."
            };
        }

        // UPPDATERA PERSONUPPGIFTER
        // Uppfyller: user story 16 (extra).
        // user skickas in från AuthService.LoggedInUser, så en användare
        // kan bara ändra sina EGNA uppgifter.

        public bool UpdateUserInfo(User? user, string newEmail, string newPhone)
        {
            if (user == null)
                return false;

            if (!IsValidEmail(newEmail) || !IsValidPhone(newPhone))
                return false;

            user.UpdateUserInfo(newEmail, newPhone);
            return true;
        }

        // BYTA LÖSENORD (t.ex. när admin satt ett startlösenord)
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
    }
}
