﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPortal.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using SendGrid;

namespace FinancialPortal.Controllers
{
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households
        [RequireHousehold]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.Households.Find(user.HouseholdId);
            return View(household);
        }
        
        // GET: Households/Details/5
        [RequireHousehold]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            Household household = db.Households.Find(user.HouseholdId);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(user.HouseholdId);
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                var house = new Household();
                db.Households.Add(house);
                db.SaveChanges();
                db.Categories.Add(new Category { Name = "Initial Deposit", HouseholdId = house.Id });
                db.Categories.Add(new Category { Name = "Food", HouseholdId = house.Id });
                db.Categories.Add(new Category { Name = "Rent", HouseholdId = house.Id });
                db.Categories.Add(new Category { Name = "Gas", HouseholdId = house.Id });
                db.Categories.Add(new Category { Name = "Salary", HouseholdId = house.Id });
                var user = db.Users.Find(User.Identity.GetUserId());
                user.HouseholdId = house.Id;
                db.SaveChanges();
                await ControllerContext.HttpContext.RefreshAuthentication(user);
                return RedirectToAction("Index");
            }

            return View(household);
        }

        // GET: Households/Edit/5
        [RequireHousehold]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireHousehold]
        public ActionResult Edit([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        [RequireHousehold]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequireHousehold]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
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

        public async Task<ActionResult> LeaveHousehold()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            await ControllerContext.HttpContext.RefreshAuthentication(user);
            return RedirectToAction("Create", "Household");
        }

        public async Task<ActionResult> SendInvitation(string ToEmail)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            
            var hhId = Int32.Parse(User.Identity.GetHouseholdId());

            var charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";

            var rng = new Random();

            var code = new string(Enumerable.Range(1, 6).Select(n => charset[rng.Next(charset.Length -1)]).ToArray());

            var invite = new Invitation()
            {
                ToEmail = ToEmail,
                FromUserId = User.Identity.GetUserId(),
                Code = code
            };

            db.Invitations.Add(invite);
            db.SaveChanges();

            var mailer = new EmailService();
            mailer.Send(new IdentityMessage() { 
                Destination = ToEmail,
                Subject = "",
                Body = "You have been...." + "..." + code + "..."
            });

            return RedirectToAction("Index");
        }

        public ActionResult JoinHousehold(string code)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var invite = db.Invitations.Where(i => i.Code == code && i.ToEmail == user.Email).FirstOrDefault();

            if(invite != null)
            {
                user.HouseholdId = invite.HouseholdId;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
