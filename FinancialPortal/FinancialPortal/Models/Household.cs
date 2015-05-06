using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class Household
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
        public virtual ICollection<HouseholdAccount> Accounts { get; set; }
        public virtual ICollection<Invitation> Invitations { get; set; }
        public virtual ICollection<Category> Categories { get; set; }

        public Household()
        {
            Users = new HashSet<ApplicationUser>();
            BudgetItems = new HashSet<BudgetItem>();
            Accounts = new HashSet<HouseholdAccount>();
            Invitations = new HashSet<Invitation>();
            Categories = new HashSet<Category>();

        }
    }
}