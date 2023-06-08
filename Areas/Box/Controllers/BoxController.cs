using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using System.IO;

namespace App.Controllers.Boxs
{
     [Area("Box")]
     [Route("/Box/{action=index}")]
     public class BoxController : BaseController
     {
          private readonly AppDbContext _context;
          public string _user;
          public IHttpContextAccessor _httpContextAccessor;

          public BoxController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
          {
               _context = context;
               _httpContextAccessor = httpContextAccessor;
               _user = _httpContextAccessor.HttpContext.Session.GetString("UserName");

          }

          // GET: Box
          public async Task<IActionResult> Index()
          {
               if (!IsLogin)
               {
                    return RedirectToAction("Login", "Account");
               }
               var appDbContext = _context.Boxs.Include(b => b.Account).Where(b => b.Account.username == CurrentUser);
               return View(await appDbContext.ToListAsync());
          }

          // GET: Box/Details/5
          [HttpGet]
          public async Task<IActionResult> Details(int? id)
          {
               if (id == null || _context.Boxs == null)
               {
                    return NotFound();
               }

               var box = await _context.Boxs
                   .Include(b => b.Account)
                   .FirstOrDefaultAsync(m => m.id == id);

               ViewBag.listFile = await _context.Files.Where(f => f.BoxId == id).ToListAsync();
               ViewBag.listComment = await _context.Comments.Include(c => c.Account).Where(c => c.BoxId == id).ToListAsync();
               ViewBag.Account = await _context.Accounts.FirstOrDefaultAsync(a => a.username == CurrentUser);

               if (box == null)
               {
                    return NotFound();
               }

               return View(box);
          }

          // GET: Box/Create
          public IActionResult Create()
          {
               ViewData["UserId"] = new SelectList(_context.Accounts, "id", "username");
               return View();
          }

          // POST: Box/Create
          // To protect from overposting attacks, enable the specific properties you want to bind to.
          // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Create([Bind("id,Title,Content,Pass,ShareCode,Url,IsPublic,UserId")] Box box)
          {
               box.UserId = _context.Accounts.FirstOrDefault(a => a.username == _user).id;
               box.Url = agc.GenerateSlug(box.Title) + "_" + CodeRandom(6);
               box.DateCreated = DateTime.Now;
               if (ModelState.IsValid)
               {
                    _context.Add(box);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
               }
               ViewData["UserId"] = new SelectList(_context.Accounts, "id", "username", box.UserId);
               return View(box);
          }

          // GET: Box/Edit/5
          public async Task<IActionResult> Edit(int? id)
          {

               if (id == null || _context.Boxs == null)
               {
                    return NotFound();
               }

               var box = await _context.Boxs.FindAsync(id);
               if (box == null)
               {
                    return NotFound();
               }
               ViewData["UserId"] = new SelectList(_context.Accounts, "id", "username", box.UserId);
               return View(box);
          }

          // POST: Box/Edit/5
          // To protect from overposting attacks, enable the specific properties you want to bind to.
          // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Edit(int id, [Bind("id,Title,Content,Pass,ShareCode,Url,IsPublic,UserId")] Box box)
          {
               if (id != box.id)
               {
                    return NotFound();
               }

               if (ModelState.IsValid)
               {
                    try
                    {
                         _context.Update(box);
                         await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                         if (!BoxExists(box.id))
                         {
                              return NotFound();
                         }
                         else
                         {
                              throw;
                         }
                    }
                    return RedirectToAction(nameof(Index));
               }
               ViewData["UserId"] = new SelectList(_context.Accounts, "id", "password", box.UserId);
               return View(box);
          }

          // GET: Box/Delete/5
          public async Task<IActionResult> Delete(int? id)
          {
               if (id == null || _context.Boxs == null)
               {
                    return NotFound();
               }

               var box = await _context.Boxs
                   .Include(b => b.Account)
                   .FirstOrDefaultAsync(m => m.id == id);
               if (box == null)
               {
                    return NotFound();
               }

               return View(box);
          }

