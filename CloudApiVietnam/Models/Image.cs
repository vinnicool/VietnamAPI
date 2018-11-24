using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class Image
    {
        [Key]
        public string name { get; set; }
        public Byte[] image { get; set; }
        
    }


}