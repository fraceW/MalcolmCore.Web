using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Utils.Model
{
    public class NonBodyParameter
    {
        public string Name { get; set; }
        public string In { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
    }
}
