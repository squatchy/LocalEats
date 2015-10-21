using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalEats.Model
{
    public class BaseModel
    {
        public string Id { get; set; }

        public override bool Equals(object obj)
        {
            BaseModel r = obj as BaseModel;
            if (r == null)
                return false;
            else
                return this.Id.Equals(r.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
