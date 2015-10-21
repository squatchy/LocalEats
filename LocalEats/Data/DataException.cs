using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalEats.Data
{
    public class DataException : Exception
    {
        private Exception e;

		public DataException(Exception e)
        {
            this.e = e;
        }

        public DataException(string message) : base(message)
        {
        }
    }
}
