using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Models
{
  public class LibraryContext : IdentityDbContext<ApplicationUser>
  // {
  //   public virtual DbSet<Category> Categories { get; set; }
  //   public DbSet<Item> Items { get; set; }
  //   public DbSet<CategoryItem> CategoryItem { get; set; }
    
    public LibraryContext(DbContextOptions options) : base(options) { }
  }
}