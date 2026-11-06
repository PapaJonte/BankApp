using BankApp.Jonathan;
using BankApp.Transaction_Layer.API;
using BankApp.GustafsClasses;
using System;
using System.Collections.Generic;
using System.Text;
using BankApp.Transaction_Layer.Interface;
using BankApp.Transaction_Layer.DataModel;
using BankApp.Transaction_Layer.Enum;

namespace BankApp.Robin
{
    /// <summary>
    /// Customer menu for applying, viewing loan details and preparing repayments
    /// </summary>
    internal class LoanMenu
    {
        private readonly LoanService loanService;
        private readonly AccountManager accountManager;
        private readonly IViewTransactionStatisticBridge bankTransferStatistics;
        private readonly BankTransfer bankTransfer;
        private readonly ExchangeRate exchangeRates;

        // Constructor
        public LoanMenu(LoanService loanService, AccountManager accountManager, IViewTransactionStatisticBridge bankTransferStatistics, BankTransfer bankTransfer, ExchangeRate exchangeRates)
        {
            this.loanService = loanService;
            this.accountManager = accountManager;
            this.bankTransferStatistics = bankTransferStatistics;
            this.bankTransfer = bankTransfer;
            this.exchangeRates = exchangeRates;
        }

        // Handles loan applications
        public Loan? ApplyForLoan(int userId, decimal requestedAmount, int duration)
        {
            // Retrieve all bank accounts belonging to the customer
            List<BankAccount> customerAccounts = accountManager.BankAccountList
                .Where(account => account.GetUserId() == userId)
                .ToList();

            // Check whether the customer has any available accounts
            if (customerAccounts.Count == 0)
            {
                Console.WriteLine("You have no bank accounts available.");
                return null;
            }

            // List the customer's payout account and their currencies
            Console.WriteLine("\nSelect an account to receive your loan:");

            foreach (BankAccount account in customerAccounts)
            {
                Console.WriteLine($"Account number: {account.GetBankAccountNumber()} ({account.GetCurrency()})");
            }

            // Ask the customer which account should receive the loan
            Console.WriteLine("Enter the destination account number: ");

            if (!int.TryParse(Console.ReadLine(), out int selectedAccountNumber))
            {
                Console.WriteLine("Invalid account number.");
                return null;
            }

            // Verify that the selected account belongs to the customer
            if (!accountManager.AccountOwnershipVerification(userId, selectedAccountNumber))
            {
                Console.WriteLine("The selected account does not belong to you.");
                return null;
            }

            // Gets the customers combined total balance
            decimal totalAccountBalance;

            try
            {
                totalAccountBalance = accountManager.GetTotalBalance(userId);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Your total balance could not be calculated because an exchange rate is unavailable. Please try again later.");
                return null;
            }

            // Convert the account number to a string for the transaction system
            string destinationAccountNumber = selectedAccountNumber.ToString();

            // Validates and calculates interest etc.
            Loan? loan = loanService.ApplyForLoan(userId, requestedAmount, duration, totalAccountBalance, destinationAccountNumber);

            // If the loan application is rejected, LoanService returns null
            if (loan == null)
            {
                Console.WriteLine("Loan application rejected.");
                return null;
            }

            // Display the loan details
            Console.WriteLine($"Application submitted.\nLoan ID: {loan.LoanId}\nStatus: {loan.Status}\nLoan amount: {loan.LoanAmount} SEK\n" +
                $"Term: {loan.Duration} months\nInterest rate: {loan.InterestRate}%\nTotal interest over the loan term: {loan.TotalInterest:N2} SEK\n" +
                $"Total repayment: {loan.TotalRepayment:N2} SEK");
            return loan;
        }

        // Repayment amount and source account
        private void PrepareLoanRepayment(int userId, int loanId)
        {
            // Only show payment details for a loan owned by this customer
            Loan? loan = loanService.GetCustomersLoan(userId).Find(item => item.LoanId == loanId);

            if (loan == null)
            {
                Console.WriteLine("Loan not found.");
                return;
            }

            if (loan.NextPaymentDate is not DateTime dueDate)
            {
                Console.WriteLine("There is no scheduled payment for this loan.");
                return;
            }

            // The repay amount includes any fees on the current installment
            decimal paymentAmount = loanService.GetMonthlyPayment(loanId) + loan.LateFees;

            if (loan.LateBillDeadLine is DateTime deadline)
            {
                Console.WriteLine($"Late payment. Pay {paymentAmount:N2} SEK by {deadline:yy-MM-dd}.");
            }
            else
            {
                Console.WriteLine($"Installment due on {dueDate:yy-MM-dd}: {paymentAmount:N2} SEK");
            }

            // List only regular bank accounts owned by this customer
            var accounts = accountManager.BankAccountList
                .Where(account => account.GetUserId() == userId)
                .ToList();

            if (accounts.Count == 0)
            {
                Console.WriteLine("You have no regular accounts to pay from.");
                return;
            }

            // Show the customer's regular accounts before asking them to choose
            foreach (BankAccount account in accounts)
            {
                Console.WriteLine($"{account.GetBankAccountNumber()} ({account.GetCurrency()})");
            }

            Console.WriteLine("Enter the account number to pay from: ");
            
            if (!int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Enter a valid accout number.");
                return;
            }

            // Select the account from the signed-in customer's list
            BankAccount? source = accounts.Find(account => account.GetBankAccountNumber() == accountNumber);
            if (source == null)
            {
                Console.WriteLine("Select one of the listed accounts.");
                return;
            }

            // The loan is billed in SEK, but WithdrawMoney expects the account's currency
            Currency accountCurrency = source.GetCurrency();
            decimal debitAmount = paymentAmount;

            if (accountCurrency != Currency.SEK && exchangeRates.TryCalculateExchangeValue(Currency.SEK, accountCurrency, paymentAmount, out debitAmount)
                != StatusCode.Successful)
            {
                Console.WriteLine("The payment could not be converted to the account currency.");
                return;
            }

            if (debitAmount <= 0m)
            {
                Console.WriteLine("There is no valid payment amount to withdraw.");
                return;
            }

            AccountData sourceAccount = new(userId, accountNumber, accountCurrency);
            DateTime submittedDate = DateTime.Today;

            if (bankTransfer.WithdrawMoney(sourceAccount, debitAmount) != StatusCode.Successful)
            {
                Console.WriteLine("The withdrawal failed. The installment is still due.");
                return;
            }

            // Record the original SEK amount after the direct withdrawal succeeds
            if(!loanService.RepayLoan(loanId, paymentAmount, submittedDate))
            {
                Console.WriteLine("The withdrawal succeeded, but the loan payment could not be recorded. Contact an administrator before retrying.");
                return;
            }

            Console.WriteLine($"Payment of {paymentAmount:N2} SEK recorded.");
        }


