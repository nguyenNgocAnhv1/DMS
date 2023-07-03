using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
     public class UserRole
     {
          public int UserId { get; set; }
          public int RoleId { get; set; }
          [ForeignKey("UserId")]
          public Account Account { get; set; }
          [ForeignKey("RoleId")]
          public Role Role { get; set; }
          
     }
}