using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
     public class BoxShare
     {
          [Key]
          public int id { get; set; }
          [StringLength(int.MaxValue)]
          public string ListName { get; set; }
          public int BoxId { get; set; }
          [ForeignKey("BoxId")]
          public Box Box { get; set; }
     }
}