# BankApp

SavingsAccount
Not to be confused with BankAccount!
SavingsAccount as a class serves the purpose primarily of storing the balance of accounts. Because this class is intended to have many almost completely identical variations in it's objects there is a constructor provided for assigning values to it's properties.

The properties include "Balance", "Interest", "SavingsAccountNumber". Savings account number exists for the purpose of identifying such accounts later when transactions must be made, for there is supposed to be a multitude of savings accounts. Meanwhile Balance, and Interest serve a more obvious purpose.

The majority of methods are dedicated to returning the various different properties of the class. An exception to this rule is one method dedicated to reducing or increasing balance, for transaction purposes.

BankAccount
Not to be confused with SavingsAccount!
The BankAccount serves to contain SavingsAccounts, and sort them by user ID. Hence why it has both a list and a dictionary dedicated to storing objects of the SavingsAccount class.

AccountManager