using Microsoft.AspNetCore.Mvc;
using Library.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using System;


namespace Library.Controllers
{
  [Authorize]
  public class PatronsController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public PatronsController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public ActionResult Index(string searchPatron)
    {
      if(!string.IsNullOrEmpty(searchPatron))
      {
        var searchPatrons = _db.Patrons.Where(patrons => patrons.Name.Contains(searchPatron));                    
        return View(searchPatrons);
      }
      return View(_db.Patrons.ToList());
    }

    [Authorize]
    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Patron patron)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      patron.User = currentUser;
      _db.Patrons.Add(patron);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisPatron = _db.Patrons 
          .Include(patron => patron.Checkouts)
          .ThenInclude(join => join.Book)
          .FirstOrDefault(patron => patron.PatronId == id);
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ViewBag.IsCurrentUser = userId != null ? userId == thisPatron.User.Id : false;
      return View(thisPatron);
    }

    [Authorize]
    public async Task<ActionResult> Edit(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisPatron = _db.Patrons.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(patrons => patrons.PatronId == id);
      if (thisPatron == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      return View(thisPatron);
    }

    [HttpPost]
    public ActionResult Edit(Patron patron)
    {
      _db.Entry(patron).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisPatron = _db.Patrons.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(patrons => patrons.PatronId == id);
      if (thisPatron == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      return View(thisPatron);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisPatron = _db.Patrons.FirstOrDefault(patrons => patrons.PatronId == id);
      _db.Patrons.Remove(thisPatron);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }  

    [Authorize]
    public async Task<ActionResult> CheckOutBook(int id, string searchBook)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisPatron = _db.Patrons.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(patrons => patrons.PatronId == id);
      if (thisPatron == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      if(!string.IsNullOrEmpty(searchBook))
      {
        var searchBooks = _db.Books.Where(books => books.Title.Contains(searchBook)).ToList();
        ViewBag.BookId = searchBooks;
      }
      return View(thisPatron);
    }

    [HttpPost]
    public ActionResult CheckOutBook(Patron patron, int[] BookId)
    {
      if(BookId.Length !=0)
      {
        foreach(int id in BookId)
        {
          _db.Checkouts.Add(new Checkout() { PatronId = patron.PatronId, BookId = id, DueDate = DateTime.Now + 30});
        }
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [Authorize]
    public async Task<ActionResult> CheckInBook(int id, string searchBook)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisPatron = _db.Patrons.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(patrons => patrons.PatronId == id);
      if (thisPatron == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      if(!string.IsNullOrEmpty(searchBook))
      {
        var searchBooks = _db.Books.Where(books => books.Title.Contains (searchBook)).ToList();
        ViewBag.BookId = searchBooks;
      }
      return View(thisPatron);
    
    } 
    [HttpPost]
    public ActionResult CheckInBook(Patron patron, int[] BookId, DateTime DueDate)
    {
      if(BookId.Length !=0)
      {
        foreach(int id in BookId)
        {
          if(DueDate < (DateTime.Now + 30))
          _db.Checkouts.Remove(new Checkout() { PatronId = patron.PatronId, BookId = id});
        }
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

  }
}

