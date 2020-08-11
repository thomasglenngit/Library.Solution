using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Library.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
  public class BooksController : Controller
  {
    private readonly LibraryContext _db;
    public BooksController(LibraryContext db)
    {
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
      return View(thisBook);
    }
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
    public ActionResult Create(Book book, int[] AuthorId)
    {
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

    public ActionResult Edit(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(books => books.BookId == id);
      return View(thisBook);
    }

    [HttpPost]
    public ActionResult Edit(Book book)
    {
      _db.Entry(book).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddAuthor(int id, string searchAuthor)
    {
      var thisBook = _db.Books.FirstOrDefault(books => books.BookId == id);
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

    public ActionResult Delete(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(books => books.BookId == id);
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

    // [HttpPost]
    // public ActionResult DeleteAuthor(int joinId)
    // {
    //   var joinEntry = _db.ClientContractor.FirstOrDefault(entry => entry.ClientContractorId == joinId);
    //   _db.ClientContractor.Remove(joinEntry);
    //   _db.SaveChanges();
    //   return RedirectToAction("Index");
    // }
  }
}