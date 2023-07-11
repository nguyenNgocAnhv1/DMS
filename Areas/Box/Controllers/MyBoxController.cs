using System;
using System.Web;
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
using System.IO.Compression;
using System.IO.Compression;
namespace App.Controllers.Boxs
{
     [Area("Box")]
     [Route("/api/MyBox/{action=index}")]
     public class MyBoxController : BaseController
     {
          private readonly AppDbContext _context;
          public string _user;
          public IHttpContextAccessor _httpContextAccessor;

          public MyBoxController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
          {
               _context = context;
               _httpContextAccessor = httpContextAccessor;
               _user = _httpContextAccessor.HttpContext.Session.GetString("UserName");

          }

          // GET: Box
          public async Task<IActionResult> Index(int userAccess)
          {
               agc.a(userAccess.ToString());
               if (!IsLogin)
               {
                    return Json(new {kq ="false", des = "Chua login"});
               }
               var user = await _context.Accounts.FirstOrDefaultAsync(a => a.id == userAccess);
               var appDbContext = _context.Boxs.Where(b => b.UserId == userAccess).Select(b => new{b.id,b.Title,b.Pass,b.ShareCode,b.Url,b.IsPublic,b.UserId,b.DateCreated,b.View,b.Img,b.AdminBan}).ToList();
               var boxShare = _context.BoxShares.Include(b => b.Box).Where(b => b.UserId== userAccess).Select(b => new{b.Box.id,b.Box.Title,b.Box.Pass,b.Box.ShareCode,b.Box.Url,b.Box.IsPublic,b.Box.UserId,b.Box.DateCreated,b.Box.View,b.Box.Img,b.Box.AdminBan});
               
               ViewBag.name = user.Name;
              return Json(new {kq ="true", box = JsonConvert.SerializeObject(appDbContext), boxShare= JsonConvert.SerializeObject(boxShare) });
          }

          // GET: Box/Details/5
          [HttpGet]
          public async Task<IActionResult> Details(int? id, string? pass)
          {
               var box = await _context.Boxs
                   .Include(b => b.Account)
                   .FirstOrDefaultAsync(m => m.id == id);

               if (id == null || _context.Boxs == null)
               {
                    return NotFound();
               }
               if (box.IsPublic != true)
               {
                    return RedirectToAction("Hided");
               }
               if (box.AdminBan == true)
               {
                    return View("AdminBan");
               }


               if (!string.IsNullOrEmpty(box.Pass))
               {

                    if (pass != box.Pass)
                    {
                         return RedirectToAction("CheckPass", new { id = id });
                    }
               }
               box.View = box.View + 1;
               _context.Update(box);
               await _context.SaveChangesAsync();
               ViewBag.listFile = await _context.Files.Where(f => f.BoxId == id && f.Name != box.Img).ToListAsync();
               ViewBag.listComment = await _context.Comments     .Include(c => c.Account).Where(c => c.BoxId == id).ToListAsync();
               ViewBag.voteStatus = 100;
               if (CurrentUser != null)
               {
                    var acc = await _context.Accounts.FirstOrDefaultAsync(a => a.username == CurrentUser);
                    ViewBag.Account = acc;
                    var voteStatus = await _context.Votes.FirstOrDefaultAsync(v => v.BoxId == box.id && v.AccountId == acc.id);
                    if (voteStatus != null)
                    {
                         ViewBag.voteStatus = voteStatus.Status;
                    }


               }
               else
               {
                    ViewBag.Account = null;

               }

               if (box == null)
               {
                    return NotFound();
               }
               var up = _context.Votes.Where(v => v.BoxId == box.id && v.Status == 1).Count();
               var down = _context.Votes.Where(v => v.BoxId == box.id && v.Status == -1).Count();
               ViewBag.vote = up - down;

               return View(box);
          }
          [HttpGet]
          public async Task<IActionResult> ShortLink(int id, string? pass)
          {
               var box = await _context.Boxs.FirstOrDefaultAsync(b => b.id == id);


               if (id == null || _context.Boxs == null)
               {
                    return NotFound();
               }
               if (box.IsPublic != true)
               {
                    return RedirectToAction("Hided");
               }
               if (box.AdminBan == true)
               {
                    return View("AdminBan");
               }


               if (!string.IsNullOrEmpty(box.Pass))
               {

                    if (pass != box.Pass)
                    {
                         return RedirectToAction("CheckPass", new { id = id });
                    }
               }
               ViewBag.listFile = await _context.Files.Where(f => f.BoxId == id && f.Name != box.Img).ToListAsync();

               return View(box);
          }

