using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Jonathan
{
    /// <summary>
    /// The application decision. Approval and payout confirmation are tracked separately
    /// </summary>
    internal enum LoanStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
