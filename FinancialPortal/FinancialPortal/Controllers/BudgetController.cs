using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPortal.Models;

namespace FinancialPortal.Controllers
{
    public class BudgetController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budget
        public async Task<ActionResult> Index()
        {
            var budgetItems = db.BudgetItems.Include(b => b.Category).Include(b => b.Household);
            return View(await budgetItems.ToListAsync());
        }

        // GET: Budget/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = await db.BudgetItems.FindAsync(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // GET: Budget/Create
        public ActionResult Create()
        {
          
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Budget/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,HouseholdId,CategoryId,Amount")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                int householdId = Int32.Parse(User.Identity.GetHouseholdId());
                budgetItem.HouseholdId = householdId;
                db.BudgetItems.Add(budgetItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItem.CategoryId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budgetItem.HouseholdId);
            return View(budgetItem);
        }

        // GET: Budget/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = await db.BudgetItems.FindAsync(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItem.CategoryId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budgetItem.HouseholdId);
            return View(budgetItem);
        }

        // POST: Budget/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,HouseholdId,CategoryId,Amount")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budgetItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItem.CategoryId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budgetItem.HouseholdId);
            return View(budgetItem);
        }

        // GET: Budget/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = await db.BudgetItems.FindAsync(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // POST: Budget/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            BudgetItem budgetItem = await db.BudgetItems.FindAsync(id);
            db.BudgetItems.Remove(budgetItem);
            await db.SaveChangesAsync();
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
