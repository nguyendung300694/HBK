using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HBK.Models
{
    [Table("tblProductCategory")]
    public class ProductCategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductCategoryId { get; set; }

        [Required, MaxLength(200)]
        public string ProductCategoryName { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public virtual ProductCategoryExtend ProductCategoryExtend { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}