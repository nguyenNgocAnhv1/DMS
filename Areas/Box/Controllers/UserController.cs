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
               if (ModelState.IsValid)
               {
                    _context.Update(acc);
                    await _context.SaveChangesAsync();
                    _ThongBao = "Update thanh cong";
               }
               return View(acc);
          }
          [HttpPost]
          public async Task<IActionResult> UpdateAvt(int id, IFormFile file)
          {

               var acc = await _context.Accounts
                                .FirstOrDefaultAsync(m => m.id == id);
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
          public async Task<IActionResult> Admin()
          {
               var listUser = _context.Accounts.ToList();
               return View(listUser);
          }
     }
}
