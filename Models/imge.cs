using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models
{
    //Model image items 
    public class imge
    {
        [Key]
        public int image_id { get; set; }
        public string ProfilePicture { get; set; }
        public int item_Id { get; set; }
        [ForeignKey(nameof(item_Id))]
        public items items { get; set; }
    }
}
