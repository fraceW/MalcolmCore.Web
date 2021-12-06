using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Utils.Model
{
    public class RetuenResult<T>
    {
        public int code { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }
}
