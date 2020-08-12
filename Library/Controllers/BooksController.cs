using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Library.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Library.Controllers
{
  public class BooksController : Controller
  {
    private readonly LibraryContext _db;
     private readonly UserManager<ApplicationUser> _userManager; 
    public BooksController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public ActionResult Index(string searchBook)
    {
      if(!string.IsNullOrEmpty(searchBook))
      {
        var searchBooks = _db.Books.Where(books => books.Title.Contains(searchBook)).ToList();                    
        return View(searchBooks);
      }
      return View(_db.Books.ToList());
    }

    public ActionResult Details(int id)
    {
      var thisBook = _db.Books
          .Include(book => book.Authors)
          .ThenInclude(join => join.Author)
          .Include(book => book.Checkouts)
          .ThenInclude(join => join.Patron)
          .FirstOrDefault(book => book.BookId == id);
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ViewBag.IsCurrentUser = userId != null ? userId == thisBook.User.Id : false;
      return View(thisBook);
    }
    
    [Authorize]
    public ActionResult Create(string searchAuthor)
    {
      if(!string.IsNullOrEmpty(searchAuthor))
      {
        var searchAuthors = _db.Authors.Where(authors => authors.Name.Contains(searchAuthor)).ToList();        
        ViewBag.AuthorId = searchAuthors;
      }
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Book book, int[] AuthorId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      book.User = currentUser;
      _db.Books.Add(book);
      if(AuthorId.Length !=0)
      {
        foreach(int id in AuthorId)
        {
          _db.BooksAuthors.Add(new BookAuthor() { AuthorId = id, BookId = book.BookId});
        }
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [Authorize]
    public async Task<ActionResult> Edit(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisBook = _db.Books.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(books => books.BookId == id);
      if (thisBook == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      return View(thisBook);
    }

    [HttpPost]
    public ActionResult Edit(Book book)
    {
      _db.Entry(book).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [Authorize]
    public async Task<ActionResult> AddAuthor(int id, string searchAuthor)
    {

      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisBook = _db.Books.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(books => books.BookId == id);
      if (thisBook != null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      if(!string.IsNullOrEmpty(searchAuthor))
      {
        var searchAuthors = _db.Authors.Where(authors => authors.Name.Contains(searchAuthor)).ToList();        
        ViewBag.AuthorList = searchAuthors;
      }
      return View(thisBook);
    }

    [HttpPost]
    public ActionResult AddAuthor(Book book, int[] AuthorList)
    {
      if(AuthorList.Length !=0)
      {
        foreach(int id in AuthorList)
        {
          _db.BooksAuthors.Add(new BookAuthor() { AuthorId = id, BookId = book.BookId});
        }
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteAuthor(int joinId)
    {
      var joinEntry = _db.BooksAuthors.FirstOrDefault(entry => entry.BookAuthorId == joinId);
      _db.BooksAuthors.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisBook = _db.Books.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(books => books.BookId == id);
      if (thisBook != null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      return View(thisBook);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(books => books.BookId == id);
      _db.Books.Remove(thisBook);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}