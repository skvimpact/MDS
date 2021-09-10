using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDS.Models
{
    public class SchemaItem
    {
        public int IntMessageID { get; set; }
        public int IntMessageLineID { get; set; }
        public string ElementName { get; set; }
        public int LineType { get; set; }
        public int ParentElementID { get; set; }
        public string Value { get; set; }
        public int Indentation { get; set; }
        public string FullPath { get; set; }
        public int Multiplicity { get; set; }
    }
}
