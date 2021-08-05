using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDS.Models
{
    public class SchemaItem
    {
        //[]Int_ Message ID
        public int IntMessageID { get; set; }
        public int IntMessageLineID { get; set; }
    }
}
