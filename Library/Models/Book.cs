using System.Collections.Generic;

namespace Library.Models
{
  public class Book
  {
    public Book()
    {
      this.Authors = new HashSet<BookAuthor>();
      this.Checkouts = new HashSet<Checkout>();
    }
    public int BookId {get; set;}
    public string Title {get; set;}
    public int Copies {get; set;}
    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<BookAuthor> Authors {get; set;}
    public virtual ICollection<Checkout> Checkouts {get; set;}
  }
}