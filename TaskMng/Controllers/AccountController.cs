using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TaskManagerUsersBLL.Interfaces;
using TaskMng.Models;
using TaskManagerUsersBLL.Services;
using TaskManagerUsersBLL.Models;
using TaskManagerUsersBLL.Infrastructure;

namespace TaskMng.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {         
            if (ModelState.IsValid)
            {
                var userBLL = new UserBLL { UserName = model.Name, Password = model.Password };

                var claim = await UserService.Authenticate(userBLL);
                if (claim == null)
                {
                    ModelState.AddModelError("", "Invalid login or password.");
                }
                else
                {
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    if (String.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (!(model.IsManager && (string.IsNullOrWhiteSpace(model.TeamName) || model.TeamName.Length < 5)))
                {
                    var role = model.IsManager ? "Manager" : "Programmer";
                    var userBll = new UserBLL
                    {
                        Email = model.Email,
                        Password = model.Password,
                        TeamName = model.TeamName,
                        UserName = model.Name,
                        Role = role//!!TODO
                    };
                    var operationDetails = await UserService.Create(userBll);
                    if (operationDetails.Succedeed)
                    {
                        return RedirectToAction("Index", "Home"); ;
                    }
                    else
                    {
                        ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError("TeamName", "The team name must be at least 5 characters long. ");
                }
            }
            return View(model);
        }
    }
}

