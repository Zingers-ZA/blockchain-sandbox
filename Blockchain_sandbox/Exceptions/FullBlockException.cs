using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox.Exceptions
{
    class FullBlockException : Exception
    {
        public FullBlockException() : base("This block is full")
        {
            
        }
    }
}
