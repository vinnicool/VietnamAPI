using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class Formulieren
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is requierd")]
        public string Name { get; set; }
        public string Region { get; set; }
        public string FormTemplate { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("FormulierenId")]
        public ICollection<FormContent> FormContent { get; set; } 
    }


}