using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HBK.Models
{
    [Table("tblProductExtend")]
    public class ProductExtend
    {

        [Key, ForeignKey("Product")]
        public int ProductId { get; set; }

        [MaxLength(400)]
        public string FileName { get; set; }

        public int? FileSize { get; set; }

        [MaxLength(1000)]
        public string FilePath { get; set; }

        public virtual Product Product { get; set; }
    }
}