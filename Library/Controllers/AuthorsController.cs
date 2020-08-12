using Microsoft.AspNetCore.Mvc;
using Library.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library.Controllers
{
  public class AuthorsController : Controller
  {
    private readonly LibraryContext _db;

    public  AuthorsController(LibraryContext db)
    {
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

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Author author)
    {
      _db.Authors.Add(author);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisAuthor  = _db.Authors
        .Include(author => author.Books)
        .ThenInclude(join => join.Book)
        .FirstOrDefault(authors => authors.AuthorId == id);
      return View(thisAuthor);
    }

    public ActionResult Edit(int id)
    {
      var thisAuthor = _db.Authors.FirstOrDefault(authors => authors.AuthorId == id);
      return View(thisAuthor);
    }

    [HttpPost]
    public ActionResult Edit(Author author)
    {
      _db.Entry(author).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisAuthor = _db.Authors.FirstOrDefault(authors => authors.AuthorId == id);
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

    public ActionResult AddBook(int id, string searchBook)
    {
      var thisAuthor = _db.Authors.FirstOrDefault(authors => authors.AuthorId == id);
      if(!string.IsNullOrEmpty(searchBook))
      {
        var searchBooks = _db.Books.Where(books => books.Title.Contains(searchBook)).ToList();
        ViewBag.BookId = searchBooks;
      }
      return View(thisAuthor);
    }

    [HttpPost]
    public ActionResult AddBook(Book book, int [] BookId)
    {
      if(BookId.Length != 0)
      {
        foreach(int id in BookId)
        {
          _db.BooksAuthors.Add(new BookAuthor() { BookId = id, BookAuthorId = book.BookId});
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