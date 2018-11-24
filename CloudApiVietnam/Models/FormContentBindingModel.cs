using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class FormContentBindingModel
    {
        [Required(ErrorMessage = "FormId is verplicht")]      
        public int FormId { get; set; }
        public string Content { get; set; }
    }
}