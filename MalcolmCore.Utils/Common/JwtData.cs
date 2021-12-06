using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Utils.Common
{
    public class JwtData
    {
        public DateTime expire { get; set; }  //代表过期时间
        public string userId { get; set; }
        public string userAccount { get; set; }
    }
}
