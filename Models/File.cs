using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
     public class File
     {
          [Key]
          public int id { get; set; }
          [StringLength(255)]
          public string Name { get; set; }
          [Column(TypeName = "decimal(18,1)")]
          public double Size { get; set; }
          public DateTime DatePost { get; set; }
          public int BoxId { get; set; }
          [ForeignKey("BoxId")]
          public Box Box{get; set; }
     }
}