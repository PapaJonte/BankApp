"""Definitions for all BankApp UML diagrams.

Each entry renders one PNG via uml_render and one matching Mermaid (.mmd)
source file. Run:  python generate.py
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from uml_render import ClassNode, Edge, render_class_diagram, render_sequence

HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.abspath(os.path.join(HERE, '..', 'diagrams'))
os.makedirs(OUT, exist_ok=True)

DIAGRAMS = []


def cls(name, title=None, kind='class', attrs=(), ops=(), notes=(), col=0, row=0, stereo=None):
    return ClassNode(name, title, kind, list(attrs), list(ops), list(notes), col, row, stereo)


def add(name, title, kind, data, mmd):
    DIAGRAMS.append({'name': name, 'title': title, 'kind': kind, 'data': data, 'mmd': mmd})


# =====================================================================
# 01 — Users & Authentication (Marias' module, namespace BankApp)
# =====================================================================
users_auth_nodes = [
    cls('Menu', 'Menu', 'component',
        notes=['«BankApp.Robin» external', 'also uses UserService for', 'account creation (see 06)'],
        col=0, row=0),
    cls('AuthService', kind='class', col=1, row=0,
        attrs=['- _userService : UserService',
               '+ LoggedInUser : User?  {get; private set;}'],
        ops=['+ Login(username, password) : LoginResult',
             '+ Logout()',
             '+ IsAdmin(user) : bool',
             '+ IsLoggedInUserAdmin() : bool',
             '+ CanCreateAdminAccount() : bool',
             '+ UnlockUser(username) : bool']),
    cls('User', kind='class', col=2, row=0,
        attrs=['+ UserID : int',
               '+ Username : string  (personnummer)',
               '+ Password : string',
               '+ Email : string',
               '+ PhoneNumber : string',
               '+ Role : UserRole',
               '+ FailedLoginAttempts : int',
               '+ IsLockedOut : bool'],
        ops=['+ RegisterFailedLogin()',
             '+ UpdateUserInfo(newEmail, newPhone)']),
    cls('UserRole', kind='enum', col=3, row=0,
        attrs=['Customer = 0', 'Admin = 1', 'Owner = 2']),
    cls('LoginResult', kind='enum', col=1, row=1,
        attrs=['Success = 0', 'WrongCredentials = 1', 'LockedOut = 2']),
    cls('UserService', kind='class', col=2, row=1,
        attrs=['- _users : Dictionary<int, User>',
               '- _nextUserId : int = 1'],
        ops=['+ GetUserByUsername(username) : User?',
             '+ GetUserById(userId) : User?',
             '+ GetAllUsers() : IReadOnlyList<User>',
             '+ UsernameExists(username) : bool',
             '+ IsValidPersonalNumberFormat(u) : bool',
             '+ CreateUser(callerIsAdmin, callerIsOwner,',
             '    username, password, email, phone,',
             '    role) : bool',
             '+ UpdateUserInfo(user, newEmail, newPhone) : bool',
             '- AddUser(username, password, email, phone, role)',
             '- NormalizeUsername(username) : string'],
        notes=['ctor seeds a default Owner',
               'and Admin account']),
]
users_auth_edges = [
    Edge('Menu', 'AuthService', 'uses', '«uses»'),
    Edge('AuthService', 'User', 'assoc', 'LoggedInUser'),
    Edge('User', 'UserRole', 'assoc', 'Role'),
    Edge('AuthService', 'LoginResult', 'uses', 'returns'),
    Edge('AuthService', 'UserService', 'assoc', 'delegates to'),
    Edge('UserService', 'User', 'composes', '1..*  stores'),
]
add('01-users-auth', 'Users & Authentication — BankApp (Marias)', 'class',
    (users_auth_nodes, users_auth_edges), '''
classDiagram
direction LR
class User {
  +UserID : int
  +Username : string
  +Password : string
  +Email : string
  +PhoneNumber : string
  +Role : UserRole
  +FailedLoginAttempts : int
  +IsLockedOut : bool
  +RegisterFailedLogin()
  +UpdateUserInfo(newEmail, newPhone)
}
class UserRole {
  <<enumeration>>
  Customer
  Admin
  Owner
}
class LoginResult {
  <<enumeration>>
  Success
  WrongCredentials
  LockedOut
}
class UserService {
  -_users : Dictionary~int, User~
  -_nextUserId : int
  +GetUserByUsername(username) User?
  +GetUserById(userId) User?
  +GetAllUsers() IReadOnlyList
  +UsernameExists(username) bool
  +IsValidPersonalNumberFormat(username) bool
  +CreateUser(callerIsAdmin, callerIsOwner, username, password, email, phone, role) bool
  +UpdateUserInfo(user, newEmail, newPhone) bool
  -AddUser(username, password, email, phone, role)
  -NormalizeUsername(username) string
}
class AuthService {
  -_userService : UserService
  +LoggedInUser : User?
  +Login(username, password) LoginResult
  +Logout()
  +IsAdmin(user) bool
  +IsLoggedInUserAdmin() bool
  +CanCreateAdminAccount() bool
  +UnlockUser(username) bool
}
class Menu {
  <<external BankApp.Robin>>
}
Menu ..> AuthService : uses
Menu ..> UserService : uses
AuthService --> User : LoggedInUser
AuthService --> UserService : delegates to
UserService "1" *-- "1..*" User : stores
User --> UserRole : Role
AuthService ..> LoginResult : returns
''')

# =====================================================================
# 02 — Accounts (Gustafs classes, namespace BankApp)
# =====================================================================
accounts_nodes = [
    cls('AccountManager', kind='class', col=0, row=0,
        attrs=['- bankAccountList : List<BankAccount>'],
        ops=['+ CreateAccount(userId)',
             '+ CreateSavingsAccount(userId)',
             '+ GetAccounts(userId)',
             '+ GetTotalBalance(userId) : decimal',
             '- CheckCurrency() : Currency'],
        notes=['console-driven front-end,',
               'prompts for amounts/currency']),
    cls('BankAccount', kind='class', col=1, row=0,
        attrs=['+ savingAccounts : List<SavingsAccount>',
               '- userId : int'],
        ops=['+ GetUserId() : int'],
        notes=['a customer\'s banking identity,',
               'aggregates SavingsAccounts']),
    cls('SavingsAccount', kind='class', col=1, row=1,
        attrs=['- Balance : decimal',
               '- Currency : Currency',
               '- Interest : decimal'],
        ops=['+ GetBalance() : decimal',
             '+ GetCurrency() : Currency',
             '+ GetInterest() : decimal']),
    cls('ExchangeRate', 'ExchangeRate', 'class', col=0, row=1,
        attrs=['- exchangeRateRegistry'],
        ops=['+ SetExchangeRate(c1, c2, rate)',
             '+ TryCalculateExchangeValue(c1, c2,',
             '    value, out amount) : StatusCode'],
        stereo='«Transaction Layer API»'),
    cls('Currency', kind='enum', col=1, row=2,
        attrs=['EUR, GBP, NOK, SEK, USD,', '... 154 ISO-4217 codes'],
        stereo='«Transaction Layer enum»'),
]
accounts_edges = [
    Edge('AccountManager', 'BankAccount', 'composes', '1..*  owns'),
    Edge('BankAccount', 'SavingsAccount', 'composes', '0..*  savingAccounts'),
    Edge('AccountManager', 'SavingsAccount', 'uses', '«uses»'),
    Edge('AccountManager', 'ExchangeRate', 'uses', '«uses»'),
    Edge('SavingsAccount', 'Currency', 'assoc', 'Currency'),
    Edge('ExchangeRate', 'Currency', 'uses', 'keys'),
]
add('02-accounts', 'Accounts — BankApp (Gustaf)', 'class',
    (accounts_nodes, accounts_edges), '''
classDiagram
direction LR
class AccountManager {
  -bankAccountList : List~BankAccount~
  +CreateAccount(userId)
  +CreateSavingsAccount(userId)
  +GetAccounts(userId)
  +GetTotalBalance(userId) decimal
  -CheckCurrency() Currency
}
class BankAccount {
  +savingAccounts : List~SavingsAccount~
  -userId : int
  +GetUserId() int
}
class SavingsAccount {
  -Balance : decimal
  -Currency : Currency
  -Interest : decimal
  +GetBalance() decimal
  +GetCurrency() Currency
  +GetInterest() decimal
}
class ExchangeRate {
  <<external Transaction Layer API>>
  -exchangeRateRegistry : ExchangeRateRegistry
  +SetExchangeRate(c1, c2, rate)
  +TryCalculateExchangeValue(c1, c2, value, out amount) StatusCode
}
class Currency {
  <<enumeration external>>
  EUR GBP NOK SEK USD
  ...154 ISO-4217 codes
}
AccountManager "1" *-- "1..*" BankAccount : owns
BankAccount "1" *-- "0..*" SavingsAccount : savingAccounts
AccountManager ..> SavingsAccount : creates/reads
AccountManager ..> ExchangeRate : uses
SavingsAccount --> Currency : Currency
ExchangeRate ..> Currency : keys
''')

# =====================================================================
# 03 — Transaction Layer: transfers, staging & scheduling
# =====================================================================
txn_nodes = [
    cls('BankTransfer', kind='class', col=0, row=0, stereo='«API»',
        ops=['+ TransferMoney(origin, target,',
             '    amount, currency) : StatusCode',
             '+ DepositMoney(accountNumber,',
             '    amount) : StatusCode'],
        notes=['public facade of the layer']),
    cls('BankTransaction', kind='class', col=1, row=0, stereo='«Controller»',
        attrs=['- stageTransaction : StageTransaction',
               '- bankTransaction : BankTransaction',
               '- exchangeRateModule : ExchangeRate'],
        ops=['+ RequestDeposit(accountNumber, amount)',
             '+ RequestWithdrawal(accountNumber, amount)',
             '+ TransferMoney(origin, target,',
             '    amount, currency) : StatusCode'],
        notes=['computes both currency legs,',
               'then stages deposit + withdrawal']),
    cls('StageTransaction', kind='class', col=2, row=0,
        attrs=['- transactionScheduler',
               '- transactionRegistry'],
        ops=['+ Stage(accountNumber, amount, type)',
             '- InitScheduler()',
             '- OnSchedule()'],
        notes=['ctor starts the scheduler']),
    cls('TransactionScheduler', kind='class', col=3, row=0,
        attrs=['- ct : CancellationTokenSource',
               '- OnSchedule : Action?'],
        ops=['+ StartSchedule(listener)',
             '+ CancelSchedule()',
             '- TransactionSchedule(...) : Task'],
        notes=['async Task.Delay loop, tick',
               'every 15 (config) minutes']),
    cls('TransactionRegistry', kind='class', col=3, row=1,
        attrs=['- stagedTransactions :',
             '    Queue<TransactionDataObject>',
             '- registryLock : Lock'],
        ops=['+ RegisterStagedTransaction(t)',
             '+ GetAllStagedTransactions() : List',
             '+ GetNextStagedTransaction() : TDO',
             '+ ClearStagedTransactions()'],
        notes=['thread-safe FIFO queue']),
    cls('TransactionDataObject', kind='record', col=2, row=1,
        stereo='«record struct»',
        attrs=['+ AccountNumber : string',
               '+ Amount : decimal',
               '+ TransactionType : TransactionType']),
    cls('AccountData', kind='record', col=0, row=1, stereo='«record struct»',
        attrs=['+ AccountNumber : string',
               '+ Currency : Currency']),
    cls('StatusCode', kind='enum', col=1, row=2,
        attrs=['Successful', 'Aborted', 'Failed', 'Invalid']),
    cls('TransactionType', kind='enum', col=2, row=2,
        attrs=['Deposit', 'Withdrawal']),
]
txn_edges = [
    Edge('BankTransfer', 'BankTransaction', 'uses', 'delegates'),
    Edge('BankTransfer', 'AccountData', 'uses', 'parameters'),
    Edge('BankTransfer', 'StatusCode', 'uses', 'returns'),
    Edge('BankTransaction', 'StageTransaction', 'assoc', 'stages via'),
    Edge('BankTransaction', 'AccountData', 'uses', 'reads'),
    Edge('BankTransaction', 'StatusCode', 'uses', 'returns'),
    Edge('StageTransaction', 'TransactionScheduler', 'assoc', 'callback'),
    Edge('StageTransaction', 'TransactionRegistry', 'assoc', 'enqueues'),
    Edge('StageTransaction', 'TransactionDataObject', 'uses', '«create»'),
    Edge('TransactionRegistry', 'TransactionDataObject', 'composes', '1..*  queues'),
    Edge('TransactionDataObject', 'TransactionType', 'assoc', 'TransactionType'),
]
add('03-transactions', 'Transaction Layer — transfers, staging & scheduling', 'class',
    (txn_nodes, txn_edges), '''
classDiagram
direction LR
class BankTransfer {
  <<API>>
  +TransferMoney(origin, target, amount, currency) StatusCode
  +DepositMoney(accountNumber, amount) StatusCode
}
class BankTransaction {
  <<Controller>>
  -stageTransaction : StageTransaction
  -bankTransaction : BankTransaction
  -exchangeRateModule : ExchangeRate
  +RequestDeposit(accountNumber, amount)
  +RequestWithdrawal(accountNumber, amount)
  +TransferMoney(origin, target, amount, currency) StatusCode
}
class StageTransaction {
  -transactionScheduler : TransactionScheduler
  -transactionRegistry : TransactionRegistry
  +Stage(accountNumber, amount, type)
  -InitScheduler()
  -OnSchedule()
}
class TransactionScheduler {
  -ct : CancellationTokenSource
  -OnSchedule : Action
  +StartSchedule(listener)
  +CancelSchedule()
  -TransactionSchedule(duration, ct) Task
}
class TransactionRegistry {
  -stagedTransactions : Queue~TransactionDataObject~
  -registryLock : Lock
  +RegisterStagedTransaction(t)
  +GetAllStagedTransactions() List
  +GetNextStagedTransaction() TDO
  +ClearStagedTransactions()
}
class TransactionDataObject {
  <<record struct>>
  +AccountNumber : string
  +Amount : decimal
  +TransactionType : TransactionType
}
class AccountData {
  <<record struct>>
  +AccountNumber : string
  +Currency : Currency
}
class StatusCode {
  <<enumeration>>
  Successful
  Aborted
  Failed
  Invalid
}
class TransactionType {
  <<enumeration>>
  Deposit
  Withdrawal
}
BankTransfer ..> BankTransaction : delegates
BankTransfer ..> AccountData : parameters
BankTransfer ..> StatusCode : returns
BankTransaction --> StageTransaction : stages via
BankTransaction ..> AccountData : reads
BankTransaction ..> TransactionType : Deposit/Withdrawal
StageTransaction --> TransactionScheduler : callback
StageTransaction --> TransactionRegistry : enqueues
StageTransaction ..> TransactionDataObject : create
TransactionRegistry "1" *-- "1..*" TransactionDataObject : queues
BankTransaction ..> StatusCode : returns
''')

# =====================================================================
# 04 — Transaction Layer: exchange rates & service locator
# =====================================================================
infra_nodes = [
    cls('ExchangeRate', kind='class', col=0, row=0, stereo='«API»',
        attrs=['- exchangeRateRegistry :',
             '    ExchangeRateRegistry'],
        ops=['+ SetExchangeRate(c1, c2, rate) : StatusCode',
             '+ TryGetExchangeRate(c1, c2,',
             '    out rate) : StatusCode',
             '+ TryCalculateExchangeValue(c1, c2,',
             '    value, out amount) : StatusCode'],
        notes=['each instance owns a private',
               'registry (never persisted)']),
    cls('ExchangeRateRegistry', kind='class', col=1, row=0,
        attrs=['- exchangeRates :',
             '    Dictionary<(Currency, Currency),',
             '              decimal>'],
        ops=['+ SetExchangeRate(c1, c2, rate)',
             '+ TryGetExchangeRate(c1, c2,',
             '    out rate) : bool'],
        notes=['removes inverse pair on set,',
               'inverts (1/rate) on lookup']),
    cls('Currency', kind='enum', col=2, row=0,
        attrs=['EUR, GBP, NOK, SEK, USD,', '... 154 ISO-4217 codes']),
    cls('ExchangeRateCalculator', kind='class', col=0, row=1,
        ops=['+ static CalculateWithExchangeRate(',
             '      val, rate) : decimal'],
        notes=['pure helper: val * rate']),
    cls('BankTransactionServiceLocator', kind='class', col=1, row=1,
        attrs=['- _instance : ...ServiceLocator',
             '    {static readonly}',
             '- _services : Dictionary<Type, object>'],
        ops=['+ Instance : ...ServiceLocator',
             '    {static; get;}',
             '+ RegisterService(o)',
             '+ TryGetService(t, out o) : bool'],
        notes=['eager singleton, bootstraps',
               'the whole layer on first access']),
    cls('LocatorBootstrap', kind='static', col=2, row=1,
        ops=['+ static RegisterAll(locator)'],
        notes=['composition root — creates and',
               'registers ExchangeRate,',
               'BankTransaction and BankTransfer']),
]
infra_edges = [
    Edge('ExchangeRate', 'ExchangeRateRegistry', 'composes', 'owns'),
    Edge('ExchangeRate', 'ExchangeRateCalculator', 'uses', 'multiplies'),
    Edge('ExchangeRateRegistry', 'Currency', 'uses', 'tuple key'),
    Edge('LocatorBootstrap', 'BankTransactionServiceLocator', 'uses', 'populates'),
]
add('04-exchange-locator', 'Transaction Layer — exchange rates & service locator', 'class',
    (infra_nodes, infra_edges), '''
classDiagram
direction LR
class ExchangeRate {
  <<API>>
  -exchangeRateRegistry : ExchangeRateRegistry
  +SetExchangeRate(c1, c2, rate) StatusCode
  +TryGetExchangeRate(c1, c2, out rate) StatusCode
  +TryCalculateExchangeValue(c1, c2, value, out amount) StatusCode
}
class ExchangeRateRegistry {
  -exchangeRates : Dictionary~(Currency, Currency), decimal~
  +SetExchangeRate(c1, c2, rate)
  +TryGetExchangeRate(c1, c2, out rate) bool
}
class Currency {
  <<enumeration>>
  EUR GBP NOK SEK USD
  ...154 ISO-4217 codes
}
class ExchangeRateCalculator {
  +CalculateWithExchangeRate(val, rate) decimal
}
class BankTransactionServiceLocator {
  -_instance : BankTransactionServiceLocator
  -_services : Dictionary~Type, object~
  +Instance : BankTransactionServiceLocator
  +RegisterService(o)
  +TryGetService(t, out o) bool
}
class LocatorBootstrap {
  <<utility>>
  +RegisterAll(locator)$
}
ExchangeRate "1" *-- "1" ExchangeRateRegistry : owns
ExchangeRate ..> ExchangeRateCalculator : multiplies
ExchangeRateRegistry ..> Currency : tuple key
LocatorBootstrap ..> BankTransactionServiceLocator : populates
''')

# =====================================================================
# 05 — Loans (Jonathan's module)
# =====================================================================
loans_nodes = [
    cls('LoanMenu', 'LoanMenu', 'component', col=1, row=0,
        notes=['«BankApp.Robin» external']),
    cls('LoanService', kind='class', col=1, row=1,
        attrs=['- loans : List<Loan>',
               '- interestRuleService',
               '- calculator : InterestCalculator',
               '- creditScoreService',
               '- nextLoandId : int = 1'],
        ops=['+ ApplyForLoan(userId, amount,',
             '    duration, balance) : Loan?',
             '+ ValidateLoanApplication(...) : bool',
             '+ CalculateEligibleBalance(...) : decimal',
             '+ CalculateMaxLoanAmount(balance) : decimal',
             '+ CalculateOutstandingLoans(userId) : decimal',
             '+ ChangeLoanStatus(loanId, status,',
             '    balance) : bool',
             '+ RepayLoan(loanId, amount) : bool',
             '+ GetMonthlyPayment(loanId) : decimal',
             '+ UpdateInterestRule(min, max, adj)',
             '+ GetCustomersLoan(userId) : List<Loan>']),
    cls('CreditScoreService', kind='class', col=0, row=0,
        attrs=['- creditScores :',
             '    Dictionary<int, int>'],
        ops=['+ GetCreditScore(userId) : int',
             '+ UpdateCreditScore(userId, score)',
             '+ IncreaseCreditScore(userId, amount)',
             '+ DecreaseCreditScore(userId, amount)'],
        notes=['defaults to 100, clamped 0-100']),
    cls('Loan', kind='class', col=2, row=0,
        attrs=['+ UserId : int', '+ LoanId : int',
               '+ LoanAmount : decimal {get; private set;}',
               '+ RemainingLoanBalance : decimal',
               '+ InterestRate : decimal', '+ Duration : int',
               '+ TotalInterest : decimal',
               '+ TotalRepayment : decimal',
               '+ Status : LoanStatus {get; private set;}',
               '+ ApprovalDate : DateTime?',
               '+ NextPaymentDate : DateTime?',
               '+ PaymentsCompleted : int',
               '+ InterestPaid : decimal'],
        ops=['- ReduceBalance(amount)',
             '- UpdateStatus(newStatus)',
             '- StartRepaymentSchedule(date)',
             '- RecordMonthlyPayment(interest,',
             '    amortization)']),
    cls('LoanStatus', kind='enum', col=3, row=0,
        attrs=['Pending = 0', 'Approved = 1', 'Rejected = 2']),
    cls('InterestCalculator', kind='class', col=0, row=2,
        attrs=['- interestRuleService'],
        ops=['+ GetInterestRate(duration) : decimal',
             '+ GetInterestRate(duration,',
             '    creditScore) : decimal',
             '+ CalculateMonthlyAmortization(',
             '    amount, duration) : decimal',
             '+ CalculateMonthlyInterest(balance,',
             '    rate) : decimal',
             '+ CalculateMonthlyPayment(...) : decimal',
             '+ CalculateTotalInterest(amount,',
             '    duration, rate) : decimal',
             '+ CalculateTotalRepayment(amount,',
             '    duration, rate) : decimal']),
    cls('InterestRuleService', kind='class', col=2, row=2,
        attrs=['- interestRules : List<InterestRule>',
               '  seeded: 0-29:+5.0, 30-49:+3.5,',
               '  50-69:+2.0, 70-89:+1.0, 90-100:+0.0'],
        ops=['+ GetInterestAdjustment(score) : decimal',
             '+ UpdateInterestAdjustment(',
             '    min, max, newAdjustment)']),
    cls('InterestRule', kind='class', col=3, row=2,
        attrs=['+ MinimumCreditScore : int',
               '+ MaximumCreditScore : int',
               '+ InterestAdjustment : decimal'],
        notes=['credit-score band +',
               'rate adjustment']),
]
loans_edges = [
    Edge('LoanMenu', 'LoanService', 'uses', '«uses»'),
    Edge('LoanService', 'CreditScoreService', 'assoc', 'prices via'),
    Edge('LoanService', 'InterestCalculator', 'assoc', 'uses'),
    Edge('LoanService', 'InterestRuleService', 'assoc', 'rules + updates'),
    Edge('LoanService', 'Loan', 'composes', '*  manages'),
    Edge('Loan', 'LoanStatus', 'assoc', 'Status'),
    Edge('InterestCalculator', 'InterestRuleService', 'assoc', 'adjustments'),
    Edge('InterestRuleService', 'InterestRule', 'composes', '5 bands'),
]
add('05-loans', 'Loans — BankApp.Jonathan', 'class',
    (loans_nodes, loans_edges), '''
classDiagram
direction LR
class LoanMenu {
  <<external BankApp.Robin>>
}
class LoanService {
  -loans : List~Loan~
  -interestRuleService : InterestRuleService
  -calculator : InterestCalculator
  -creditScoreService : CreditScoreService
  -nextLoandId : int
  +ApplyForLoan(userId, amount, duration, balance) Loan?
  +ValidateLoanApplication(requested, eligible, committed) bool
  +CalculateEligibleBalance(balance, outstanding) decimal
  +CalculateMaxLoanAmount(balance) decimal
  +CalculateOutstandingLoans(userId) decimal
  +ChangeLoanStatus(loanId, status, balance) bool
  +RepayLoan(loanId, amount) bool
  +GetMonthlyPayment(loanId) decimal
  +UpdateInterestRule(min, max, adj)
  +GetCustomersLoan(userId) List~Loan~
}
class CreditScoreService {
  -creditScores : Dictionary~int, int~
  +GetCreditScore(userId) int
  +UpdateCreditScore(userId, score)
  +IncreaseCreditScore(userId, amount)
  +DecreaseCreditScore(userId, amount)
}
class Loan {
  +UserId : int
  +LoanId : int
  +LoanAmount : decimal
  +RemainingLoanBalance : decimal
  +InterestRate : decimal
  +Duration : int
  +TotalInterest : decimal
  +TotalRepayment : decimal
  +Status : LoanStatus
  +ApprovalDate : DateTime
  +NextPaymentDate : DateTime
  +PaymentsCompleted : int
  +InterestPaid : decimal
  -ReduceBalance(amount)
  -UpdateStatus(newStatus)
  -StartRepaymentSchedule(date)
  -RecordMonthlyPayment(interest, amortization)
}
class LoanStatus {
  <<enumeration>>
  Pending
  Approved
  Rejected
}
class InterestCalculator {
  -interestRuleService : InterestRuleService
  +GetInterestRate(duration) decimal
  +GetInterestRate(duration, creditScore) decimal
  +CalculateMonthlyAmortization(amount, duration) decimal
  +CalculateMonthlyInterest(balance, rate) decimal
  +CalculateMonthlyPayment(...) decimal
  +CalculateTotalInterest(amount, duration, rate) decimal
  +CalculateTotalRepayment(amount, duration, rate) decimal
}
class InterestRuleService {
  -interestRules : List~InterestRule~
  +GetInterestAdjustment(creditScore) decimal
  +UpdateInterestAdjustment(min, max, newAdjustment)
}
class InterestRule {
  +MinimumCreditScore : int
  +MaximumCreditScore : int
  +InterestAdjustment : decimal
}
LoanMenu ..> LoanService : uses
LoanService --> CreditScoreService : prices via
LoanService --> InterestCalculator : uses
LoanService --> InterestRuleService : rules + updates
LoanService "1" *-- "*" Loan : manages
Loan --> LoanStatus : Status
InterestCalculator --> InterestRuleService : adjustments
InterestRuleService "1" *-- "5" InterestRule : bands
''')

# =====================================================================
# 06 — Menus & presentation (Robin's module)
# =====================================================================
ui_nodes = [
    cls('UserService', 'UserService', 'component', col=0, row=0,
        notes=['«Marias» external']),
    cls('Menu', kind='class', col=1, row=0,
        attrs=['- _loanMenu : LoanMenu',
               '- _userService : UserService',
               '- _authService : AuthService'],
        ops=['+ LoginMenu()',
             '+ CreateUser() : bool',
             '+ ShowCreatedUsers()',
             '+ ShowAdminMenu(userId)',
             '+ ShowUserMenu()',
             '+ ShowOwnerMenu()'],
        notes=['arrow-key menus, Console.Clear redraw']),
    cls('LoanMenu', kind='class', col=2, row=0,
        attrs=['- loanService : LoanService',
               '- accountManager : AccountManager'],
        ops=['+ ShowLoanMenu(userId)',
             '+ ApplyForLoan(userId, amount,',
             '    duration) : Loan?',
             '+ ChangeLoanStatus(loanId,',
             '    newStatus) : bool'],
        notes=['apply for loan / view my loans']),
    cls('LoanService', 'LoanService', 'component', col=3, row=0,
        notes=['«Jonathan» external']),
    cls('AuthService', 'AuthService', 'component', col=0, row=1,
        notes=['«Marias» external']),
    cls('Theme', kind='class', col=1, row=2,
        attrs=['- style : Style'],
        ops=['+ Markup(input)'],
        notes=['Spectre.Console blue/white',
               'bold style; not wired in']),
    cls('EmailService', kind='class', col=2, row=2,
        ops=['+ SendEmail(recipient, subject, body)'],
        notes=['MailKit SMTP/IMAP sender,',
               'not wired to anything yet']),
    cls('AccountManager', 'AccountManager', 'component', col=3, row=1,
        notes=['«Gustaf» external']),
]
ui_edges = [
    Edge('Menu', 'LoanMenu', 'assoc', 'opens'),
    Edge('Menu', 'UserService', 'uses', '«uses»'),
    Edge('Menu', 'AuthService', 'uses', '«uses»'),
    Edge('LoanMenu', 'LoanService', 'uses', '«uses»'),
    Edge('LoanMenu', 'AccountManager', 'uses', '«uses»'),
]
add('06-menus-ui', 'Menus & presentation — BankApp.Robin', 'class',
    (ui_nodes, ui_edges), '''
classDiagram
direction LR
class Menu {
  -_loanMenu : LoanMenu
  -_userService : UserService
  -_authService : AuthService
  +LoginMenu()
  +CreateUser() bool
  +ShowCreatedUsers()
  +ShowAdminMenu(userId)
  +ShowUserMenu()
  +ShowOwnerMenu()
}
class LoanMenu {
  -loanService : LoanService
  -accountManager : AccountManager
  +ShowLoanMenu(userId)
  +ApplyForLoan(userId, amount, duration) Loan?
  +ChangeLoanStatus(loanId, newStatus) bool
}
class Theme {
  -style : Style
  +Markup(input)
}
class EmailService {
  +SendEmail(recipient, subject, body)
}
class UserService {
  <<external Marias>>
}
class AuthService {
  <<external Marias>>
}
class LoanService {
  <<external Jonathan>>
}
class AccountManager {
  <<external Gustaf>>
}
Menu --> LoanMenu : opens
Menu ..> UserService : uses
Menu ..> AuthService : uses
LoanMenu ..> LoanService : uses
LoanMenu ..> AccountManager : uses
''')

# =====================================================================
# 00 — Architecture / package overview
# =====================================================================
arch_nodes = [
    cls('marias', 'Users & Authentication', 'package', col=0, row=0,
        stereo='«namespace: BankApp»',
        notes=['User, UserRole, LoginResult',
               'UserService — in-memory registry,',
               '  seeds Owner + Admin',
               'AuthService — login, 3-strike',
               '  lockout, role checks']),
    cls('root', 'BankApp (root)', 'package', col=1, row=0,
        stereo='«composition root»',
        notes=['Program.Main() wires: Theme,',
               '  UserService, AuthService,',
               '  CreditScoreService, InterestRuleService,',
               '  LoanService, AccountManager,',
               '  LoanMenu, Menu → LoginMenu()',
               'GoldenSack — ASCII logo writer']),
    cls('game', 'Pull The Sack (slot machine)', 'package', col=2, row=0,
        stereo='«namespace: BankApp.Pull_The_Sack»',
        notes=['GamblingFacade, Orchestrator',
               'SlotMachineController (global ns)',
               'SlotMachine, Reel, SymbolCatalog',
               'PayoutEvaluator, PayoutTable',
               'Views: Splash/Game/Reel/Result',
               'ViewUpdater, ViewRegistry, GameLoop',
               'IConsole/LockedConsole, UserInput',
               'BankBridge : IBankInterface']),
    cls('jonathan', 'Loans', 'package', col=0, row=1,
        stereo='«namespace: BankApp.Jonathan»',
        notes=['Loan, LoanStatus',
               'LoanService — applications,',
               '  approval, repayment (5x limit)',
               'CreditScoreService',
               'InterestCalculator, InterestRuleService,',
               '  InterestRule']),
    cls('robin', 'Menus & Presentation', 'package', col=1, row=1,
        stereo='«namespace: BankApp.Robin»',
        notes=['Menu — login + role menus',
               'LoanMenu — loan UI',
               'Theme — Spectre styling (unwired)',
               'EmailService — MailKit sender',
               '  (unwired, global namespace)']),
    cls('gustaf', 'Accounts', 'package', col=2, row=1,
        stereo='«namespace: BankApp»',
        notes=['AccountManager — console front-end',
               'BankAccount — per-user identity',
               'SavingsAccount — balance,',
               '  currency, interest']),
    cls('transaction', 'Transaction Layer', 'package', col=3, row=1,
        stereo='«namespace: BankApp.Transaction_Layer»',
        notes=['BankTransfer, ExchangeRate (API)',
               'BankTransaction, StageTransaction',
               'TransactionScheduler, TransactionRegistry',
               'ExchangeRateRegistry, ExchangeRateCalculator',
               'BankTransactionServiceLocator + bootstrap',
               'record structs: AccountData,',
               '  TransactionDataObject',
               'enums: Currency (154), StatusCode,',
               '  TransactionType']),
]
arch_edges = [
    Edge('root', 'marias', 'uses', 'wires'),
    Edge('root', 'gustaf', 'uses', 'wires'),
    Edge('root', 'robin', 'uses', 'wires'),
    Edge('root', 'jonathan', 'uses', 'wires'),
    Edge('root', 'game', 'uses', 'GoldenSack uses IConsole'),
    Edge('robin', 'marias', 'uses', 'login / user mgmt'),
    Edge('robin', 'jonathan', 'uses', 'loan menu'),
    Edge('robin', 'gustaf', 'uses', 'balances'),
    Edge('gustaf', 'transaction', 'uses', 'ExchangeRate, Currency'),
]
add('00-architecture', 'BankApp — package / architecture overview', 'class',
    (arch_nodes, arch_edges), '''
flowchart TB
subgraph ROOT["BankApp (root)"]
  P["Program.Main() — composition root<br/>wires Theme, UserService, AuthService, CreditScoreService,<br/>InterestRuleService, LoanService, AccountManager, LoanMenu, Menu<br/>then runs menu.LoginMenu()"]
  G["GoldenSack — ASCII logo writer (uses IConsole)"]
end
subgraph MARIAS["Users & Authentication — BankApp (Marias)"]
  M1["User, UserRole, LoginResult"]
  M2["UserService — in-memory registry (seeds Owner + Admin)"]
  M3["AuthService — login, 3-strike lockout, role checks"]
end
subgraph ROBIN["Menus & Presentation — BankApp.Robin"]
  R1["Menu — login + role menus"]
  R2["LoanMenu — loan UI"]
  R3["Theme — Spectre styling (unwired)"]
  R4["EmailService — MailKit sender (unwired, global ns)"]
end
subgraph JONATHAN["Loans — BankApp.Jonathan"]
  J1["Loan, LoanStatus"]
  J2["LoanService — applications, approval, repayment (5x limit)"]
  J3["CreditScoreService, InterestCalculator, InterestRuleService, InterestRule"]
end
subgraph GUSTAF["Accounts — BankApp (Gustaf)"]
  GU1["AccountManager — console front-end"]
  GU2["BankAccount — per-user identity"]
  GU3["SavingsAccount — balance, currency, interest"]
end
subgraph TXN["Transaction Layer — BankApp.Transaction_Layer"]
  T1["BankTransfer, ExchangeRate (API)"]
  T2["BankTransaction, StageTransaction (Controller)"]
  T3["TransactionScheduler, TransactionRegistry"]
  T4["ExchangeRateRegistry, ExchangeRateCalculator"]
  T5["BankTransactionServiceLocator + LocatorBootstrap"]
end
subgraph GAME["Pull The Sack — BankApp.Pull_The_Sack"]
  GA1["GamblingFacade, Orchestrator"]
  GA2["SlotMachineController (global ns), SlotMachine, Reel, SymbolCatalog"]
  GA3["PayoutEvaluator, PayoutTable, SpinResult, GameState"]
  GA4["Views: Splash/Game/Reel/Result + ViewUpdater, ViewRegistry, GameLoop"]
  GA5["IConsole/LockedConsole, UserInput, BankBridge : IBankInterface"]
end
ROOT -.->|wires| MARIAS
ROOT -.->|wires| ROBIN
ROOT -.->|wires| JONATHAN
ROOT -.->|wires| GUSTAF
ROOT -.->|GoldenSack uses IConsole| GAME
ROBIN -.->|login / user mgmt| MARIAS
ROBIN -.->|loan menu| JONATHAN
ROBIN -.->|balances| GUSTAF
GUSTAF -.->|ExchangeRate, Currency| TXN
''')

# =====================================================================
# 07 — Game model (Pull The Sack)
# =====================================================================
game_model_nodes = [
    cls('GamblingFacade', kind='class', col=0, row=0,
        ops=['+ GambleOnLoan(loan) : IBankInterface'],
        notes=['intended entry point for the',
               'bank side (session runs twice —',
               'Start + PullTheSack)']),
    cls('Orchestrator', kind='class', col=1, row=0,
        ops=['+ Start(loanAmount) :',
             '    SlotMachineController'],
        notes=['composition root — constructs',
               'and wires every object']),
    cls('SlotMachineController', kind='class', col=2, row=0,
        stereo='«global namespace»',
        attrs=['machine : SlotMachine',
               'updater : IViewUpdater',
               'bank : IBankInterface',
               'userInput : UserInput',
               'viewRegistry : ViewRegistry'],
        ops=['+ PullTheSack() : IBankInterface',
             '- RunSession() : IBankInterface',
             '- StateControl(gameState)',
             '- ServeViewToUpdater(type)',
             '- RemoveViewFromUpdater(type)',
             '- ClearAllViews()'],
        notes=['busy-spins the state machine',
               'until GamePhase = Exiting']),
    cls('IBankInterface', kind='interface', col=3, row=0,
        attrs=['+ GetLoanAmount() : decimal',
               '+ SetLoanAmount(amount)']),
    cls('BankBridge', kind='class', col=4, row=0,
        attrs=['- _loanAmount : decimal'],
        ops=['+ GetLoanAmount() : decimal',
             '+ SetLoanAmount(amount)'],
        notes=['carries the loan amount',
               'between bank and game']),
    cls('GameState', kind='record', col=0, row=1, stereo='«record struct»',
        attrs=['+ GamePhase : GamePhase',
               '+ Balance : decimal',
               '+ SpinCount : int',
               '+ SpinResult : SpinResult?'],
        notes=['immutable snapshot, mutated',
               'with with-expressions']),
    cls('SpinResult', kind='record', col=0, row=2, stereo='«record»',
        attrs=['+ Symbols : IReadOnlyList<SlotSymbol>',
               '+ PayoutMultiplier : decimal']),
    cls('IPayoutEvaluator', kind='interface', col=2, row=1,
        attrs=['+ Evaluate(symbols) : decimal']),
    cls('PayoutEvaluator', kind='class', col=2, row=2,
        attrs=['- table : PayoutTable'],
        ops=['+ Evaluate(symbols) : decimal'],
        notes=['most frequent symbol: count/3 ×',
               'table value; else 0.5']),
    cls('PayoutTable', kind='class', col=2, row=3,
        attrs=['+ Entries : IReadOnlyList<PayoutEntry>?'],
        ops=['+ GetPayoutValue(symbol) : decimal',
             '- GenerateTable()'],
        notes=['7 entries: Sackpot 5x ...',
               'Ampersand 1.6x']),
    cls('PayoutEntry', kind='record', col=3, row=3, stereo='«record struct»',
        attrs=['+ Symbol : SlotSymbol',
               '+ Multiplier : decimal']),
    cls('SlotMachine', kind='class', col=1, row=2,
        attrs=['+ State : GameState {get; private set;}'],
        ops=['+ StartGame()', '+ Spin()', '+ ResolveSpinResult()',
             '+ Apply()', '+ EndGame()',
             '- UpdateGamePhase(phase)']),
    cls('GameLoop', kind='class', col=1, row=3,
        attrs=['- _glts : CancellationTokenSource',
               '- _gameTime : Stopwatch',
               '- updater : IViewUpdater'],
        ops=['+ StartGameLoopAsync()',
             '+ StopGameLoopAsync()',
             '- RunAsync(...) : Task'],
        notes=['fire-and-forget Task; 2 fps →',
               'ViewUpdater.Tick(deltaTime)']),
    cls('Reel', kind='class', col=0, row=3,
        attrs=['- _catalog : SymbolCatalog',
               '- rnd : Random',
               '+ CurrentSymbol : SlotSymbol'],
        ops=['+ Spin()']),
    cls('SymbolCatalog', kind='class', col=1, row=4,
        attrs=['+ Symbols : IReadOnlyDictionary<int,',
             '    SlotSymbol> {get; private set;}',
             '+ {static} SymbolGraphic'],
        ops=['+ {static} GetSymbolGraphicNullCheck(',
             '    symbol) : string']),
    cls('SlotSymbol', kind='enum', col=2, row=4,
        attrs=['Sackpot', 'Awesome', 'Coin', 'Star',
               'Bar', 'Stash', 'Ampersand']),
]
game_model_edges = [
    Edge('GamblingFacade', 'Orchestrator', 'uses', 'runs'),
    Edge('Orchestrator', 'SlotMachineController', 'uses', '«create»'),
    Edge('SlotMachineController', 'SlotMachine', 'assoc', 'drives'),
    Edge('SlotMachineController', 'IBankInterface', 'uses', 'bank handoff'),
    Edge('BankBridge', 'IBankInterface', 'implements'),
    Edge('SlotMachine', 'GameState', 'assoc', 'State'),
    Edge('GameState', 'SpinResult', 'assoc', 'SpinResult'),
    Edge('SlotMachine', 'IPayoutEvaluator', 'assoc', 'evaluates via'),
    Edge('PayoutEvaluator', 'IPayoutEvaluator', 'implements'),
    Edge('PayoutEvaluator', 'PayoutTable', 'assoc'),
    Edge('PayoutTable', 'PayoutEntry', 'composes', '7  Entries'),
    Edge('PayoutEntry', 'SlotSymbol', 'assoc', 'Symbol'),
    Edge('SlotMachine', 'Reel', 'composes', '3  reels'),
    Edge('SlotMachine', 'GameLoop', 'assoc', 'starts/stops'),
    Edge('Reel', 'SymbolCatalog', 'assoc', 'rolls from'),
    Edge('SymbolCatalog', 'SlotSymbol', 'assoc', 'maps'),
]
add('07-game-model', 'Pull The Sack — game model (facade, machine, payout)', 'class',
    (game_model_nodes, game_model_edges), '''
classDiagram
direction LR
class GamblingFacade {
  +GambleOnLoan(loan) IBankInterface
}
class Orchestrator {
  +Start(loanAmount) SlotMachineController
}
class SlotMachineController {
  <<global namespace>>
  -machine : SlotMachine
  -updater : IViewUpdater
  -bank : IBankInterface
  -userInput : UserInput
  -viewRegistry : ViewRegistry
  +PullTheSack() IBankInterface
  -RunSession() IBankInterface
  -StateControl(gameState)
  -ServeViewToUpdater(type)
  -RemoveViewFromUpdater(type)
  -ClearAllViews()
}
class IBankInterface {
  <<interface>>
  +GetLoanAmount() decimal
  +SetLoanAmount(amount)
}
class BankBridge {
  -_loanAmount : decimal
  +GetLoanAmount() decimal
  +SetLoanAmount(amount)
}
class GameState {
  <<record struct>>
  +GamePhase : GamePhase
  +Balance : decimal
  +SpinCount : int
  +SpinResult : SpinResult
}
class SpinResult {
  <<record>>
  +Symbols : IReadOnlyList~SlotSymbol~
  +PayoutMultiplier : decimal
}
class IPayoutEvaluator {
  <<interface>>
  +Evaluate(symbols) decimal
}
class PayoutEvaluator {
  -table : PayoutTable
  +Evaluate(symbols) decimal
}
class PayoutTable {
  +Entries : IReadOnlyList~PayoutEntry~
  +GetPayoutValue(symbol) decimal
  -GenerateTable()
}
class PayoutEntry {
  <<record struct>>
  +Symbol : SlotSymbol
  +Multiplier : decimal
}
class SlotMachine {
  +State : GameState
  +StartGame()
  +Spin()
  +ResolveSpinResult()
  +Apply()
  +EndGame()
  -UpdateGamePhase(phase)
}
class GameLoop {
  -_glts : CancellationTokenSource
  -_gameTime : Stopwatch
  -updater : IViewUpdater
  +StartGameLoopAsync()
  +StopGameLoopAsync()
  -RunAsync(...) Task
}
class Reel {
  -_catalog : SymbolCatalog
  -rnd : Random
  +CurrentSymbol : SlotSymbol
  +Spin()
}
class SymbolCatalog {
  +Symbols : IReadOnlyDictionary~int, SlotSymbol~
  +SymbolGraphic : IReadOnlyDictionary
  +GetSymbolGraphicNullCheck(symbol) string
}
class SlotSymbol {
  <<enumeration>>
  Sackpot
  Awesome
  Coin
  Star
  Bar
  Stash
  Ampersand
}
GamblingFacade ..> Orchestrator : runs
Orchestrator ..> SlotMachineController : create
SlotMachineController --> SlotMachine : drives
SlotMachineController ..> IBankInterface : bank handoff
BankBridge ..|> IBankInterface
SlotMachine --> GameState : State
GameState --> SpinResult : SpinResult
SlotMachine --> IPayoutEvaluator : evaluates via
PayoutEvaluator ..|> IPayoutEvaluator
PayoutEvaluator --> PayoutTable
PayoutTable "1" *-- "7" PayoutEntry : Entries
PayoutEntry --> SlotSymbol : Symbol
SlotMachine "1" *-- "3" Reel : reels
SlotMachine --> GameLoop : starts/stops
Reel --> SymbolCatalog : rolls from
SymbolCatalog --> SlotSymbol : maps
''')

# =====================================================================
# 08 — Game views & console (Pull The Sack)
# =====================================================================
game_view_nodes = [
    cls('IView', kind='interface', col=0, row=0,
        attrs=['+ HasBeenServed : bool',
               '+ GameState : GameState',
               '+ ViewState : ViewState'],
        ops=['+ Render(console, dt)',
             '+ ResetView()']),
    cls('SplashView', kind='class', col=1, row=0, stereo='«IView»',
        attrs=['- config : SplashViewConfig',
               '- currentFrame : int'],
        ops=['+ Render(console, dt)', '+ ResetView()'],
        notes=['4-frame ASCII animation loop']),
    cls('GameView', kind='class', col=2, row=0, stereo='«IView»',
        attrs=['- config : GameViewConfig',
               '- reels : List<ReelView>',
               '- blinkGraphic : bool  ...'],
        ops=['+ Render(console, dt)', '+ ResetView()',
             '- RenderSpinning(console, dt)',
             '- Resolve()',
             '- OnReelResolveComplete()',
             '- EvaluateCompleteText() : string'],
        notes=['whole frame inside console.Lock();',
               'subscribes last reel\'s',
               'OnResolveComplete callback']),
    cls('ReelView', kind='class', col=3, row=0, stereo='«IView»',
        attrs=['+ OnResolveComplete : Action?',
             '- resolveComplete : bool',
             '- sequenceSpeed : float  ...'],
        ops=['+ Target(target, len,',
             '    speed, decay)',
             '+ Render(console, dt)', '+ ResetView()'],
        notes=['scroll + resolve slowdown;',
               'rendered only via GameView']),
    cls('ResultView', kind='class', col=4, row=0, stereo='«IView»',
        attrs=['- config : ResultViewConfig',
               '- viewIndex : int'],
        ops=['+ Render(console, dt)', '+ ResetView()'],
        notes=['outcome screen; Complete on',
               '5th render (input debounce)']),
    cls('IViewUpdater', kind='interface', col=0, row=1,
        ops=['+ Register(view)', '+ DeRegister(view)',
             '+ Tick(deltaTime)']),
    cls('UserInput', kind='class', col=1, row=1,
        attrs=['- console : IConsole'],
        ops=['+ ReadKeyCode(key)',
             '+ TryReadKeyCode(key) : bool'],
        notes=['blocking wait + per-tick polling']),
    cls('IConsole', kind='interface', col=2, row=1,
        attrs=['+ Lock() : IDisposable',
             '+ TryReadKey(out key) : bool',
             '+ Write / WriteLine /',
             '    WriteCentered(text)',
             '+ SetCursor(left, top)',
             '+ SetConsoleColor(color)',
             '+ ReadKey() : ConsoleKeyInfo']),
    cls('LockedConsole', kind='class', col=3, row=1,
        attrs=['- _gate : object'],
        ops=['+ Lock() : IDisposable',
             '+ TryReadKey(out key) : bool'],
        notes=['lock around every console op;',
               'ReadKey deliberately unlocked',
               '(anti-flicker workaround)']),
    cls('BalanceView', kind='class', col=4, row=1, stereo='«IView»',
        attrs=['+ Target : decimal',
               '- _displayed : decimal'],
        ops=['+ Render(console, dt)  (empty)'],
        notes=['constructed but never',
               'registered — dead code']),
    cls('ViewUpdater', kind='class', col=0, row=2,
        attrs=['- _console : IConsole',
               '- _views : List<IView>'],
        ops=['+ Register(view)', '+ DeRegister(view)',
             '+ Tick(dt)  — renders all']),
    cls('ViewRegistry', kind='class', col=1, row=2,
        attrs=['- _views : Dictionary<ViewType,',
             '    IView>'],
        ops=['+ AddView(type, view)',
             '+ RemoveView(type) : bool',
             '+ GetView(type) : IView',
             '+ AllViews : IEnumerable<IView>']),
    cls('ViewConfig', kind='record', col=3, row=2, stereo='«abstract record»',
        attrs=['+ Width : int = 60', '+ Height : int = 20',
               '+ Title : string']),
    cls('SplashViewConfig', kind='record', col=2, row=3,
        attrs=['+ SponsorText : string',
               '+ Frames : 4 ASCII frames']),
    cls('GameViewConfig', kind='record', col=3, row=3,
        attrs=['+ ReelCols/Rows/Width/Gap/Top',
               '+ ResolveSlowDownSteps',
               '+ Banner, SponsorText, TopArt,',
               '    BottomArt, CompleteText_*']),
    cls('ResultViewConfig', kind='record', col=4, row=3,
        attrs=['+ LogoArt : string',
               '+ HoldSeconds (unused)']),
]
game_view_edges = [
    Edge('ViewUpdater', 'IViewUpdater', 'implements'),
    Edge('LockedConsole', 'IConsole', 'implements'),
    Edge('UserInput', 'IConsole', 'assoc', 'reads via'),
    Edge('GameView', 'ReelView', 'composes', '3  positions'),
    Edge('SplashViewConfig', 'ViewConfig', 'extends'),
    Edge('GameViewConfig', 'ViewConfig', 'extends'),
    Edge('ResultViewConfig', 'ViewConfig', 'extends'),
]
add('08-game-views', 'Pull The Sack — views, console & input', 'class',
    (game_view_nodes, game_view_edges), '''
classDiagram
direction LR
class IView {
  <<interface>>
  +HasBeenServed : bool
  +GameState : GameState
  +ViewState : ViewState
  +Render(console, dt)
  +ResetView()
}
class SplashView {
  -config : SplashViewConfig
  -currentFrame : int
  +Render(console, dt)
  +ResetView()
}
class GameView {
  -config : GameViewConfig
  -reels : List~ReelView~
  -blinkGraphic : bool
  +Render(console, dt)
  +ResetView()
  -RenderSpinning(console, dt)
  -Resolve()
  -OnReelResolveComplete()
  -EvaluateCompleteText() string
}
class ReelView {
  +OnResolveComplete : Action
  -resolveComplete : bool
  -sequenceSpeed : float
  +Target(target, len, speed, decay)
  +Render(console, dt)
  +ResetView()
}
class ResultView {
  -config : ResultViewConfig
  -viewIndex : int
  +Render(console, dt)
  +ResetView()
}
class BalanceView {
  +Target : decimal
  -_displayed : decimal
  +Render(console, dt)
}
class IViewUpdater {
  <<interface>>
  +Register(view)
  +DeRegister(view)
  +Tick(deltaTime)
}
class ViewUpdater {
  -_console : IConsole
  -_views : List~IView~
  +Register(view)
  +DeRegister(view)
  +Tick(dt)
}
class UserInput {
  -console : IConsole
  +ReadKeyCode(key)
  +TryReadKeyCode(key) bool
}
class IConsole {
  <<interface>>
  +Lock() IDisposable
  +TryReadKey(out key) bool
  +Write(text)
  +WriteLine(text)
  +WriteCentered(text)
  +SetCursor(left, top)
  +SetConsoleColor(color)
  +ReadKey() ConsoleKeyInfo
}
class LockedConsole {
  -_gate : object
  +Lock() IDisposable
  +TryReadKey(out key) bool
}
class ViewRegistry {
  -_views : Dictionary~ViewType, IView~
  +AddView(type, view)
  +RemoveView(type) bool
  +GetView(type) IView
  +AllViews : IEnumerable
}
class ViewConfig {
  <<abstract record>>
  +Width : int
  +Height : int
  +Title : string
}
class SplashViewConfig {
  +SponsorText : string
  +Frames : 4 ASCII frames
}
class GameViewConfig {
  +ReelCols
  +ReelRows
  +ReelWidth
  +ReelGap
  +ReelTop
  +ResolveSlowDownSteps
  +Banner
  +CompleteText_Lost
  +CompleteText_Won
  +CompleteText_SackPot
}
class ResultViewConfig {
  +LogoArt : string
  +HoldSeconds
}
SplashView ..|> IView
GameView ..|> IView
ReelView ..|> IView
ResultView ..|> IView
BalanceView ..|> IView
ViewUpdater ..|> IViewUpdater
LockedConsole ..|> IConsole
UserInput --> IConsole : reads via
GameView "1" *-- "3" ReelView : positions
SplashViewConfig --|> ViewConfig
GameViewConfig --|> ViewConfig
ResultViewConfig --|> ViewConfig
''')

# =====================================================================
# Sequence — game session (spin, resolve, payout)
# =====================================================================
add('seq-game', 'Sequence — Pull The Sack game session', 'seq',
    dict(
        participants=[
            ('p', 'Player', 'actor'),
            ('ctl', 'SlotMachineController', None),
            ('sm', 'SlotMachine', None),
            ('gl', 'GameLoop', None),
            ('vu', 'ViewUpdater', None),
            ('gv', 'GameView', None),
            ('rl', 'Reels (x3)', None),
            ('pe', 'PayoutEvaluator', None),
            ('bb', 'BankBridge', '«IBankInterface»'),
        ],
        items=[
            ('msg', 'p', 'ctl', 'GambleOnLoan(loan) via facade', 'call'),
            ('msg', 'ctl', 'sm', 'StartGame() — phase = Splash', 'call'),
            ('msg', 'sm', 'gl', 'StartGameLoopAsync()', 'call'),
            ('frame', 'loop', [
                ('every 500 ms (render Task)', [
                    ('msg', 'gl', 'vu', 'Tick(deltaTime)', 'call'),
                    ('msg', 'vu', 'gv', 'Render(console, dt)', 'call'),
                ]),
            ]),
            ('note', 'ctl', 'UserInput.ReadKeyCode(Spacebar) — blocks while SplashView animates'),
            ('msg', 'ctl', 'sm', 'Spin() — phase = Spinning', 'call'),
            ('msg', 'sm', 'rl', 'Spin() on each reel', 'call'),
            ('note', 'ctl', 'space polled with TryReadKeyCode while reels scroll'),
            ('msg', 'ctl', 'sm', 'ResolveSpinResult()', 'call'),
            ('msg', 'rl', 'sm', 'CurrentSymbol x 3', 'return'),
            ('msg', 'sm', 'pe', 'Evaluate(symbols)', 'call'),
            ('msg', 'pe', 'sm', 'PayoutMultiplier', 'return'),
            ('self', 'sm', 'State = with { SpinResult, phase = Resolving }'),
            ('note', 'gv', 'Resolve(): reel slowdown with stagger, payline blink,ViewState = Complete'),
            ('note', 'ctl', 'space pressed (ViewState = Complete)'),
            ('msg', 'ctl', 'sm', 'Apply() — phase = Result', 'call'),
            ('note', 'ctl', 'ResultView shows loan vs payout, Complete on 5th render'),
            ('note', 'ctl', 'space pressed again'),
            ('msg', 'ctl', 'bb', 'SetLoanAmount(423424) — hardcoded placeholder', 'call'),
            ('msg', 'ctl', 'sm', 'EndGame() — phase = Exiting', 'call'),
            ('msg', 'sm', 'gl', 'StopGameLoopAsync()', 'call'),
            ('msg', 'ctl', 'p', 'returns IBankInterface', 'return'),
        ],
    ), '''
sequenceDiagram
actor Player
participant SlotMachineController
participant SlotMachine
participant GameLoop
participant ViewUpdater
participant GameView
participant Reels as "Reels (x3)"
participant PayoutEvaluator
participant BankBridge as "BankBridge «IBankInterface»"
Player->>SlotMachineController: GambleOnLoan(loan) via facade
SlotMachineController->>SlotMachine: StartGame() - phase = Splash
SlotMachine->>GameLoop: StartGameLoopAsync()
loop every 500 ms (render Task)
  GameLoop->>ViewUpdater: Tick(deltaTime)
  ViewUpdater->>GameView: Render(console, dt)
end
Note over SlotMachineController: UserInput.ReadKeyCode(Spacebar) - blocks<br/>while SplashView animates
SlotMachineController->>SlotMachine: Spin() - phase = Spinning
SlotMachine->>Reels: Spin() on each reel
Note over SlotMachineController: space polled with TryReadKeyCode<br/>while reels scroll
SlotMachineController->>SlotMachine: ResolveSpinResult()
Reels-->>SlotMachine: CurrentSymbol x 3
SlotMachine->>PayoutEvaluator: Evaluate(symbols)
PayoutEvaluator-->>SlotMachine: PayoutMultiplier
SlotMachine->>SlotMachine: State = with { SpinResult, phase = Resolving }
Note over GameView: Resolve(): reel slowdown with stagger,<br/>payline blink, ViewState = Complete
Note over SlotMachineController: space pressed (ViewState = Complete)
SlotMachineController->>SlotMachine: Apply() - phase = Result
Note over SlotMachineController: ResultView shows loan vs payout,<br/>Complete on 5th render
Note over SlotMachineController: space pressed again
SlotMachineController->>BankBridge: SetLoanAmount(423424) - hardcoded placeholder
SlotMachineController->>SlotMachine: EndGame() - phase = Exiting
SlotMachine->>GameLoop: StopGameLoopAsync()
SlotMachineController-->>Player: returns IBankInterface
''')

# =====================================================================
# Sequence — login with lockout
# =====================================================================
add('seq-login', 'Sequence — Login with 3-strike lockout', 'seq',
    dict(
        participants=[
            ('c', 'Customer', 'actor'),
            ('m', 'Menu', None),
            ('a', 'AuthService', None),
            ('u', 'UserService', None),
            ('ur', 'User', '«entity»'),
        ],
        items=[
            ('msg', 'c', 'm', 'starts app (LoginMenu)', 'call'),
            ('msg', 'm', 'a', 'Login(personnummer, password)', 'call'),
            ('msg', 'a', 'u', 'GetUserByUsername(username)', 'call'),
            ('msg', 'u', 'a', 'User?', 'return'),
            ('frame', 'alt', [
                ('[unknown user or wrong password]', [
                    ('msg', 'a', 'ur', 'RegisterFailedLogin()', 'call'),
                    ('msg', 'ur', 'a', 'FailedLoginAttempts++', 'return'),
                    ('frame', 'opt', [
                        ('[attempts >= 3]', [
                            ('self', 'ur', 'IsLockedOut = true'),
                            ('msg', 'a', 'm', 'LockedOut', 'return'),
                            ('msg', 'm', 'c', '"account locked" — exit', 'return'),
                        ]),
                    ]),
                    ('msg', 'a', 'm', 'WrongCredentials', 'return'),
                ]),
                ('[user locked out]', [
                    ('msg', 'a', 'm', 'LockedOut', 'return'),
                ]),
                ('[correct password]', [
                    ('msg', 'a', 'ur', 'FailedLoginAttempts = 0', 'call'),
                    ('self', 'a', 'LoggedInUser = user'),
                    ('msg', 'a', 'm', 'Success', 'return'),
                    ('frame', 'alt', [
                        ('[admin or owner]', [
                            ('msg', 'm', 'm', 'ShowAdminMenu(userId)', 'call'),
                        ]),
                        ('[customer]', [
                            ('msg', 'm', 'm', 'ShowUserMenu()', 'call'),
                        ]),
                    ]),
                ]),
            ]),
        ],
    ), '''
sequenceDiagram
actor Customer
participant Menu
participant AuthService
participant UserService
participant User as "User «entity»"
Customer->>Menu: starts app (LoginMenu)
Menu->>AuthService: Login(personnummer, password)
AuthService->>UserService: GetUserByUsername(username)
UserService-->>AuthService: User?
alt unknown user or wrong password
  AuthService->>User: RegisterFailedLogin()
  User-->>AuthService: FailedLoginAttempts++
  opt attempts >= 3
    User->>User: IsLockedOut = true
    AuthService-->>Menu: LockedOut
    Menu-->>Customer: "account locked" - exit
  end
  AuthService-->>Menu: WrongCredentials
else user locked out
  AuthService-->>Menu: LockedOut
else correct password
  AuthService->>User: FailedLoginAttempts = 0
  AuthService->>AuthService: LoggedInUser = user
  AuthService-->>Menu: Success
  alt admin or owner
    Menu->>Menu: ShowAdminMenu(userId)
  else customer
    Menu->>Menu: ShowUserMenu()
  end
end
''')

# =====================================================================
# Sequence — loan application & decision
# =====================================================================
add('seq-loan', 'Sequence — Loan application, pricing & decision', 'seq',
    dict(
        participants=[
            ('c', 'Customer', 'actor'),
            ('lm', 'LoanMenu', None),
            ('ls', 'LoanService', None),
            ('am', 'AccountManager', None),
            ('cs', 'CreditScoreService', None),
            ('ic', 'InterestCalculator', None),
            ('ir', 'InterestRuleService', None),
        ],
        items=[
            ('msg', 'c', 'lm', '"Apply for a loan"', 'call'),
            ('msg', 'lm', 'am', 'GetTotalBalance(userId)', 'call'),
            ('msg', 'am', 'lm', 'totalBalance', 'return'),
            ('msg', 'lm', 'ls', 'ApplyForLoan(userId, amount, duration, balance)', 'call'),
            ('self', 'ls', 'eligibility: balance*5 - committed loans'),
            ('msg', 'ls', 'cs', 'GetCreditScore(userId)', 'call'),
            ('msg', 'cs', 'ls', 'creditScore (default 100)', 'return'),
            ('msg', 'ls', 'ic', 'GetInterestRate(duration, score)', 'call'),
            ('msg', 'ic', 'ir', 'GetInterestAdjustment(score)', 'call'),
            ('msg', 'ir', 'ic', 'adjustment', 'return'),
            ('msg', 'ic', 'ls', 'interestRate', 'return'),
            ('msg', 'ls', 'ic', 'CalculateTotalInterest(...)', 'call'),
            ('msg', 'ic', 'ls', 'totalInterest', 'return'),
            ('self', 'ls', 'new Loan(amount) — Status = Pending'),
            ('msg', 'ls', 'lm', 'Loan?', 'return'),
            ('msg', 'lm', 'c', 'loan terms (rate, interest, repayment)', 'return'),
            ('frame', 'opt', [
                ('[later: decision on the loan]', [
                    ('msg', 'lm', 'ls', 'ChangeLoanStatus(loanId, status, balance)', 'call'),
                    ('msg', 'ls', 'am', 'GetTotalBalance(loan.UserId)', 'call'),
                    ('msg', 'am', 'ls', 'totalBalance', 'return'),
                    ('self', 'ls', 're-check borrowing limit (5x rule)'),
                    ('msg', 'ls', 'ls', 'loan.UpdateStatus(status)', 'call'),
                    ('frame', 'opt', [
                        ('[approved]', [
                            ('self', 'ls', 'loan.StartRepaymentSchedule(today)'),
                        ]),
                    ]),
                    ('msg', 'ls', 'lm', 'bool', 'return'),
                ]),
            ]),
        ],
    ), '''
sequenceDiagram
actor Customer
participant LoanMenu
participant LoanService
participant AccountManager
participant CreditScoreService
participant InterestCalculator
participant InterestRuleService
Customer->>LoanMenu: "Apply for a loan"
LoanMenu->>AccountManager: GetTotalBalance(userId)
AccountManager-->>LoanMenu: totalBalance
LoanMenu->>LoanService: ApplyForLoan(userId, amount, duration, balance)
LoanService->>LoanService: eligibility: balance*5 - committed loans
LoanService->>CreditScoreService: GetCreditScore(userId)
CreditScoreService-->>LoanService: creditScore (default 100)
LoanService->>InterestCalculator: GetInterestRate(duration, score)
InterestCalculator->>InterestRuleService: GetInterestAdjustment(score)
InterestRuleService-->>InterestCalculator: adjustment
InterestCalculator-->>LoanService: interestRate
LoanService->>InterestCalculator: CalculateTotalInterest(...)
InterestCalculator-->>LoanService: totalInterest
LoanService->>LoanService: new Loan(amount) - Status = Pending
LoanService-->>LoanMenu: Loan?
LoanMenu-->>Customer: loan terms (rate, interest, repayment)
opt later: decision on the loan
  LoanMenu->>LoanService: ChangeLoanStatus(loanId, status, balance)
  LoanService->>AccountManager: GetTotalBalance(loan.UserId)
  AccountManager-->>LoanService: totalBalance
  LoanService->>LoanService: re-check borrowing limit (5x rule)
  LoanService->>LoanService: loan.UpdateStatus(status)
  opt approved
    LoanService->>LoanService: loan.StartRepaymentSchedule(today)
  end
  LoanService-->>LoanMenu: bool
end
''')

# =====================================================================
# Sequence — transfer & scheduled batch execution
# =====================================================================
add('seq-transfer', 'Sequence — Transfer staging & scheduled execution', 'seq',
    dict(
        participants=[
            ('c', 'Customer', 'actor'),
            ('bt', 'BankTransfer', '«API»'),
            ('bx', 'BankTransaction', '«Controller»'),
            ('er', 'ExchangeRate', '«API»'),
            ('st', 'StageTransaction', None),
            ('tr', 'TransactionRegistry', None),
            ('ts', 'TransactionScheduler', None),
        ],
        items=[
            ('msg', 'c', 'bt', 'TransferMoney(origin, target, amount, currency)', 'call'),
            ('msg', 'bt', 'bx', 'TransferMoney(...)', 'call'),
            ('msg', 'bx', 'er', 'TryCalculateExchangeValue(from, to, amount)', 'call'),
            ('msg', 'er', 'bx', 'StatusCode + converted amount', 'return'),
            ('msg', 'bx', 'st', 'Stage(target, deposit, Deposit)', 'call'),
            ('msg', 'st', 'tr', 'RegisterStagedTransaction(TDO)', 'call'),
            ('msg', 'bx', 'st', 'Stage(origin, withdrawal, Withdrawal)', 'call'),
            ('msg', 'st', 'tr', 'RegisterStagedTransaction(TDO)', 'call'),
            ('msg', 'bx', 'bt', 'StatusCode', 'return'),
            ('msg', 'bt', 'c', 'StatusCode (staged, not yet posted)', 'return'),
            ('frame', 'loop', [
                ('[every 15 min (Task.Delay tick)]', [
                    ('msg', 'ts', 'st', 'OnSchedule()', 'call'),
                    ('msg', 'st', 'tr', 'GetAllStagedTransactions()', 'call'),
                    ('msg', 'tr', 'st', 'List<TransactionDataObject>', 'return'),
                    ('self', 'st', 'post deposits/withdrawals (TODO)'),
                    ('msg', 'st', 'tr', 'ClearStagedTransactions()', 'call'),
                ]),
            ]),
        ],
    ), '''
sequenceDiagram
actor Customer
participant BankTransfer as "BankTransfer «API»"
participant BankTransaction as "BankTransaction «Controller»"
participant ExchangeRate as "ExchangeRate «API»"
participant StageTransaction
participant TransactionRegistry
participant TransactionScheduler
Customer->>BankTransfer: TransferMoney(origin, target, amount, currency)
BankTransfer->>BankTransaction: TransferMoney(...)
BankTransaction->>ExchangeRate: TryCalculateExchangeValue(from, to, amount)
ExchangeRate-->>BankTransaction: StatusCode + converted amount
BankTransaction->>StageTransaction: Stage(target, deposit, Deposit)
StageTransaction->>TransactionRegistry: RegisterStagedTransaction(TDO)
BankTransaction->>StageTransaction: Stage(origin, withdrawal, Withdrawal)
StageTransaction->>TransactionRegistry: RegisterStagedTransaction(TDO)
BankTransaction-->>BankTransfer: StatusCode
BankTransfer-->>Customer: StatusCode (staged, not yet posted)
loop every 15 min (Task.Delay tick)
  TransactionScheduler->>StageTransaction: OnSchedule()
  StageTransaction->>TransactionRegistry: GetAllStagedTransactions()
  TransactionRegistry-->>StageTransaction: List~TransactionDataObject~
  StageTransaction->>StageTransaction: post deposits/withdrawals (TODO)
  StageTransaction->>TransactionRegistry: ClearStagedTransactions()
end
''')


def render_all():
    results = []
    for d in DIAGRAMS:
        base = os.path.join(OUT, d['name'])
        if d['kind'] == 'class':
            nodes, edges = d['data']
            render_class_diagram(base + '.png', nodes, edges, title=d['title'])
        else:
            p = d['data']['participants']
            it = d['data']['items']
            render_sequence(base + '.png', p, it, title=d['title'])
        with open(base + '.mmd', 'w', encoding='utf-8') as f:
            f.write(d['mmd'].strip() + '\n')
        results.append(base + '.png')
    return results


if __name__ == '__main__':
    for f in render_all():
        print('wrote', f)
