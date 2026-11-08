# BankApp
User, Authentication and Permissions:
This part of BankApp handles users, login and roles. It lives in the "Users" folder and consists of three classes.
-User: Holds a user´s data. The password is never stored in clear text, only as a hash and a salt.
-UserService: Creates and finds users, validates input and changes passwords.
-AuthService: Handles login, logout, account lockout and permission checks.

Roles:
-Customer: Use their own account and update their own contact information.
-Admin: Creates customers, unlocks accounts and resets customer´s passwords if they happen to get locked out.
-Owner: Everything an Admin can do, plus create Admin accounts and reset Admins' passwords. 

Permissions are enforced in `UserService` and `AuthService`. Viewing bank-wide
statistics is implemented in the Transactions layer (`BankTransferStatistic`),
not in the user classes. 
**Login and lockout**
- After 3 failed attempts in a row, the account is locked.
- A successful login resets the counter.
- Only an Admin or Owner can unlock a locked account.

New users:
- Permissions are checked in `UserService`, not only in the menu, so the rules hold even if the menu forgets to check.
- Every new customer automatically gets a SEK account with a balance of 0.
Example:
var userService = new UserService(accountManager);
var auth = new AuthService(userService);
LoginResult result = auth.Login("199001011234", "myPassword");
if (result == LoginResult.Success && auth.IsLoggedInUserAdmin())
{
    CreateUserResult res = userService.CreateUser(
        auth.LoggedInUser, "198505051234", "password123",
        "anna@example.com", "0701234567", out User? created);
    if (res != CreateUserResult.Success)
        Console.WriteLine(UserService.GetErrorMessage(res));
}

Result values:
- `LoginResult`: `Success`, `WrongCredentials`, `LockedOut`
- `CreateUserResult`: `Success`, `NotAllowed`, `InvalidPersonalNumber`, `UsernameTaken`, `InvalidPassword`, `InvalidEmail`, `InvalidPhone`

Known limitations:
- Users are stored in memory only and are lost when the program closes.
- The Owner and Admin are created at startup with built-in default passwords. In a real system these should come from configuration or be changed on first login.
- Validation of email and phone numbers is basic.
- Permission rules are repeated in several methods and could be gathered in one place.
- Some methods return only `true` or `false`, so the menu cannot tell why they failed.



---Accounts---

--SavingsAccount--
Not to be confused with BankAccount!
SavingsAccount as a class serves the purpose primarily of storing the balance of accounts. Because this class is intended to have many almost completely identical variations in it's objects there is a constructor provided for assigning values to it's properties.

The properties include "Balance", "Interest", and "SavingsAccountNumber". Savings account number exists for the purpose of identifying such accounts later when transactions must be made, for there is supposed to be a multitude of savings accounts. Meanwhile Balance, and Interest serve a more obvious purpose.

TransferBalance:
Unlike other methods of the class this one does not exist for the purpose of returning private value of the class. TransferBalance() either raises or lowers balance of an account depending on the input values. The bool value in the parameter determines whether it is a deposit that shall be made, or a withdrawal. The other input value simply determines how much the transfer sum happens to be. This method does not actually transfer values however, for there are other methods inside of the Transaction Layer folder that manages that job.

GetBalance, GetCurrency, GetSavingsAccountNumber:
These methods all return a value, and are named after the variable in their name.

--BankAccount--
Not to be confused with SavingsAccount, though that may pherhaps easily be done, for BankAccount has many properties, variables, and methods that are either identical, or somewhat similar. For instance BankAccount, just like SavingsAccount has balance, currency, a number variable to identify the specific object that is somewhat similar to what SavingsAccount has, and additionally methods to return these values.
The BankAccount serves partly to contain SavingsAccounts. Hence why it has both a list dedicated to storing objects of the SavingsAccount class, and a dictionary for storing the list.
Additionally it's more closely associated with the user ID in order to establish ownership. The userId variable that represents the ID of the user, and the bank account number may both be returned with a method the same way as values in the SavingsAccount class.

GetThresholdNotification:
This method returns the threshold number once a transaction happens to be above the maximum threshold for deposits, and withdrawals. The method assists transaction based systems by providing this information.

TransferBalance:
Same as in SavingsAccount above.

GetInterest, GetUserId:
Returns interest and user ID, just like the names imply. Very similar to other Methods that follow the naming convention of having "Get" in the name.

GetBalance, GetCurrency, GetBankAccountNumber:
Same as in SavingsAccount above.

--AccountManager--
This class has various different methods for handling the two other Account classes. Here is a rundown of the included methods for manipulating the accounts:

CheckCurrency:
This method allows the account user on the bank to give an input to choose currency for an account.

AccountNumberGenerator:
This method generates a unique number to identify individual accounts that are created.

CreateAccount:
Not to be confused with CreateSavingsAccount. This method creates an object of the BankAccount class.

CreateSavingsAccount:
Not to be confused with the CreateAccount class. This method creates an object of the SavingsAccount class.

GetAccounts:
This method prints out balance and currency of the various accounts, both Bank accounts and savings accounts, of a user into the Console. 

ConvertBalanceToSek
This method converts currencies to SEK. This serves the GetTotalBalance method below.

