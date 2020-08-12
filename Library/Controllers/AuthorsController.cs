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

namespace Library.Controllers
{
  public class AuthorsController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager; 

    public  AuthorsController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    } 

    public ActionResult Index(string searchAuthor)
    {
      if(!string.IsNullOrEmpty(searchAuthor))
      {
        var searchAuthors = _db.Authors.Where(authors => authors.Name.Contains(searchAuthor)).ToList();                    
        return View(searchAuthors);
      }
      return View(_db.Authors.ToList());
    }

    [Authorize]
    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Author author)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      author.User = currentUser;
      _db.Authors.Add(author);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisAuthor  = _db.Authors
        .Include(author => author.Books)
        .ThenInclude(join => join.Book)
        .Include(author => author.User)
        .FirstOrDefault(authors => authors.AuthorId == id);
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ViewBag.IsCurrentUser = userId != null ? userId == thisAuthor.User.Id : false;
      return View(thisAuthor);
    }
    
    [Authorize]
    public async Task<ActionResult> Edit(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisAuthor = _db.Authors.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(authors => authors.AuthorId == id);
      if (thisAuthor == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      return View(thisAuthor);
    }

    [HttpPost]
    public ActionResult Edit(Author author)
    {
      _db.Entry(author).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisAuthor = _db.Authors.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(authors => authors.AuthorId == id);
      if (thisAuthor == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      return View(thisAuthor);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisAuthor = _db.Authors.FirstOrDefault(authors => authors.AuthorId == id);
      _db.Authors.Remove(thisAuthor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    
    [Authorize]
    public async Task<ActionResult> AddBook(int id, string searchBook)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisAuthor = _db.Authors.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(authors => authors.AuthorId == id);
      if (thisAuthor == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      if(!string.IsNullOrEmpty(searchBook))
      {
        var searchBooks = _db.Books.Where(books => books.Title.Contains(searchBook)).ToList();
        ViewBag.BookId = searchBooks;
      }
      return View(thisAuthor);
    }

    [HttpPost]
    public ActionResult AddBook(Author author, int [] BookId)
    {
      if(BookId.Length != 0)
      {
        foreach(int id in BookId)
        {
          _db.BooksAuthors.Add(new BookAuthor() { BookId = id, AuthorId = author.AuthorId });
        }
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteBook(int joinId)
    {
      var joinEntry = _db.BooksAuthors.FirstOrDefault(entry => entry.BookAuthorId == joinId);
      _db.BooksAuthors.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}