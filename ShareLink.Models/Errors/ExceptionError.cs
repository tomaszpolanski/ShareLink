using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLink.Models.Errors
{
    public class ExceptionError : ErrorBase
    {
        public readonly Exception Ex;

        public ExceptionError(Exception e)
        {
            Ex = e;
        }
    }
}
