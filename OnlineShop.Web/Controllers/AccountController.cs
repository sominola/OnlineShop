using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data.Models;
using OnlineShop.Web.ViewModels.Account;

namespace OnlineShop.Web.Controllers
{
    [Route("")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Authorize]
        [Route("Account/")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var model = new EditAccountViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Login = user.Login,
                Name = user.Name,
                LastName = user.LastName,
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [Route("Account/")]
        public async Task<IActionResult> Index(EditAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.Login = model.Login;
                    user.Name = model.Name;
                    user.LastName = model.LastName;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        ViewBag.IsSuccess = true;
                        return View();
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            return View();
        }

        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    Name = model.Name,
                    LastName = model.LastName,
                    Login = model.Login, 
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View();
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login(string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel {ReturnUrl = returnUrl});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login and (or) password");
                }
            }

            return View();
        }

        [HttpGet]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return View(new ChangePasswordViewModel());
            }
            
            var user = await _userManager.GetUserAsync(HttpContext.User);
            
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Wrong user id");
                return View();
            }

            var model = new ChangePasswordViewModel()
            {
                Id = user.Id,
                Login = user.Login
            };
            return View(model);
        }
        
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user;
                if (model.Id == null)
                {
                    user = await _userManager.FindByNameAsync(model.Login);
                }
                else
                {
                    user = await _userManager.FindByIdAsync(model.Id);
                }
                
                if (user != null)
                {
                    var passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as
                            IPasswordValidator<User>;
                    var passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    var result =
                        await passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);
                        await _userManager.UpdateAsync(user);
                        ViewBag.IsSuccess = true;
                        return View();
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}