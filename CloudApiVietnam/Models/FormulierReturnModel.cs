using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class FormulierReturnModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string FormTemplate { get; set; }
    }
}