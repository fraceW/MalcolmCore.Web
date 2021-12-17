using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MalcolmCore.Data
{
    public class useDetail
    {
        [Key]
        public string id { get; set; }
        [Display(Name = "详情")]
        public string useDetails { get; set; }
    }
}
