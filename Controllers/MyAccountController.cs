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
     [Route("/api/account/{action}")]
     public class MyAccountController : BaseController
     {
          private readonly AppDbContext _context;
          private readonly ISendMailService _sendmailservice;
          [TempData]
          public string _ThongBao { get; set; }
          public MyAccountController(AppDbContext context, ISendMailService sendmailservice)
          {
               _context = context;
               _sendmailservice = sendmailservice;
          }

          [HttpPost]
          public async Task<IActionResult> MyLogin([FromBody] Account acc)
          {
               var password = acc.password;
               var username = acc.username;
               Account model = new Account()
               {
                    username = acc.username,
                    password = acc.password
               };
               string messages = string.Join("; ", ModelState.Values
                                                       .SelectMany(x => x.Errors)
                                                       .Select(x => x.ErrorMessage));
               if (ModelState.IsValid)
               {
                    var loginUser = _context.Accounts.ToList().FirstOrDefault(m => m.username == model.username);
                    if (loginUser == null)
                    {
                         ModelState.AddModelError("", "Đăng nhập thất bại");
                         return Json(new { kq = "false" });
                    }
                    else
                    {
                         if (password == loginUser.password)
                         {
                              if (loginUser.BanEnabled == true)
                              {
                                   return Json(new { kq = "ban" });
                              }
                              var listRole = _context.UserRoles.Include(u => u.Role).Where(r => r.UserId == loginUser.id).Select(u => u.Role.Name).ToList();
                              var result = String.Join(", ", listRole);
                              CurrentUser = loginUser.username;
                              UserId = loginUser.id;
                              UserRole = result;

                              agc.a($"Login {CurrentUser} -  {UserId} - {UserRole}");
                              return Json(new { kq = "true", user = JsonConvert.SerializeObject(loginUser) });
                         }
                         else
                         {
                              ModelState.AddModelError("", "Login False");
                              return Json(new { kq = "false" });
                         }
                    }
               }
               return Json(new { kq = "false" });

          }
          [HttpPost]
          public async Task<IActionResult> MyRegister([FromBody] Account model)
          {
               agc.a(model.username + " " + model.password);
               if (ModelState.IsValid)
               {
                    var isExit = await _context.Accounts.FirstOrDefaultAsync(a => a.username == model.username);
                    if (isExit != null)
                    {
                         ModelState.AddModelError("", "Tai khoan da ton tai");
                         return Json(new { kq = "Tai khoan da ton tai" });
                    }
               }


               if (ModelState.IsValid)
               {
                    model.Img = "avtuser.jpg";
                    model.BanEnabled = false;
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    return Json(new { kq = "Thanh cong", user = JsonConvert.SerializeObject(model) });
               }
               return Json(new { kq = "Loi khong xac dinh" });
          }
          public async Task<IActionResult> MyLogout()
          {
               agc.a($"Logout {CurrentUser} -  {UserId}");
               CurrentUser = "";
               UserId = -1;
               UserRole = "";
               await HttpContext.SignOutAsync();
              return Json(new {kq = "true"});
          }
     }

}
