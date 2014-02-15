using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClicky.Core
{
    public class UnknownRemoteProviderException : Exception
    {
        public UnknownRemoteProviderException(string remote)
            : base(remote)
        {
        }
    }
}
