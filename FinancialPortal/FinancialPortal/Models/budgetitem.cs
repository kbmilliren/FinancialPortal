using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class Budgetitem
    {
        public int Id { get; set; }
        public string Household { get; set; }
        public int CategoryId { get; set; }
        public int Amount { get; set; }
    }
}