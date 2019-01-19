using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class FormContentApiModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int FormTemplateId { get; set; }
    }
}