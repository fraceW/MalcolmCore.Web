using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MalcolmCore.Data
{
    public class useinfo
    {
        [Key]
        public string id { get; set; }
        [Display(Name ="用户")]
        public string usename { get; set; }
        [Display(Name = "密码")]
        public string pwd { get; set; }
        [Display(Name = "用户说明")]
        public string useremark { get; set; }
        [Display(Name = "创建时间")]
        public DateTime creatdate { get; set; }
    }
}
