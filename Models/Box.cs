using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
     public class Box
     {
          [Key]
          public int id { get; set; }
          [StringLength(255)]
          public string Title { get; set; }
          public string Content { get; set; }
          [StringLength(255)]
          public string? Pass{get; set; }
          [StringLength(255)]
          public string? ShareCode { get; set; }
          [StringLength(255)]
          public string? Url{get; set; }
          public int? View { get; set; }
          [Column(name: "IsAvaliable")]
          public bool IsPublic{get; set; }
          public int? UserId { get; set; }
          public DateTime? DateCreated{get; set; }
          [ForeignKey("UserId")]
          public Account? Account { get; set; }

     }
}