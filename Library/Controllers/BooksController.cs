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

    public ActionResult Index()
    {
      List<Book> model = _db.Books.ToList();
      return View(model);
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
        ViewBag.FoundAuthors = searchAuthors;
      }
      ViewBag.AuthorId = _db.Authors.ToList();
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

    /*public ActionResult Edit(int id)
    {
      var thisContractor = _db.Contractors.FirstOrDefault(contractors => contractors.ContractorId == id);
      return View(thisContractor);
    }

    [HttpPost]
    public ActionResult Edit(Contractor contractor)
    {
      _db.Entry(contractor).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddClient(int id)
    {
      var thisContractor = _db.Contractors.FirstOrDefault(contractors => contractors.ContractorId == id);
      ViewBag.ClientId = new SelectList(_db.Clients, "ClientId", "Name");
      return View(thisContractor);
    }

    [HttpPost]
    public ActionResult AddClient(Contractor contractor, int ClientId)
    {
      var testvariable = _db.ClientContractor.FirstOrDefault(join=>join.ClientId == ClientId && join.ContractorId == contractor.ContractorId);

      if(testvariable != null)
      {
      return RedirectToAction("Details", new {id=contractor.ContractorId});
      }

      if (ClientId != 0)
      {
      _db.ClientContractor.Add(new ClientContractor() { ClientId = ClientId, ContractorId = contractor.ContractorId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisContractor = _db.Contractors.FirstOrDefault(contractors => contractors.ContractorId == id);
      return View(thisContractor);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisContractor = _db.Contractors.FirstOrDefault(contractors => contractors.ContractorId == id);
      _db.Contractors.Remove(thisContractor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteClient(int joinId)
    {
      var joinEntry = _db.ClientContractor.FirstOrDefault(entry => entry.ClientContractorId == joinId);
      _db.ClientContractor.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddArmory(int id)
    {
      var thisContractor = _db.Contractors.FirstOrDefault(contractors => contractors.ContractorId == id);
      ViewBag.ArmoryId = new SelectList(_db.Armories, "ArmoryId", "WeaponName");
      return View(thisContractor);
    }

    [HttpPost]
    public ActionResult AddArmory(Contractor contractor, int ArmoryId)
    {
      var testvariable = _db.ContractorArmory.FirstOrDefault(join=>join.ArmoryId == ArmoryId && join.ContractorId == contractor.ContractorId);

      if(testvariable != null)
      {
      return RedirectToAction("Details", new {id=contractor.ContractorId});
      }

      if (ArmoryId != 0)
      {
      _db.ContractorArmory.Add(new ContractorArmory() { ArmoryId = ArmoryId, ContractorId = contractor.ContractorId });
      }
      _db.SaveChanges();
      return RedirectToAction("Details", new {id=contractor.ContractorId});
    }

    [HttpPost]
    public ActionResult DeleteArmory(int joinId)
    {
      var joinEntry = _db.ContractorArmory.FirstOrDefault(entry => entry.ContractorArmoryId == joinId);
      _db.ContractorArmory.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new {id=joinEntry.ContractorId});
    }*/
  }
}