using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class FormContentBindingModel
    {
        //[Required(ErrorMessage = "FormId is verplicht")]      
        public int Id { get; set; }
        public string FormContent { get; set; }
        public List<byte[]> Images { get; set; }
    }

    public class FormContentModel
    {   
        public int Id { get; set; }
        public List<FormContentKeyValuePair> FormContent { get; set; }
        public List<byte[]> Images { get; set; }
    }

}