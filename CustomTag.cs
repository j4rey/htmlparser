using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htmlstate
{
    class CustomTag
    {
        public CustomTag(String tagname)
        {
            this.TagName = tagname;
        }
        public String TagName { get; set; }

        public override string ToString()
        {
            return TagName;
        }
    }
}
