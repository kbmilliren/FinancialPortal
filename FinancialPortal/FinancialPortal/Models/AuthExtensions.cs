using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using FinancialPortal.Models;


namespace FinancialPortal.Models
{
    public static class AuthExtensions
    {
        public static async Task RefreshAuthentication(this HttpContextBase context, ApplicationUser user)
        {
            context.GetOwinContext().Authentication.SignOut();
            await context.GetOwinContext().Get<ApplicationSignInManager>().SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }

        public static string GetHouseholdId(this IIdentity user)
        {
            var claimsIdentity = (ClaimsIdentity)user;
            var HouseholdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            if (HouseholdClaim != null)
                return HouseholdClaim.Value;
            else
                return null;
        }

        public static bool IsInHousehold(this IIdentity user)
        {
            var householdClaim = ((ClaimsIdentity)user).Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            return householdClaim != null && !string.IsNullOrWhiteSpace(householdClaim.Value);
        }

    }
        public class RequireHousehold : System.Web.Mvc.AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                var isAuthorized = base.AuthorizeCore(httpContext);
                if (!isAuthorized)
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(httpContext.User.Identity.GetHouseholdId()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {           
                        base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Households", action = "Create" }));
                }
            }

              

           

        }
}