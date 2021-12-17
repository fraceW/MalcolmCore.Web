using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MalcolmCore.Data
{
    public class modelA
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public List<modelB> modelBs { get; set; } = new List<modelB>(); 
    }
}
