using App.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace App.Controllers
{
     public class AccountController : BaseController
     {
          private readonly AppDbContext _context;
          public AccountController(AppDbContext context)
          {
               _context = context;
          }
          public async Task<IActionResult> Login()
          {
               return View();
          }

          [HttpPost]
          public async Task<IActionResult> Login(string username, string password)
          {
               var model = new Account()
               {
                    username = username,
                    password = password
               };


               if (ModelState.IsValid)
               {
                    var loginUser = _context.Accounts.ToList().FirstOrDefault(m => m.username == model.username);
                    if (loginUser == null)
                    {

                         ModelState.AddModelError("", "Đăng nhập thất bại");
                         return View(model);
                    }
                    else
                    {

                         if (model.password == loginUser.password)
                         {
                              if (loginUser.BanEnabled == true)
                              {
                                   return View("Ban");
                              }
                              var listRole = _context.UserRoles.Include(u => u.Role).Where(r => r.UserId == loginUser.id).Select(u => u.Role.Name).ToList();
                              var result = String.Join(", ", listRole);
                              CurrentUser = loginUser.username;
                              UserId = loginUser.id;
                              UserRole = result;

                              agc.a($"Login {CurrentUser} -  {UserId} - {UserRole}");
                              return RedirectToAction("Index", "Box", new { area = "Box", userAccess = UserId });
                         }
                         else
                         {
                              ModelState.AddModelError("", "Login False");
                              return View(model);
                         }
                    }
               }
               return View(model);
          }
          [HttpGet]
          public async Task<IActionResult> LoginGet(Account model)
          {
               agc.a(model.username);
               if (ModelState.IsValid)
               {
                    var loginUser = _context.Accounts.ToList().FirstOrDefault(m => m.username == model.username);
                    if (loginUser == null)
                    {

                         ModelState.AddModelError("", "Đăng nhập thất bại");
                         return View("Login", model);
                    }
                    else
                    {

                         if (model.password == loginUser.password)
                         {
                              if (loginUser.BanEnabled == true)
                              {
                                   return View("Ban");
                              }
                              var listRole = _context.UserRoles.Include(u => u.Role).Where(r => r.UserId == loginUser.id).Select(u => u.Role.Name).ToList();
                              var result = String.Join(", ", listRole);
                              CurrentUser = loginUser.username;
                              UserId = loginUser.id;
                              UserRole = result;

                              agc.a($"Login {CurrentUser} -  {UserId} - {UserRole}");
                              return RedirectToAction("Index", "Box", new { area = "Box", userAccess = UserId });
                         }
                         else
                         {
                              ModelState.AddModelError("", "Login False");
                              return View("Login", model);
                         }
                    }
               }
               return View("Login", model);
          }
          public IActionResult Register()
          {
               return View();
          }
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Register(string username, string password)
          {
               var model = new Account()
               {
                    username = username,
                    password = password
               };
               if (ModelState.IsValid)
               {
                    var isExit = await _context.Accounts.FirstOrDefaultAsync(a => a.username == model.username);
                    if (isExit != null)
                    {
                         ModelState.AddModelError("", "Tai khoan da ton tai");
                    }
               }


               if (ModelState.IsValid)
               {
                    model.Img = "avtuser.jpg";
                    model.BanEnabled = false;
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Login));
               }
               return View(model);
          }

          public async Task<IActionResult> RegisterGet(string img, string email, string name)
          {
               var model = new Account()
               {
                    Name = name,
                    Email = email,
                    Img = img,
               };
               return View(model);
          }

          [HttpPost]
          public async Task<IActionResult> RegisterGet(int id, Account model)
          {
               if (ModelState.IsValid)
               {
                    var isExit = await _context.Accounts.FirstOrDefaultAsync(a => a.username == model.username);
                    if (isExit != null)
                    {
                         ModelState.AddModelError("", "Tai khoan da ton tai");
                    }
               }

               if (ModelState.IsValid)
               {
                    model.Img = "avtuser.jpg";
                    model.BanEnabled = false;
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Login));
               }
               return View(model);
          }
          public async Task<IActionResult> LogoutAsync()
          {
               agc.a($"Logout {CurrentUser} -  {UserId}");
               CurrentUser = "";
               UserId = -1;
               await HttpContext.SignOutAsync();
               return RedirectToAction("Login");
          }
          public async Task LoginGoogle()
          {
               await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
               {
                    RedirectUri = Url.Action("GoogleResponse")
               });

          }
          public async Task<IActionResult> GoogleResponse()
          {
               var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
               var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
               {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
               });
               var name = (claims.ToList())[1].Value.ToString();
               var email = (claims.ToList())[4].Value.ToString();
               var image = (claims.ToList())[5].Value.ToString();
               System.Console.WriteLine(email);
               var user = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
               if (user != null)
               {
                    return RedirectToAction("LoginGet", new { username = user.username, password = user.password });
               }
               else
               {
                    return RedirectToAction("RegisterGet", new {name = name, email = email, img = image});
               }


          }

          public async Task<IActionResult> LogoutGoogle()
          {
               // HttpContext.si
               await HttpContext.SignOutAsync();

               return RedirectToAction("Login");
          }
     }

}
