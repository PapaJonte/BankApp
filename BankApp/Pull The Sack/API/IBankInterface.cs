namespace BankApp.Pull_The_Sack.API;


public interface IBankInterface
{
    decimal GetLoanAmount();

    void SetLoanAmount(decimal amount);
}