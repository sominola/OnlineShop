using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data.Models;
using OnlineShop.Services.File;
using OnlineShop.Web.ViewModels.Account;

namespace OnlineShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IImageService _imageService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            IImageService imageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _imageService = imageService;
        }

        [HttpGet]
        [Authorize]
        [Route("Account/")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
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
                    var image = await _imageService.UploadImageAsync(model.File, Guid.Parse(user.Id));

                    user.Email = model.Email;
                    user.Login = model.Login;
                    user.Name = model.Name;
                    user.LastName = model.LastName;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        ViewBag.IsSuccess = true;
                        model.AvatarPath = image.Path;
                        return View(model);
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
            if (_signInManager.IsSignedIn(User))
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

        [AllowAnonymous]
        [Route("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            var model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
            };

            if (remoteError != null)
            {
                ModelState
                    .AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", model);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState
                    .AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", model);
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, true, true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var surname = info.Principal.FindFirstValue(ClaimTypes.Surname);
            var identifier = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            // var picture = $"https://graph.facebook.com/{identifier}/picture?type=large";

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new User()
                {
                    Login = email ??= identifier,
                    Email = email,
                    Name = name,
                    LastName = surname
                };

                await _userManager.CreateAsync(user);
            }

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, false);

            return LocalRedirect(returnUrl);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ExternalLogin")]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                new {ReturnUrl = returnUrl});

            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login(string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
            };
            return View(model);
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

            return View(model);
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