using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FinancialPortal.Models;

namespace FinancialPortal.Controllers
{
    [RequireHousehold]
    public class HomeController : Controller

    {
        private ApplicationDbContext db = new ApplicationDbContext();

        
        public ActionResult Index()
        {
        
           var user = db.Users.Find(User.Identity.GetUserId());
           var household = db.Households.Find(user.HouseholdId);

           var model = new DashboardViewModel()
           {
               HouseholdAccounts = household.Accounts.ToList(),
               Transactions = (from account in household.Accounts
                               from transaction in account.Transactions
                               select transaction).ToList()
           };



           return View(model);
        }


        [HttpPost]
        public JsonResult GetChartData()
        {
            var hhId = int.Parse(User.Identity.GetHouseholdId());
            var acctId = db.BudgetItems.FirstOrDefault(a => a.HouseholdId == hhId);

            //DateTime startDate = DateTime.Today;
            //DateTime endDate = DateTime.Today.AddDays(-30);

            var house = db.Households.Find(hhId);



            var endPeriod = System.DateTime.Now.AddDays(31);
            var data =
                (
                    from c in house.Categories
                    select new
                    {
                        Name = c.Name,
                        ActualAmount = (from t in c.Transactions
                                        where t.Date <= endPeriod
                                        select t.AbsAmount).DefaultIfEmpty().Sum(),
                        //ActualAmount = c.Transactions.Where(t=> t.Date >= startDate && t.Date <= endDate).Select(t=> t.Amount).DefaultIfEmpty().Sum(),
                        BudgetAmount = c.BudgetItem.Select(t => t.Amount).DefaultIfEmpty().Sum()
                    }
                );

            return Json(data);
        }

    }

}