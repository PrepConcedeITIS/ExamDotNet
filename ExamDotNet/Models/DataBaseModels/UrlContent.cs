using System;
using System.Collections.Generic;

namespace ExamDotNet
{
    public partial class UrlContent
    {
        public int Id { get; set; }
        public int? DomainId { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }

        public virtual Domain Domain { get; set; }
    }
}
