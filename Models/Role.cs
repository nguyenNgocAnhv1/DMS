using System.ComponentModel.DataAnnotations;

namespace App.Models
{
     public class Role
     {
          [Key]
          public int id { get; set; }
          [StringLength(255)]
          public string Name { get; set; }

     }
}