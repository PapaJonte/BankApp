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




## Documentation

- [UML diagrams](docs/uml/README.md) — architecture overview, per-module class
  diagrams and sequence diagrams, with editable Mermaid sources.
