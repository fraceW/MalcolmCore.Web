using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MalcolmCore.WebApi.Utils
{
    public class ActionHelp
    {
        private static Action<string, string> OnCapture;
        public static void Init(Action<string, string> action)
        {
            OnCapture = action;
        }


        public static void Do() 
        {
            OnCapture("123456", "789");
        }
    }
}
