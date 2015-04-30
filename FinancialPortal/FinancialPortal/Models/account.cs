using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Household { get; set; }
        public string Name { get; set; }
        public int Balance { get; set; }
        public int ReconciledBalance { get; set; }

        public virtual ICollection<Transaction> Transaction { get; set; }

        public Account()
        {
            Transaction = new HashSet<Transaction>();
        }

    }


}