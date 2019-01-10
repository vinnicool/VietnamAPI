using System.Collections.Generic;

namespace CloudApiVietnam.Models
{
    public class FormImageModel
    {
        public int FormId { get; set; }
        public string Name { get; set; }
        public string TemplateName { get; set; }
        public string BirthYear { get; set; }
        
        public List<byte[]> Image { get; set; }
    }
}