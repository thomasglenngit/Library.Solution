using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Library.Models
{
  public class Patron
  {
    public Patron() : base()
    {
      this.Checkouts = new HashSet<Checkout>();
    }
    public int PatronId {get; set;}
    public string Name {get; set;}
    public virtual ApplicationUser User {get; set;}
    public virtual ICollection<Checkout> Checkouts {get; set;}
  }
}