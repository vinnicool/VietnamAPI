using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class FormTemplatePlusContent
    {
        public int Id { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }
        public string FormTemplate { get; set; }
        public FormContentApiModel FormContent { get; set; }
        //public List<byte[]> Images { get; set; }
    }
}