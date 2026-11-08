# BankApp

BankApp is a C# console banking application. Customers can manage regular and savings accounts, transfer money, and apply for loans. Administrators manage users, exchange rates, and loan rules. Owners can also view bank-wide transaction statistics.

## Run the application

Use the .NET 10 SDK. Run these commands from the repository root.

```powershell
dotnet restore BankApp.slnx
dotnet run --project BankApp/BankApp.csproj --no-launch-profile
```

You can also open `BankApp.slnx` in Visual Studio. The console menus use the arrow keys to move, Enter to select, and Esc to leave a menu.

`UserService` creates the initial owner and administrator when the app starts. New customers receive an empty SEK account. Users, accounts, loans, exchange rates, and transaction history are held in memory for the current session.

Email delivery uses the `BANK_MAIL_ADDRESS` and `BANK_MAIL_PASSWORD` environment variables. Set these locally when testing delivery. Keep mail credentials out of Git.

## Source structure

All application code is under `BankApp/`. `Program.cs` creates the services, shares them between the menus, and starts `Menu.LoginMenu()`.

| Folder | Responsibility |
| --- | --- |
| `Users/` | User data, login, account lockout, and permissions |
| `Accounts/` | Regular accounts, savings accounts, and balance lookups |
| `Currencies/` | Supported currencies, stored rates, and currency conversion |
| `Loans/` | Loan applications, borrowing limits, interest rules, and repayments |
| `Transactions/` | Transfers, scheduled processing, transaction history, and statistics |
| `ConsoleUi/` | Role-based menus, loan menus, and console display |
| `Notifications/` | Email delivery |
| `Shared/` | ID generation shared by accounts and transactions |
| `Pull The Sack/` | Optional slot-machine game offered during loan applications |

### Users and accounts

| Type | Responsibility |
| --- | --- |
| `User` | Holds identity, contact details, role, password hash and salt, and failed login attempts. |
| `UserRole`, `LoginResult`, `CreateUserResult` | Define the roles and the possible results of login and user creation. |
| `UserService` | Creates and finds users, validates input, and handles profile and password changes. |
| `AuthService` | Tracks the signed-in user, checks permissions, and handles login, logout, and the three-attempt lockout. |
| `AccountManager` | Creates and finds accounts, checks ownership, and totals a customer's regular and savings balances in SEK for loan applications. |
| `BankAccount` | Holds a regular account's number, owner, balance, currency, and linked savings accounts. |
| `SavingsAccount` | Holds its own account number, balance, currency, and savings interest rate. |
| `UniqueIDGenerator` | Generates IDs used for account numbers and transaction groups. |

### Currencies

| Type | Responsibility |
| --- | --- |
| `Currency` | Lists the currency codes that an account can use. |
| `ExchangeRate` | Provides methods to set a rate, look it up, and convert an amount. One instance is shared across the banking features. |
| `ExchangeRateRegistry` | Stores one rate per currency pair and returns the inverse when the reverse direction is requested. |
| `ExchangeRateCalculator` | Applies a rate to an amount. |

### Loans

| Type | Responsibility |
| --- | --- |
| `Loan` | Stores the quoted terms and tracks approval, payout, remaining principal, installment dates, and late fees. |
| `LoanStatus` | Defines `Pending`, `Approved`, and `Rejected`. |
| `LoanService` | Handles applications and decisions, checks the borrowing limit, records payouts and repayments, and applies credit-score changes. |
| `InterestCalculator` | Calculates the interest rate, monthly installment, and total interest over the loan term. |
| `InterestRuleService` | Stores the base rates by loan term and the adjustments by credit-score range. |
| `InterestRule` | Represents one credit-score range and its interest adjustment. |
| `CreditScoreService` | Stores each customer's score and adjusts it after late or on-time payments. |

A customer applies for a term of 12–144 months and chooses a regular account for the payout. `LoanService` checks the five-times borrowing limit at application and again at approval, taking existing loans and pending applications into account. The customer sees the quoted interest rate, total interest, and repayment total when applying.

Once the admin approves the loan, the payout enters the transaction queue. The first installment is due one month after the payout is credited. The customer pays the monthly installment and any accumulated fees from a regular account. An admin can change the base rate or credit-score adjustment for future applications. Existing loans keep their quoted rate.

### Transactions

