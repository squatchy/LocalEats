using System;
using System.Collections.Generic;

namespace LocalEats.Model
{
    public class StateInfo : BaseModel
    {
        public string Name
        {
            get;
            set;
        }

        public List<CityInfo> Cities
        {
            get;
            set;
        }

        public bool AreCitiesLoaded
        {
            get
            {
                return Cities != null;
            }
        }
    }
}
