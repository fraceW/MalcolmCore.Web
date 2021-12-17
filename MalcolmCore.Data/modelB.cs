using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MalcolmCore.Data
{
    public class modelB
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("ModelAId")]
        public string ModelAId { get; set; }
    }
}
