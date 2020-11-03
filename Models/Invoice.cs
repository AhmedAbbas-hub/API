using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models
{
    public class Invoice
    {
        [Key]
        public int Invoice_Id { get; set; }
        public int Quantity{get;set;}
        public DateTime Date { get; set; }
        public int item_Id { get; set; }
        [ForeignKey(nameof(item_Id))]
        public items items { get; set; }
    }
}
