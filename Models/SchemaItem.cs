using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDS.Models
{
    public class SchemaItem
    {
        [Column("Int_ Message ID")]
        public int IntMessageID { get; set; }
        [Column("Int_ Message Line ID")]
        public int IntMessageLineID { get; set; }
    }
}
