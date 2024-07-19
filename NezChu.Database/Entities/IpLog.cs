using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezChu.Database.Entities
{
    public class IpLog
    {
        [Key]
        public int LogId { get; set; }

        public string Ipaddress { get; set; } = null!;

        public DateTime Date { get; set; }
    }
}
