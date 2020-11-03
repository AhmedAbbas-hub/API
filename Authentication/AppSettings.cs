using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Authentication
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public TimeSpan Tokenlifetime { get; set; }
    }
}
