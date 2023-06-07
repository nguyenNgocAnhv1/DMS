using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Controllers.Boxs
{
     [Area("Box")]
     [Route("/Box/{action=index}")]
     public class BoxController : Controller
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
               var appDbContext = _context.Boxs.Include(b => b.Account);
               return View(await appDbContext.ToListAsync());
          }

          // GET: Box/Details/5
          public async Task<IActionResult> Details(int? id)
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
               box.Url = agc.GenerateSlug(box.Title) +"_"+ CodeRandom(6);
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
               // box.UserId = _context.Accounts.FirstOrDefault(a => a.username == _user).id;
               // box.Url = agc.GenerateSlug(box.Title) + CodeRandom(6);
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
               if (box != null)
               {
                    _context.Boxs.Remove(box);
               }

               await _context.SaveChangesAsync();
               return RedirectToAction(nameof(Index));
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
               agc.a(kq);
               // return Content("ac");
               return Json(new { kq = kq });
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
               // agc.a(kq);
               // return Content("ac");
               return kq;
          }

          private bool BoxExists(int id)
          {
               return (_context.Boxs?.Any(e => e.id == id)).GetValueOrDefault();
          }
     }
}
