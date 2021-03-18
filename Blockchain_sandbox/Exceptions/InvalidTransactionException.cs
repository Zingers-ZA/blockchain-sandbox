using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox.Exceptions
{
    class InvalidTransactionException : Exception
    {
        public InvalidTransactionException() : base("This transaction is not valid")
        {

        }
    }
}