          [HttpGet]
          public IActionResult CheckPass(int id)
          {
               ViewBag.id = id;
               return View();
          }
          [HttpPost]
          public IActionResult CheckPass(int id, string pass)
          {

               return RedirectToAction("Details", new { id = id, pass = pass });
          }
          public IActionResult Hided()
          {
               return View();
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
          public async Task<IActionResult> Create(IFormFile? file, IFormFile[]? files, [Bind("id,Title,Content,Pass,ShareCode,Url,IsPublic,UserId")] Box box)
          {
               if (file == null)
               {
                    ModelState.AddModelError(string.Empty, "Can them anh dai dien cho quoc hop");
               }
               else
               {
                    box.Img = System.IO.Path.GetFileName(file.FileName);

               }

               if (ModelState.IsValid)
               {
                    box.UserId = _context.Accounts.FirstOrDefault(a => a.username == _user).id;
                    box.Url = agc.GenerateSlug(box.Title) + "_" + CodeRandom(6);
                    box.DateCreated = DateTime.Now;
                    box.View = 0;
                    box.AdminBan = false;
                    _context.Add(box);
                    await _context.SaveChangesAsync();
                    await UpdateFileAsync(box.id, file, "");
                    if (files != null)
                    {
                         foreach (var item in files)
                         {
                              await UpdateFileAsync(box.id, item, "");
                         }
                    }
                    return RedirectToAction("Index", new { userAccess = box.UserId });
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
               ViewBag.listFile = await _context.Files.Where(f => f.BoxId == id && f.Name != box.Img).ToListAsync();

               ViewData["UserId"] = new SelectList(_context.Accounts, "id", "username", box.UserId);
               return View(box);
          }

          // POST: Box/Edit/5
          // To protect from overposting attacks, enable the specific properties you want to bind to.
          // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Edit(int id, IFormFile? file, Box box)
          {
               // var boxSql = (await _context.Boxs.FirstOrDefaultAsync(b => b.id == box.id)).Img;
               var oldImg = box.Img;
               if (file != null)
               {
                    box.Img = System.IO.Path.GetFileName(file.FileName);
               }
               if (id != box.id)
               {
                    return NotFound();
               }
               ViewBag.listFile = await _context.Files.Where(f => f.BoxId == id && f.Name != box.Img).ToListAsync();

               if (ModelState.IsValid)
               {
                    try
                    {

                         _context.Update(box);

                         await _context.SaveChangesAsync();
                         if (file != null)
                         {
                              await UpdateFileAsync(box.id, file, oldImg);


                         }


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
               }
               else
               {
                    return View(box);
               }
               ViewData["UserId"] = new SelectList(_context.Accounts, "id", "password", box.UserId);
               return RedirectToAction("Edit", new { id = box.id });
          }

          public async Task<IActionResult> AdminBan(int id)
          {
               var box = await _context.Boxs.FirstOrDefaultAsync(b => b.id == id);
               box.AdminBan = !box.AdminBan;
               await _context.SaveChangesAsync();
               return Json(new { kq = box.AdminBan.ToString() });
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
          // [HttpPost, ActionName("Delete")]
          // [ValidateAntiForgeryToken]
          public async Task<IActionResult> DeleteConfirmed(int id, int userAccess)
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
               return RedirectToAction("Index", new { userAccess = userAccess });
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
                         View = 0,
                    };
                    var isFileExit = await _context.Files.FirstOrDefaultAsync(f => f.Name == fileSql.Name && f.BoxId == fileSql.BoxId);
                    if (isFileExit == null)
                    {
                         _context.Add(fileSql);
                    }
               }
               await _context.SaveChangesAsync();
               ViewBag.Message = "Files are successfully uploaded";
               ViewBag.box = box;
               var model = await _context.Files.Where(f => f.BoxId == id).ToListAsync();
               return RedirectToAction("Edit", new { id = box.id });
          }
          [HttpGet]
          public async Task<IActionResult> Download(string filename, int id)
          {
               if (filename == null)
                    return Content("filename is not availble");
               var box = await _context.Boxs
                                              .FirstOrDefaultAsync(m => m.id == id);
               var file = await _context.Files.Include(c => c.Box).FirstOrDefaultAsync(f => f.Name == filename && f.Box.id == id);
               file.View = file.View + 1;
               _context.Update(file);
               await _context.SaveChangesAsync();
               var path = Path.Combine(Directory.GetCurrentDirectory(), "upload", box.Url, filename);

               var memory = new MemoryStream();
               using (var stream = new FileStream(path, FileMode.Open))
               {
                    await stream.CopyToAsync(memory);
               }
               memory.Position = 0;
               return File(memory, "application/octet-stream", Path.GetFileName(path));
          }
          [HttpGet]
          public async Task<IActionResult> DowloadAll(int id, string url)
          {
               // var box = await _context.Boxs.FirstOrDefaultAsync( b => b.id == id);
               var listFile = new List<string>() { };
               var listFileSql = _context.Files.Where(f => f.BoxId == id);
               if (listFileSql == null)
               {
                    return Content("Chưa có  file");
               }
               foreach (var file in listFileSql)
               {
                    listFile.Add(Path.Combine(Directory.GetCurrentDirectory(), "upload", url, file.Name));
               }
               using (MemoryStream memoryStream = new MemoryStream())
               {
                    using (ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                         foreach (string filePath in listFile)
                         {

                              string fileName = Path.GetFileName(filePath);
                              ZipArchiveEntry entry = zipArchive.CreateEntry(fileName);

                              using (Stream entryStream = entry.Open())
                              using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                              {
                                   fileStream.CopyTo(entryStream);
                              }
                         }
                    }

                    memoryStream.Position = 0;

                    // Create a new MemoryStream to return as the File content
                    var resultStream = new MemoryStream(memoryStream.ToArray());

                    // Return the ZIP archive as a downloadable file
                    return File(resultStream, "application/zip", "files.zip");
               }
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
               // return RedirectToAction("UpdateFile", new { id = id });
               // return RedirectToAction("Edit", new { id = id });
               return Json(new { });

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
               return Json(new { kq = kq });
          }
          [HttpGet]
          public async Task<IActionResult> GetUser(string userName, int boxId)
          {
               var user = await _context.Accounts.FirstOrDefaultAsync(a => a.username == userName);

               if (user == null)
               {
                    return Json(new
                    {
                         username = "",
                         name = "",
                         img = "/upload/avt/avtuser.jpg",
                         share = "",
                         textShare = "",
                    });

               }
               else
               {
                    var isShare = await _context.BoxShares.FirstOrDefaultAsync(bs => bs.BoxId == boxId && bs.UserId == user.id);
                    if (isShare != null)
                    {
                         return Json(new
                         {
                              username = user.username,
                              name = user.Name,
                              img = "/upload/avt/avtuser.jpg",
                              share = user.id,
                              textShare = "Huy share",
                         });
                    }
                    else
                    {
                         return Json(new
                         {
                              username = user.username,
                              name = user.Name,
                              img = "/upload/avt/avtuser.jpg",
                              share = user.id,
                              textShare = "Share ngay ",
                         });
                    }
               }
          }
          [HttpGet]
          public async Task<IActionResult> ShareBox(int userId, int boxId)
          {
               var shareBox = new BoxShare()
               {
                    BoxId = boxId,
                    UserId = userId,
               };
               var isExit = await _context.BoxShares.FirstOrDefaultAsync(b => b.UserId == userId && b.BoxId == boxId);
               if (isExit == null)
               {
                    _context.Add(shareBox);

               }
               else
               {

                    _context.BoxShares.Remove(isExit);


               }
               await _context.SaveChangesAsync();

               return Json(new { text = "DaShare" });
          }
          [HttpGet]
          public async Task<IActionResult> GetListShare(int boxId)
          {
               var listShare = _context.BoxShares.Include(b => b.User).Select(b => new { BoxId = b.BoxId, name = b.User.Name, userId = b.UserId, userName = b.User.username }).Where(b => b.BoxId == boxId);
               var listShareString = JsonConvert.SerializeObject(listShare);
               return Json(listShareString);
          }
          [HttpPost]
          public async Task<IActionResult> CreateComment(int boxId, string content)
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
          public async Task<IActionResult> DeleteComment(int commentId, int boxId)
          {
               var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
               _context.Comments.Remove(comment);
               await _context.SaveChangesAsync();
               return RedirectToAction("Details", new { id = boxId });
          }
          [HttpGet]
          public async Task<IActionResult> VoteChange(int boxId, int act)
          {
               if (string.IsNullOrEmpty(CurrentUser))
               {
                    agc.a("loi");
               }
               var authorId = await _context.Accounts.FirstOrDefaultAsync(a => a.username == CurrentUser);
               var vote = await _context.Votes.FirstOrDefaultAsync(v => v.AccountId == authorId.id && v.BoxId == boxId);
               if (vote == null)
               {
                    var newVote = new Vote()
                    {
                         BoxId = boxId,
                         AccountId = authorId.id,
                         DateCreated = DateTime.Now,
                         Status = act
                    };
                    _context.Add(newVote);
                    await _context.SaveChangesAsync();
               }
               else
               {
                    if (vote.Status != act)
                    {
                         vote.Status = act;
                         await _context.SaveChangesAsync();

                    }
                    else
                    {
                         vote.Status = 0;
                         act = 0;
                         await _context.SaveChangesAsync();

                    }


               }
               var up = _context.Votes.Where(v => v.BoxId == boxId && v.Status == 1).Count();
               var down = _context.Votes.Where(v => v.BoxId == boxId && v.Status == -1).Count();
               var kq = up - down;
               return Json(new { kq = kq, act = act });

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
          public async Task UpdateFileAsync(int id, IFormFile file, string oldImg)
          {
               var fileName = System.IO.Path.GetFileName(file.FileName);
               var box = await _context.Boxs
                                .FirstOrDefaultAsync(m => m.id == id);
               var ImgFile = await _context.Files.FirstOrDefaultAsync(f => f.BoxId == box.id && f.Name == oldImg);
               if (ImgFile != null)
               {
                    _context.Files.Remove(ImgFile);
                    _context.SaveChangesAsync();
               }
               // create folder
               string path = Path.Combine(Directory.GetCurrentDirectory(), "upload", box.Url);
               Directory.CreateDirectory(path);
               // Iterate each files


               // Get the file name from the browser

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
                    View = 0,
               };
               _context.Add(fileSql);

               await _context.SaveChangesAsync();
          }

     }

}
