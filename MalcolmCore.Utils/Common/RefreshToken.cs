using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Utils.Common
{
    public class RefreshToken
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string Token { get; set; }
        public DateTime expire { get; set; }
    }
}
