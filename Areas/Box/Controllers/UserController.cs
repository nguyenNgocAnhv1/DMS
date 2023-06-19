using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;

namespace App.Controllers.Boxs
{
     [Area("Box")]
     [Route("/User/{action}")]
     public class UserController : BaseController
     {
          private readonly AppDbContext _context;
          [TempData]
          public string _ThongBao { get; set; }

          public UserController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
          {
               _context = context;

          }
          [HttpGet]
          public async Task<IActionResult> Detail(int id)
          {
               var user = await _context.Accounts.FirstOrDefaultAsync(a => a.id == id);
               var listBox = _context.Boxs.Where(b => b.UserId == user.id && b.IsPublic == true).ToList();
               ViewBag.listBox = listBox;

               return View(user);
          }
          [HttpGet]
          public async Task<IActionResult> Edit(int id)
          {
               var acc = await _context.Accounts.FirstOrDefaultAsync(a => a.id == id);
               return View(acc);
          }
          [HttpPost]
          public async Task<IActionResult> Edit(int id, Account acc)
          {
               ModelState.Remove("acc.username");
               ModelState.Remove("acc.password");
               var oldAcc = await _context.Accounts.FirstOrDefaultAsync(a => a.id == id);
               if (ModelState.IsValid)
               {
                    oldAcc.Name = acc.Name;
                    oldAcc.JobTitle = acc.JobTitle;
                    oldAcc.Description = acc.Description;
                    _context.Update(oldAcc);
                    await _context.SaveChangesAsync();
               }
               _ThongBao = "Update thanh cong";

               return RedirectToAction("Edit", new { id = acc.id });
          }
          [HttpPost]
          public async Task<IActionResult> UpdateAvt(int id, IFormFile file)
          {

               var acc = await _context.Accounts
                                .FirstOrDefaultAsync(m => m.id == id);
               if (acc.Img != null && acc.Img != "avtuser.jpg")
               {
                    string oldFile = Path.Combine(Directory.GetCurrentDirectory(), "upload", "avt", acc.Img);
                    System.IO.File.Delete(oldFile);

               }
               if (file == null)
               {
                    _ThongBao = "Hay tai len anh";
                    return RedirectToAction("Detail", new { id = acc.id });
               }
               var fileName = acc.username + "_" + System.IO.Path.GetFileName(file.FileName);

               // create folder
               string path = Path.Combine(Directory.GetCurrentDirectory(), "upload", "avt");
               Directory.CreateDirectory(path);
               // Iterate each files


               // Get the file name from the browser

               // Get file path to be uploaded
               var filePath = Path.Combine(Directory.GetCurrentDirectory(), "upload", "avt", fileName);

               // Check If file with same name exists and delete it
               if (System.IO.File.Exists(filePath))
               {
                    System.IO.File.Delete(filePath);
               }

               // Create a new local file and copy contents of uploaded file
               using (var localFile = System.IO.File.OpenWrite(filePath))
               using (var uploadedFile = file.OpenReadStream())
               {
                    uploadedFile.CopyTo(localFile);
               }

               acc.Img = fileName;

               await _context.SaveChangesAsync();
               return RedirectToAction("Detail", new { id = acc.id });
          }
          public async Task<IActionResult> MyAdmin()
          {
               var listUser = _context.Accounts.ToList();
               return View(listUser);
          }
          public async Task<IActionResult> Ban(int userId)
          {
               var acc = await _context.Accounts.FirstOrDefaultAsync(a => a.id == userId);
               if (acc.BanEnabled == null)
               {
                    acc.BanEnabled = true;
               }
               else
               {
                    acc.BanEnabled = !acc.BanEnabled;
               }
               _context.Update(acc);
               await _context.SaveChangesAsync();
               return RedirectToAction("MyAdmin");
          }
          public async Task<IActionResult> SetRole(int id)
          {
               // var boxShare = await _context.BoxShares.FirstOrDefaultAsync(b => b.id == id);

               // var lisrRole = (await _context.Accounts.Include(a => a.ListBoxShare).SelectMany(a => a.).FirstOrDefaultAsync(a => a.id == id).).
               var acc = await _context.Accounts.FirstOrDefaultAsync(a => a.id == id);
               var listRole = _context.UserRoles.Where(b => b.UserId == id).Select(b => b.RoleId).ToList();
               var userRole = new BoxShareList()
               {
                    RoleList = listRole,
               };
               var listAllRole = _context.Roles.ToList();
               ViewBag.listAllRole = new MultiSelectList(listAllRole, "id", "Name");
               ViewBag.id = id;
               ViewBag.name = acc.Name;
               return View(userRole);
          }
          [HttpPost]
          public async Task<IActionResult> SetRole(int id, BoxShareList shareList)
          {
               var listAllRole = _context.Roles.ToList();
               ViewBag.listAllRole = new MultiSelectList(listAllRole, "id", "Name");
               ViewBag.id = id;
               var userRole = _context.UserRoles.Where(u => u.UserId == id);
               var listNewUserRole = new List<UserRole>() { };


               _context.RemoveRange(userRole);
               if(shareList.RoleList == null){
                    shareList.RoleList = new List<int>(){};
               }
               foreach (var item in shareList.RoleList)
               {
                    listNewUserRole.Add(
                         new UserRole()
                         {
                              UserId = id,
                              RoleId = item
                         }
                    );
               }
               _context.AddRange(listNewUserRole);
               await _context.SaveChangesAsync();

               var listRole = _context.UserRoles.Where(b => b.UserId == id).Select(b => b.RoleId).ToList();
               var model = new BoxShareList()
               {
                    RoleList = listRole,
               };
               _ThongBao = "Thanh cong";

               return RedirectToAction("SetRole", new{id = id});
          }
     }
}