GetTotalBalance
This method calculates the total balance of the various accounts, both savings accounts and bank accounts, that are owned by the same user. This method is necessary for assisting with judging maximum limits for loans outside of this class, and folder.

FindRegularAccount
This method finds a specific object of the Bank Account class.

AccountOwnershipVerification
This method serves to verify if a user truly owns an account.

---TRANSACTION LAYER---

--API--

BankTransfer:
Entry point for accessing transfer functionality. Handles service calls for transfering, deposit and withdrawing money.

BankTransferStatistic:
Entry point for accessing transfer statistics. Handles service calls for fetching pre-filtered transfer data.

ExchangeRate:
Entry point for accessing exchange rate functionality. Handles service calls for updating exchange rate, fetching exchange rate and calculating exchange rate.

--Containers--

ExchangeRateRegistry:
Registry class that holds ssot for current exchange rates. 

TransactionRegistry:
Registry class that holds current staged transactions waiting to be processed as well as history of successful transactions.

--Controller--

AccountBridge:
Bridge controller between transaction services and the bank account layer. Responsible for deposit and withdrawal calls between transaction service and bank account. Also fetch bank accounts from account manager.

BankTransaction:
Controller between API and StageTransaction. Responsible for calling for withdrawal and depositions as well as setting up transfers that will be staged.

TransactionStatisticBridge:
Bridge controller between BankTransferStatistic and the view menus. Returning pre-filtered statistic data to view.

--DataModel--

AccountPopularityCount:
Data model that stores ordered list of populat bank accounts by userID and the count (sum of transactions).

AccountPopularityVolume:
Data model that stores ordered list of popular bank accounts by userID and the volume (sum of capital).

TotalCount:
Data model containing total count (sum of transactions) for a specific account. 

TotalTransactionAmount:
Data model containing the total transactional amounts for specific userID.

TotalTransactionsDateOrdered:
Data model containing dictionary for transaction dates per account per user. Can be retireved ordered ascending or descending.

TotalVolume:
Data model containing total capital volume for specific bank account.

TransactionDate:
Data model containing transaction amount for specific date.

AccountData:
Data model contains userID, amount and currency. Used for in transfering of funds between accounts.

TransactionDataObject:
Data model used to handle transactions. Contains userID, amount, TransactionType, DateTime. 

TransactionDataWrapper:
Data object that wrapps TransactionDataObject, supports multiple transaction objects to represent transaction between accounts, or single transaction for either direct withdrawal or deposit. Used to stage transactions.

--Enum--

Currency:
Enum representing all currencies

StatusCode:
Enum representing each status code used during transactions.

TransactionType:
Enum representing transaction type; deposit or withdrawal.

--Interface--

IAccountBridge:
Interface for AccountBridge

IRequestStatus:
Interface for objects that need to implement a request status enum

IViewTransactionStatisticBridge:
Interface for transaction statistic bridge.

--Service--

ExchangeRateCalculator:
One function class, multiplies two values.

StageTransaction:
Handles staged transactions. Staging them in the registry and executing on the transactions when schedule fires.

TransactionScheduler:
Handles running the scheduler (async) with an interval. Fires event every set interval.

TransactionStatisticFilter:
Filters stored transactions into data objects that can be read from view.

---PULL THE SACK---

--API--

GamblingFacade:
Small class that starts the game.

--Controller--

BankBridge:
Small bridge class passed back to bank that contains new loan value.

SlotMachineController:
State machine class that controls the logic loop, routing view and game logic based on current game phase state.

--DataModel--

GameState:
Record struct that holds the data for current game state.

PayoutEntry:
Record struct that holds symbol value and its corresponding multiplier value.

PayoutTable:
Creates a table (dict) for each symbol and its multiplier value, can be fetch from here.

Reel:
Holds current target symbol, has spinn function for generating that symbol.

SpinResult:
Record struct, holds list of all reel symbol targets as well as the final multiplier for the loan.

SymbolCatalog:
Container for mapping symbol enums to symbol graphics. Setup by orchestrator.

ViewRegistry:
Registry that holds the IViews the game uses.

--IO--

UserInput
Class that holds two different ways of reading user key, read comment over TryReadKey in LockConsole for reason.

--Service--

GameLoop:
Class that runs the async gameloop N frames each second. Fires tick event that updates views through ViewUpdater.

LockedConsole:
Class that gathers and handles all console outputs, locks the console with each call to prevent multiple threads writing over each other.

PayoutEvaluator:
Holds logic for evaluating the payout on the loan by using the symbols each reel land on and their multiplier value.

SlotMachine:Gameplay logic, handles gamestate updates, calling payoutevaluations, gameloop lifetime and progressing gamephase.

ViewUpdater:
Holds current IViews that are to be updated by game loop and call their render function.

--View--

GameView:
Responsible for gameview state, coloring, misc text, placement of reels (not rendering) and setting up/evaluating the rendering of target symbols for each reel.

ReelView:
Responsible for drawing symbols as well as settings placement for the box around them (and drawing box) using the origin placement from GameView.

ResultView:
Responsible for showing the result of the game.

SplashView:
Responsible for view state and graphic of splash screen.





## Documentation

- [UML diagrams](docs/uml/README.md) — architecture overview, per-module class
  diagrams and sequence diagrams, with editable Mermaid sources.
