using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models
{
    public class items           //model item gold 
    {
        [Key]
        public int item_Id { get; set; }
        public int item_Code_No { get; set; }       //code number gold
        public string Name { get; set; }           //Name gold example(necklace , bracelet)
        public float Caliber { get; set; }        //Caliber gold
        public string Type { get; set; }          //Type gold example (Iraqi,Turkish)
        public float price { get; set; }          //price gold
        public int Quantity { get; set; }         //quantity gold
        public ICollection<imge> Imges { get; set; }
        public ICollection<Invoice> invoices { get; set; }
    }
}
