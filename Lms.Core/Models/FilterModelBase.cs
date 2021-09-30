using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lms.Core.Models
{
    public abstract class FilterModelBase: ICloneable
    {
        public int Page { get; set; }
        public int Limit { get; set; }

        public FilterModelBase()
        {
            this.Page = 1;
            this.Limit = 100;
        }

        public abstract object Clone();
    }
}

