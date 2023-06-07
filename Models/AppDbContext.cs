using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace App.Models
{
     public class AppDbContext : DbContext
     {
          public DbSet<Account> Accounts { get; set; }
          public DbSet<Role> Roles { get; set; }
          public DbSet<UserRole> UserRoles { get; set; }
          public DbSet<Box> Boxs { get; set; }
          public DbSet<File> Files { get; set; }
		public DbSet<BoxShare> BoxShares { get; set; }
		public DbSet<Vote> Votes { get; set; } 
		public DbSet<Comment> Comments { get; set; } 


          public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

          protected override void OnModelCreating(ModelBuilder modelBuilder)
          {
               modelBuilder.Entity<UserRole>(entity =>
                                  {
                                       entity.HasKey(c => new { c.RoleId, c.UserId });
                                  });
			 modelBuilder.Entity<Vote>(entity =>
                    {
                     
                         entity.HasOne(c => c.Account).WithMany().OnDelete(DeleteBehavior.Restrict);
                         entity.HasOne(c => c.Box).WithMany().OnDelete(DeleteBehavior.Cascade);


                    });
				modelBuilder.Entity<Comment>(entity =>
                    {
                     
                         entity.HasOne(c => c.Account).WithMany().OnDelete(DeleteBehavior.Restrict);
                         entity.HasOne(c => c.Box).WithMany().OnDelete(DeleteBehavior.Cascade);


                    });
          }
     }
}

