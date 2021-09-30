using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lms.Core.Models
{
    public class SampleFilterModel:FilterModelBase
    {
        public string Term { get; set; }

        public SampleFilterModel() : base()
        {
            this.Limit = 100;
        }


        public override object Clone()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject(jsonString, this.GetType());
        }
    }
}
