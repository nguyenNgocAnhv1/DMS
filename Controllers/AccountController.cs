using App.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace App.Controllers
{
     public class AccountController : BaseController
     {
          private readonly AppDbContext _context;
          private readonly ISendMailService _sendmailservice;
          [TempData]
          public string _ThongBao { get; set; }
          public AccountController(AppDbContext context, ISendMailService sendmailservice)
          {
               _context = context;
               _sendmailservice = sendmailservice;
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
               UserRole = "";
               await HttpContext.SignOutAsync();
               return RedirectToAction("Login");
          }
          public async Task<IActionResult> ForgotPass()
          {
               return View();
          }
          [HttpPost]
          public async Task<IActionResult> ForgotPass(Account model)
          {
               ModelState.Remove("password");
               ModelState.Remove("username ");
               var isExitUser = await _context.Accounts.FirstOrDefaultAsync(a => a.username == model.username);
               if (isExitUser == null)
               {
                    ModelState.AddModelError("", "Tai khoan khong ton tai");
                    return View(model);
               }
               if (isExitUser.Email != model.Email)
               {
                    ModelState.AddModelError("", "Email khong khop voi tai khoan");
                    return View(model);
               }
               Random rnd = new Random();
               int value = rnd.Next(1000, 9999);
               HttpContext.Session.SetString("codepass", value.ToString());
               MailContent content = new MailContent
               {
                    To = model.Email,
                    Subject = "Xác nhận mật khẩu",
                    Body = $"<p><strong>Mã xác nhận là: {value}</strong></p>"
               };
               await _sendmailservice.SendMail(content);
               _ThongBao = "Kiem tra mail cua ban";
               return RedirectToAction("ValidCode", new { username = model.username });
          }
          public async Task<IActionResult> ValidCode(string username)
          {
               ViewBag.username = username;
               return View();
          }
          [HttpPost]
          public async Task<IActionResult> ValidCode(int code, string username)
          {
               var user = await _context.Accounts.FirstOrDefaultAsync(a => a.username == username);
               if (code.ToString() == HttpContext.Session.GetString("codepass"))
               {
                    return RedirectToAction("NewPass", new { code = code, id = user.id });
               }
               _ThongBao = "Khong dung ma";
               return RedirectToAction("ValidCode", new { username = username });
          }
          public async Task<IActionResult> NewPass(int code, int id, int isChange)
          {
               if (isChange == -1)
               {
                    ViewBag.id = id;
                    return View("NewPass");
               }
               if (code.ToString() == HttpContext.Session.GetString("codepass"))
               {
                    ViewBag.id = id;
                    ViewBag.code = code;
                    return View("NewPass");
               }
               return Content("Khong hop le");
          }
          [HttpPost]
          public async Task<IActionResult> NewPass(int id, string pass, string repass, int code)
          {
               if (UserId == id || UserRole.Contains("Admin") || (code.ToString() == HttpContext.Session.GetString("codepass")))
               {
                    if (pass == repass)
                    {
                         var user = await _context.Accounts.FirstOrDefaultAsync(a => a.id == id);
                         user.password = pass;
                         _context.Accounts.Update(user);
                         await _context.SaveChangesAsync();
                         _ThongBao = "Update thanh cong";
                         HttpContext.Session.SetString("codepass", "397Y8EHWDSUGEIW");

                         return RedirectToAction("Logout");
                    }
               }

               return Content("Co loi xay ra ");
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
                    return RedirectToAction("RegisterGet", new { name = name, email = email, img = image });
               }


          }

          public async Task<IActionResult> LogoutGoogle()
          {
               // HttpContext.si
               await HttpContext.SignOutAsync();

               return RedirectToAction("Login");
          }
          public async Task<IActionResult> SendMail()
          {

               MailContent content = new MailContent
               {
                    To = "keyhmast1@gmail.com",
                    Subject = "Kiểm tra thử hay v dsp",
                    Body = "<p><strong>Xin chào ngocanh.net</strong></p>"
               };

               await _sendmailservice.SendMail(content);
               return Content("Send mail");
          }
     }

}