        // The actual menu displayed to the customer
        public void ShowLoanMenu(int userId)
        {
            string[] options =
            {
                "Apply for a loan",
                "View my loans",
                "Repay a loan",
                "Return to main menu"
            };

            int selectedOption = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== LOAN MENU =====\n");

                // Displays the options and highlights the selected one
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(options[i]);
                    }
                }

                // Reads the user's input after displaying all options
                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedOption = (selectedOption - 1 + options.Length) % options.Length;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedOption = (selectedOption + 1) % options.Length;
                        break;

                    case ConsoleKey.Escape:
                        return;

                    case ConsoleKey.Enter:
                        Console.Clear();

                        // Refresh overdue bills before handling the selected loan option
                        loanService.UpdateLatePayments(userId, DateTime.Today);

                        switch (selectedOption)
                        {
                            case 0:
                                // Apply for a new loan
                                Console.Write("Enter the loan amount (SEK): ");

                                // Safeguard to positive amounts rounded to two decimal points
                                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0m || amount != decimal.Round(amount, 2))
                                {
                                    Console.WriteLine("Enter a positive amount with at most two decimal points.");
                                    break;
                                }

                                Console.Write("Enter the duration in months (12-144): ");

                                // Safeguard to stay within the available loan duration
                                if (!int.TryParse(Console.ReadLine(), out int duration) || duration < 12 || duration > 144)
                                {
                                    Console.WriteLine("Enter a duration between 12 and 144 months.");
                                    break;
                                }

                                // Each monthly installment must repay at least one öre of principal
                                decimal minimumAmount = duration / 100m;
                                if (amount < minimumAmount)
                                {
                                    Console.WriteLine($"The minimum amount for this term is {minimumAmount:N2} SEK.");
                                    break;
                                }

                                // Sends the loan application for validation and processing
                                ApplyForLoan(userId, amount, duration);
                                break;

                            case 1:
                                // Display all loans belonging to the customer
                                List<Loan> loans = loanService.GetCustomersLoan(userId);

                                if (loans.Count == 0)
                                {
                                    Console.WriteLine("You have no loans.");
                                }
                                else
                                {
                                    foreach (Loan loan in loans)
                                    {
                                        Console.WriteLine($"\nLoan ID: {loan.LoanId}");

                                        // Show paid off once the final payment clears the remaining principal
                                        string status = loan.RemainingLoanBalance == 0m ? "Paid off" : loan.Status.ToString();
                                        
                                        Console.WriteLine($"Status: {status}");
                                        Console.WriteLine($"Loan amount: {loan.LoanAmount:N2} SEK");
                                        Console.WriteLine($"Remaining balance: {loan.RemainingLoanBalance:N2} SEK");
                                        
                                        // Show the next installment once the repayment schedule has started
                                        if (loan.NextPaymentDate is DateTime dueDate)
                                        {
                                            decimal monthlyPayment = loanService.GetMonthlyPayment(loan.LoanId);
                                            
                                            // Shows the current late bill or the regular installment
                                            if (loan.LateBillDeadLine is DateTime deadline)
                                            {
                                                decimal amountDue = monthlyPayment + loan.LateFees;
                                                Console.WriteLine($"Late payment. Pay {amountDue:N2} SEK by {deadline:yy-MM-dd}.");
                                                Console.WriteLine($"Late fees included: {loan.LateFees:N2} SEK");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"Next payment due: {dueDate:yy-MM-dd}");
                                                Console.WriteLine($"Next installment: {monthlyPayment:N2} SEK");
                                            }
                                        }
                                        Console.WriteLine($"Interest rate: {loan.InterestRate}%");
                                        Console.WriteLine($"Duration: {loan.Duration} months");
                                        Console.WriteLine($"Total interest: {loan.TotalInterest:N2} SEK");
                                        Console.WriteLine($"Total repayment: {loan.TotalRepayment:N2} SEK");
                                        Console.WriteLine("------------------------------");
                                    }
                                }
                                break;

                            case 2:
                                // Validate a repayment request
                                Console.Write("Enter the loan ID: ");

                                if (!int.TryParse(Console.ReadLine(), out int loanId))
                                {
                                    Console.WriteLine("Invalid loan ID.");
                                    break;
                                }

                                // Use the scheduled installment instead of asking for an amount
                                PrepareLoanRepayment(userId, loanId);
                                break;

                            case 3:
                                // Return to the main menu
                                return;
                        }

                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

    }
}