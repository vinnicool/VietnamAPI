using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class FormulierenBindingModel
    {
     
        //[Required(ErrorMessage = "Naam is verplicht")]
        //[Display(Name = "Name")]
        public string Name { get; set; }

        public string Region { get; set; }
        public string FormTemplate { get; set; }
        //public List<FormContent> FormContent { get; set; }

        public FormulierenBindingModel(string name, string region, string formTemplate)
        {
            this.Name = name;
            this.Region = region;
            this.FormTemplate = formTemplate;
        }

        public FormulierenBindingModel()
        {

        }
    }
}