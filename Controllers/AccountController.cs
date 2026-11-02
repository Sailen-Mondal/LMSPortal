using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LMSPortal.Models;
using LMSPortal.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace LMSPortal.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IAuthenticationManager _authManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, IAuthenticationManager authManager = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authManager = authManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return _authManager ?? HttpContext.GetOwinContext().Authentication;
            }
            private set
            {
                _authManager = value;
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToUserLanding();
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Require email confirmation or lockout validation
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "This account has been temporarily locked out due to multiple failed login attempts. Please try again in 5 minutes.");
                    return View(model);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid email address or password. Please try again.");
                    return View(model);
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToUserLanding();
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Assign new registered users strictly to the Student role
                await UserManager.AddToRoleAsync(user.Id, "Student");
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                return RedirectToLocal(returnUrl);
            }

            AddErrors(result);
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            PerformSignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // GET: /Account/LogOff
        [HttpGet]
        public ActionResult LogOff(string returnUrl = null)
        {
            PerformSignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // GET: /Account/Logout (Friendly route alias)
        [HttpGet]
        public ActionResult Logout()
        {
            return RedirectToAction("LogOff");
        }

        // POST: /Account/Logout (Friendly route alias)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout(string returnUrl = null)
        {
            return LogOff();
        }

        private void PerformSignOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
            if (Session != null)
            {
                Session.Abandon();
            }

            if (Request.Cookies["LMSPortal_Auth"] != null)
            {
                var cookie = new System.Web.HttpCookie("LMSPortal_Auth", "")
                {
                    Expires = DateTime.UtcNow.AddYears(-1),
                    Path = "/"
                };
                Response.Cookies.Add(cookie);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl != "/" && !returnUrl.ToLower().Contains("/account/"))
            {
                return Redirect(returnUrl);
            }

            return RedirectToUserLanding();
        }

        private ActionResult RedirectToUserLanding()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "AdminDashboard", new { area = "Admin" });
            }

            return RedirectToAction("Dashboard", "Student");
        }
        #endregion
    }
}
