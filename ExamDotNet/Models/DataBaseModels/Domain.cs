using System;
using System.Collections.Generic;

namespace ExamDotNet
{
    public partial class Domain
    {
        public Domain()
        {
            UrlContent = new HashSet<UrlContent>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UrlContent> UrlContent { get; set; }
    }
}
