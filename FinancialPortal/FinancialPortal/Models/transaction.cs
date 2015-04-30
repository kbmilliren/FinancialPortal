using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int Amount { get; set; }
        public int AbsAmount { get; set; }
        public int ReconciledAmount { get; set; }
        public int AbsReconciledAmount { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; }
        public Nullable <DateTimeOffset> Updated { get; set; }
        public int UpdatedByUserId { get; set; }
        public int CategoryId { get; set; }
    } 
}