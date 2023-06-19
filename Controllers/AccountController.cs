using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
          public IActionResult Login()
          {
               return View();
          }
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Login(Account model)
          {
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
                         SHA256 hashMethod = SHA256.Create();
                         if (App.Util.VerifyHash(hashMethod, model.password, loginUser.password))
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
          public IActionResult Register()
          {
               return View();
          }
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Register(Account model)
          {
               if (ModelState.IsValid)
               {
                    var isExit = await _context.Accounts.FirstOrDefaultAsync(a => a.username == model.username);
                    if (isExit != null)
                    {
                         ModelState.AddModelError("","Tai khoan da ton tai");
                    }
               }


               if (ModelState.IsValid)
               {

                    SHA256 hashMethod = SHA256.Create();
                    model.password = App.Util.GetHash(hashMethod, model.password);
                    model.Img = "avtuser.jpg";
                    model.BanEnabled = false;
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Login));
               }
               return View(model);
          }
          public IActionResult Logout()
          {
               agc.a($"Logout {CurrentUser} -  {UserId}");
               CurrentUser = "";
               UserId = -1;
               return RedirectToAction("Login");
          }
     }

}
