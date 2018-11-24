using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class FormContent
    {
        public int Id { get; set; }                
        public string Content { get; set; }


        [Required(ErrorMessage = "FormulierenId is required")]
        public int FormulierenId { get; set; }
        //public Formulieren Formulieren { get; set; }

    }
}