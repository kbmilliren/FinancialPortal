using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPortal.Models;

namespace FinancialPortal.Controllers
{
    [RequireHousehold]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        [Route("~/Accounts/{accountId}/Transactions")]
        public ActionResult Index(string query, int accountId)
        {
            var db = new ApplicationDbContext();
            var transactions = db.HouseholdAccounts.Find(accountId).Transactions.AsQueryable();
            ViewBag.AccountId = accountId;
            
            if (!String.IsNullOrWhiteSpace(query))
            {
                transactions = transactions.Where(p => p.Description.Contains(query) || p.Amount.Equals(query) || p.Category.Equals(query));
                ViewBag.Query = query;
            }
            
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        [Route("~/Accounts/{accountId}/Transactions/{transactionId}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        [Route("~/Accounts/{accountId}/Transactions/Create")]
        public ActionResult Create(int accountId)
        {
            var account = db.Households.Find(Int32.Parse(User.Identity.GetHouseholdId())).Accounts.FirstOrDefault(a => a.Id == accountId);

            if (account == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("~/Accounts/{accountId}/Transactions/Create")]
        public ActionResult Create([Bind(Include = "Id,AccountId,Amount,AbsAmount,ReconciledAmount,AbsReconciledAmount,Date,Description,Updated,UpdatedByUserId,CategoryId")] Transaction transaction, int accountId)
        {
            if (ModelState.IsValid)
            {
                var account = db.Households.Find(Int32.Parse(User.Identity.GetHouseholdId())).Accounts.FirstOrDefault(a => a.Id == accountId);
                transaction.AccountId = accountId;

                db.Transactions.Add(transaction);
                if (account == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                account.Balance += transaction.Amount;
                transaction.Date = DateTimeOffset.Now;
                transaction.UpdatedByUser = db.Users.Find(User.Identity.ToString());
               
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        [Route("~/Accounts/{accountId:int}/Transactions/{id}/Edit", Name="EditTransaction")]
        public ActionResult Edit(int accountId, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("~/Accounts/{accountId:int}/Transactions/{id}/Edit")]
        public ActionResult Edit([Bind(Include = "Id,AccountId,Amount,AbsAmount,ReconciledAmount,AbsReconciledAmount,Date,Description,Updated,UpdatedByUserId,CategoryId")] Transaction transaction, int accountId)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();

                var account = db.HouseholdAccounts.Find(transaction.AccountId);
                    account.Balance = (from t in db.Transactions
                                       where t.AccountId == account.Id
                                       select t.Amount).Sum();

                account.Balance = db.Transactions.Where(t => t.AccountId == account.Id).Select(b => b.Amount).Sum();

                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
