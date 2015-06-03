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
    public class HouseholdAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HouseholdAccounts
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var householdAccounts = db.HouseholdAccounts.Include(h => h.Household).Where(a => a.HouseholdId == user.HouseholdId) ;

            return View(householdAccounts.ToList());
        }

        // GET: HouseholdAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseholdAccount householdAccount = db.HouseholdAccounts.Find(id);
            if (householdAccount == null)
            {
                return HttpNotFound();
            }
            return View(householdAccount);
        }

        // GET: HouseholdAccounts/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View();
        }

        // POST: HouseholdAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HouseholdId,Name,Balance,ReconciledBalance")] HouseholdAccount householdAccount)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                householdAccount.HouseholdId = user.HouseholdId.Value;
                db.HouseholdAccounts.Add(householdAccount);
                db.SaveChanges();
                var transaction = new Transaction()
                {
                    Amount = householdAccount.Balance,
                    AccountId = householdAccount.Id,
                    CategoryId = db.Categories.First(c => c.Name == "Initial Deposit").Id,
                    Date = System.DateTimeOffset.Now,
                    Description = "",
                    UpdatedByUserId = user.Id
                };
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", householdAccount.HouseholdId);
            return View(householdAccount);
        }

        // GET: HouseholdAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseholdAccount householdAccount = db.HouseholdAccounts.Find(id);
            if (householdAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", householdAccount.HouseholdId);
            return View(householdAccount);
        }

        // POST: HouseholdAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Name,Balance,ReconciledBalance")] HouseholdAccount householdAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(householdAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", householdAccount.HouseholdId);
            return View(householdAccount);
        }

        // GET: HouseholdAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseholdAccount householdAccount = db.HouseholdAccounts.Find(id);
            if (householdAccount == null)
            {
                return HttpNotFound();
            }
            return View(householdAccount);
        }

        // POST: HouseholdAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HouseholdAccount householdAccount = db.HouseholdAccounts.Find(id);
            db.HouseholdAccounts.Remove(householdAccount);
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
