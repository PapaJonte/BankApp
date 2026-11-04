# BankApp

SavingsAccount
Not to be confused with BankAccount!
SavingsAccount as a class serves the purpose primarily of storing the balance of accounts. Because this class is intended to have many almost completely identical variations in it's objects there is a constructor provided for assigning values to it's properties.

The properties include "Balance", "Interest", and "SavingsAccountNumber". Savings account number exists for the purpose of identifying such accounts later when transactions must be made, for there is supposed to be a multitude of savings accounts. Meanwhile Balance, and Interest serve a more obvious purpose.

The majority of methods are dedicated to returning the various different properties of the class. An exception to this rule is one method dedicated to reducing or increasing balance, for transaction purposes.

BankAccount
Not to be confused with SavingsAccount!
The BankAccount serves to contain SavingsAccounts, and sort them by user ID. Hence why it has both a list dedicated to storing objects of the SavingsAccount class, and a dictionary for storing the list.
The userId variable that represents the ID of the user, and the bank account number may be both be returned with a method the same way as values in the SavingsAccount class.

AccountManager
This class has various different methods for handling the two other Account classes. For example it may create objects of both the SavingsAccount class, and the BankAccount class by different methods for each type of class.
Other than that there happens to be a method for calculating the total balance of all accounts of a user, there is a method for printing out the balance and currency of account of a user, and there is one private method for choosing currency for created accounts.
Otherwise all methods are public, for selection of currency is only required for internal processes.

//Transaction Layer

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

//Pull The Sack

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

SlotMachine:

Gameplay logic, handles gamestate updates, calling payoutevaluations, gameloop lifetime and progressing gamephase.

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
