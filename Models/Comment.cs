using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
     [Table("Comment")]
     public class Comment
     {

          [Key]
          public int Id { get; set; }
          [Display(Name = "Nội dung")]
          public string Content { set; get; }

          [Display(Name = "Ngày tạo")]
          public DateTime DateCreated { set; get; }
          public int BoxId { get; set; }
          [ForeignKey("BoxId")]
          public Box Box { get; set; }
          public int AccountId { get; set; }
          [ForeignKey("AccountId")]
          public Account Account { get; set; }

     }

}