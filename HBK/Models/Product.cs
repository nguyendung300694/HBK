using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HBK.Models
{
    [Table("tblProduct")]
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [ForeignKey("ProductCategory")]
        public int ProductCategoryId { get; set; }

        [Required, MaxLength(200)]
        public string ProductName { get; set; }

        [MaxLength(4000)]
        public string ProductContent { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public int? View { get; set; }

        public virtual ProductExtend ProductExtend { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
    }
}