| Type | Responsibility |
| --- | --- |
| `BankTransfer` | Exposes operations for transfers, deposits, withdrawals, and loan payouts. |
| `BankTransaction` | Validates accounts, checks available funds, and converts amounts before handing a request to processing. |
| `IAccountBridge`, `AccountBridge` | Define and implement the connection between transaction processing, account balances, and loan payout confirmation. |
| `StageTransaction` | Queues requests and processes them on the scheduler tick. It also records completed transactions. |
| `TransactionScheduler` | Runs the processing callback every 15 minutes. |
| `TransactionRegistry` | Holds the queue of pending transactions and the history of completed entries. |
| `AccountData` | Carries the user ID, account number, and currency used in a request. |
| `TransactionDataObject` | Represents one debit or credit entry, including its amount and timestamp. |
| `TransactionDataLoanObject` | Adds the loan ID to a payout entry so the loan can be updated when the credit completes. |
| `TransactionDataWrapper` | Groups the entries for one queued operation, including both sides of a transfer. |
| `TransactionType`, `StatusCode` | Describe the kind of balance change and the result of a transaction request. |

Transfers, deposits, and loan payouts are queued. Cash withdrawals and loan repayments use the immediate withdrawal path. Completed entries are stored with UTC timestamps and shown in local time in the transaction history.

### Transaction history and statistics

| Type | Responsibility |
| --- | --- |
| `TransactionStatisticFilter` | Queries and groups completed entries by user, account, or source currency. |
| `BankTransferStatistic` | Returns customer history, transfer totals, and account rankings to callers. |
| `IViewTransactionStatisticBridge`, `TransactionStatisticBridge` | Provide an adapter for views that need transaction statistics. |
| `IRequestStatus` | Gives statistics results a shared status property. |
| `TransactionDate`, `TotalTransactionsDateOrder` | Represent history entries and the history grouped by account. |
| `TotalTransactionAmount`, `TotalTransactions` | Represent a user's transaction amount and bank-wide transfer totals by source currency, respectively. |
| `AccountPopularityCount`, `AccountPopularityVolume`, `MostPopularAccounts` | Hold account rankings by transaction count or volume. |
| `TotalCount`, `TotalVolume` | Hold the count or total amount for an individual account. |

The owner menu shows completed transfer totals by source currency. Each transfer contributes once to its total. The account ranking counts completed entries on each account.

### Console menus and email

| Type | Responsibility |
| --- | --- |
| `Menu` | Handles login and role selection, account and transaction actions, profile updates, exchange-rate entry, and the owner statistics screen. |
| `LoanMenu` | Handles customer applications, displays loan details, and takes installment payments. |
| `AdminLoanMenu` | Handles loan decisions and payout requests. It also contains the interest-rate submenu. |
| `GoldenSack`, `LogoConsole` | Draw and center the ASCII login logo. |
| `Theme` | Holds a Spectre.Console style for console markup. |
| `EmailService` (`Mail.cs`) | Sends email through SMTP using MailKit. |

The current mail flow sends lockout emails and attempts alerts when a customer queues a deposit or outgoing transfer above 10,000 SEK or its equivalent. The large-transaction alert currently runs in the customer menu when the request is accepted for queuing.

### Pull The Sack

`Pull The Sack` is an optional feature that the customer can choose during a loan application.

| Type | Responsibility |
| --- | --- |
| `GamblingFacade`, `Orchestrator` | Start a game session from the loan flow and assemble the game objects. |
| `SlotMachineController`, `SlotMachine` | Coordinate gameplay and manage spins, results, and game state. |
| `IBankInterface`, `BankBridge` | Pass the loan amount into the game and expose the resulting amount. |
| `Reel`, `SlotSymbol`, `SymbolCatalog` | Represent the reels and symbols used during a spin. |
| `PayoutTable`, `PayoutEntry`, `IPayoutEvaluator`, `PayoutEvaluator` | Define payout rules and evaluate the symbols from a spin. |
| `GameState`, `GamePhase`, `SpinResult` | Describe the current session, its phase, and the result of a spin. |
| `GameLoop`, `IViewUpdater`, `ViewUpdater` | Drive the display updates during the session. |
| `IConsole`, `LockedConsole`, `UserInput` | Provide console access and handle keyboard input. |
| `IView`, `ViewRegistry`, `ViewType`, `ViewState` | Define, register, and identify the game screens and their display states. |
| `SplashView`, `GameView`, `ReelView`, `ResultView`, `BalanceView` | Hold the game display classes, including the intro, reels, and result screens. |
| `ViewConfig`, `SplashViewConfig`, `GameViewConfig`, `ResultViewConfig` | Hold display settings and text for the game screens. |

## UML and user stories

- [BankApp class diagram](https://excalidraw.com/#json=4f_-nlKWgug9R4ZNNecw4,IxMGBuyL-tnXit5AHhrcfQ) shows the whole application and how its modules connect.
- [UML diagrams](docs/uml/README.md) contains the architecture overview, class diagrams by module, and sequence diagrams for the main flows.
- [Transaction layer UML image](docs/uml/Transaction%20Layer%20UML.png) shows the transaction module in more detail.
- [User stories](BankApp/UserStories.txt) lists the project requirements.
