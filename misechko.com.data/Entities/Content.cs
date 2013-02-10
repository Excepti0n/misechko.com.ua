using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace misechko.com.data.Entities
{
    public class Content: IdableEntity
    {
        public string ContentKey { get; set; }
        public string ContentMarkup { get; set; }
    }
}
