using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.ViewModel
{
    //Add items with imge in table one
    public class ItemImageVM 
    {
       
        public int item_Code_No { get; set; }
        public string Name { get; set; }
        public float Caliber { get; set; }
        public string Type { get; set; }
        public float price { get; set; }
        public int Quantity { get; set; }
        public string ProfilePicture { get; set; }

       // We put values ​​in it so that I can send these values ​​from postmain to Visual Studio in text form and upon receipt, convert them to json
        public string items { get; set; } 
    }
}
