using BankApp.Jonathan;
using BankApp.Robin;

namespace BankApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //User user = new User();
            
            // SERVICES
            UserService userService = new UserService();
            AuthService authService = new AuthService(userService);
            InterestRuleService interestRuleService = new InterestRuleService();
            CreditScoreService creditScoreService = new CreditScoreService();
            LoanService loanService = new LoanService(interestRuleService, creditScoreService);

            Menu menu = new Menu(userService, authService);

            menu.LoginMenu();
        }
    }
}
