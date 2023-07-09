using CRUDExample.Controllers;
using DataManager.Core.AccountDTO;
using DataManager.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DataManager.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }

        [HttpGet]
        [Route("~/account/register")]
        [Authorize("NotAuthenticated")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("~/account/register")]
        [Authorize("NotAuthenticated")]
        public async Task<IActionResult> Register(RegisterDTO registerationInfo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);

                return View(registerationInfo);
            }

            var user = new ApplicationUser()
            {
                UserName = registerationInfo.Email,
                Email = registerationInfo.Email,
                Name = registerationInfo.PersonName,
                PhoneNumber = registerationInfo.PhoneNumber
            };

            var value = await _userManager.CreateAsync(user, registerationInfo.Password);

            if (value.Succeeded)
            {
                if (registerationInfo.Role == Core.Enums.Roles.Admin)
                {
                    if (!await _roleManager.RoleExistsAsync(Core.Enums.Roles.Admin.ToString()))
                    {
                        await _roleManager.CreateAsync(new ApplicationRole() { Name = Core.Enums.Roles.Admin.ToString() });
                    }
                }
                else
                {
                    if (!await _roleManager.RoleExistsAsync(Core.Enums.Roles.User.ToString()))
                    {
                        await _roleManager.CreateAsync(new ApplicationRole() { Name = Core.Enums.Roles.User.ToString() });
                    }
                }

                await _userManager.AddToRoleAsync(user, registerationInfo.Role.ToString());

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }

            ViewBag.Errors = value.Errors.Select(x => x.Description);
            return View(registerationInfo);
        }

        [HttpGet]
        [Route("~/account/login")]
        [Authorize("NotAuthenticated")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("~/account/login")]
        [Authorize("NotAuthenticated")]
        public async Task<IActionResult> Login(LoginDTO loginInfo, string? ReturnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);

                return View(loginInfo);
            }

            var result = await _signInManager.PasswordSignInAsync(loginInfo.Email, loginInfo.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(await _userManager.FindByEmailAsync(loginInfo.Email), Core.Enums.Roles.Admin.ToString()))
                    return Redirect("~/admin/home/index");


                if (!String.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                { 
                    return LocalRedirect(ReturnUrl);
                }
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }

            ViewBag.Errors = new List<string>() { "Invalid login attempt." };
            return View(loginInfo);
        }

        [HttpGet]
        [Route("~/account/logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }
    }
}

