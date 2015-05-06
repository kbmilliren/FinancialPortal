﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public string Name { get; set; }

        public virtual Household Household { get; set; }
        public ICollection<Transaction> Transaction { get; set; }
        public ICollection<BudgetItem> BudgetItem { get; set; }

        public Category()
        {
            Transaction = new HashSet<Transaction>();
            BudgetItem = new HashSet<BudgetItem>();
        }
    }
}