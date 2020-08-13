using System;

namespace Library.Models
{
  public class Checkout
  {
    public int CheckoutId {get; set;}
    public int PatronId {get; set;}
    public int BookId {get; set;}
    public DateTime DueDate {get; set;}
    public virtual Patron Patron {get; set;}
    public virtual Book Book {get; set;}
  }
}