using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HBK.Models
{
    [Table("tblCompany")]
    public class Company
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyID { get; set; }

        [Required]
        [MaxLength(500)]
        public string CompanyName { get; set; }

        [Required]
        [MaxLength(6)]
        public string CompanyType { get; set; }
    }
}