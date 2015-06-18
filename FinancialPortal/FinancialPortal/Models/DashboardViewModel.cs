using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<HouseholdAccount> HouseholdAccounts { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
        public IEnumerable<ApplicationUser> User { get; set; }

       

    }
}