          // POST: Box/Delete/5
          [HttpPost, ActionName("Delete")]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> DeleteConfirmed(int id)
          {
               if (_context.Boxs == null)
               {
                    return Problem("Entity set 'AppDbContext.Boxs'  is null.");
               }
               var box = await _context.Boxs.FindAsync(id);
               string path = Path.Combine(Directory.GetCurrentDirectory(), "upload", box.Url);
               if (Directory.Exists(path))
               {
                    System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(path);

                    Empty(directory);
                    Directory.Delete(path);
               }
               if (box != null)
               {
                    _context.Boxs.Remove(box);
               }


               await _context.SaveChangesAsync();
               return RedirectToAction(nameof(Index));
          }
          [HttpGet]
          public async Task<IActionResult> UpdateFile(int id)
          {
               var model = await _context.Files.Where(f => f.BoxId == id).ToListAsync();

               ViewBag.box = await _context.Boxs
                  .FirstOrDefaultAsync(m => m.id == id);
               return View(model);
          }
          [HttpPost]
          public async Task<IActionResult> UpdateFile(int id, IFormFile[] files)
          {
               if (id == null || _context.Boxs == null)
               {
                    return NotFound();
               }
               var box = await _context.Boxs
                                .FirstOrDefaultAsync(m => m.id == id);

               // create folder
               string path = Path.Combine(Directory.GetCurrentDirectory(), "upload", box.Url);
               Directory.CreateDirectory(path);
               // Iterate each files
               System.Console.WriteLine();
               foreach (var file in files)
               {

                    // Get the file name from the browser
                    var fileName = System.IO.Path.GetFileName(file.FileName);

                    // Get file path to be uploaded
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "upload", box.Url, fileName);

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
                    var fileSql = new App.Models.File()
                    {
                         Name = fileName,
                         DatePost = DateTime.Now,
                         Size = (file.Length / 1024.0) / 1024.0,
                         BoxId = id,
                    };
                    // double rs = 
                    // agc.a((((file.Length / 1024.0))).ToString());
                    _context.Add(fileSql);
               }
               await _context.SaveChangesAsync();
               ViewBag.Message = "Files are successfully uploaded";
               ViewBag.box = box;
               var model = await _context.Files.Where(f => f.BoxId == id).ToListAsync();
               return RedirectToAction("UpdateFile", new { id = box.id });
          }
          [HttpGet]
          public async Task<IActionResult> Download(string filename, int id)
          {
               if (filename == null)
                    return Content("filename is not availble");
               var box = await _context.Boxs
                                              .FirstOrDefaultAsync(m => m.id == id);
               var path = Path.Combine(Directory.GetCurrentDirectory(), "upload", box.Url, filename);

               var memory = new MemoryStream();
               using (var stream = new FileStream(path, FileMode.Open))
               {
                    await stream.CopyToAsync(memory);
               }
               memory.Position = 0;
               return File(memory, GetContentType(path), Path.GetFileName(path));
          }
          [HttpGet]
          public async Task<IActionResult> DeleteFile(string filename, int id)
          {
               if (filename == null)
               {
                    return Content("filename is not availble");
               }
               var box = await _context.Boxs
                                             .FirstOrDefaultAsync(m => m.id == id);
               var file = await _context.Files
                                              .FirstOrDefaultAsync(f => f.Name == filename && f.BoxId == id);
               var path = Path.Combine(Directory.GetCurrentDirectory(), "upload", box.Url, filename);
               System.IO.File.Delete(path);

               _context.Files.Remove(file);
               await _context.SaveChangesAsync();
               return RedirectToAction("UpdateFile", new { id = id });
          }


          [HttpGet]
          public IActionResult ShareCode()
          {
               Random rd = new Random();
               const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
               char[] chars = new char[12];

               for (int i = 0; i < 12; i++)
               {
                    chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
               }
               string kq = new string(chars);
               // return Content("ac");
               return Json(new { kq = kq });
          }
          [HttpPost]
          public async Task<IActionResult> CreateComment(int boxId,string content)
          {

               // var box = await _context.Boxs
               //     .Include(b => b.Account)
               //     .FirstOrDefaultAsync(m => m.id == boxId);
               Account acc = await _context.Accounts.FirstOrDefaultAsync(a => a.username == CurrentUser);
               Comment comment = new Comment()
               {
                    Content = content,
                    DateCreated = DateTime.Now,
                    BoxId = boxId,
                    AccountId = acc.id
               };
               _context.Add(comment);
               await _context.SaveChangesAsync();
               return RedirectToAction("Details", new { id = boxId });
          }
          public async Task<IActionResult> DeleteComment(int commentId, int boxId){
               var comment = await _context.Comments.FirstOrDefaultAsync( c => c.Id == commentId);
               _context.Comments.Remove(comment);
               await _context.SaveChangesAsync();
               return RedirectToAction("Details", new {id = boxId});
          }
          public string CodeRandom(int x)
          {
               Random rd = new Random();
               const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
               char[] chars = new char[x];

               for (int i = 0; i < x; i++)
               {
                    chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
               }
               string kq = new string(chars);
               return kq;
          }


          private bool BoxExists(int id)
          {
               return (_context.Boxs?.Any(e => e.id == id)).GetValueOrDefault();
          }
          public void Empty(System.IO.DirectoryInfo directory)
          {
               foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
               foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
          }
          private string GetContentType(string path)
          {
               var types = GetMimeTypes();
               var ext = Path.GetExtension(path).ToLowerInvariant();
               return types[ext];
          }

          private Dictionary<string, string> GetMimeTypes()
          {
               return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
          }

     }

}
