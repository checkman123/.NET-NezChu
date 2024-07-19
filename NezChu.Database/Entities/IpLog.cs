using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NezChu.Database.Entities
{
    public class IpLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }

        public string Ipaddress { get; set; } = null!;

        public DateTime Date { get; set; }
    }
}
