using System.ComponentModel.DataAnnotations;
namespace App.Models
{
     public class Account
     {
          [Key]
          public int id { get; set; }
          [Required(ErrorMessage = "Ban phai nhap user name")]
          [StringLength(255)]
          [MinLength(6)]
          public string username { get; set; }
		[StringLength(int.MaxValue)]
          [MinLength(6)]
          [Required(ErrorMessage = "Ban phai nhap password")]
          public string password { get; set; }
          [StringLength(255)]
          public string? Name { get; set; }
          [StringLength(255)]
          public string? JobTitle { get; set; }
          [StringLength(255)]
          public string? Img { get; set; }
          [StringLength(255)]
          public string? Description { get; set; }
          // public DateTime? BanDate { get; set; }
          public bool? BanEnabled { get; set; }
          [StringLength(255)]
          public string? Email { get; set; }
     }
}
