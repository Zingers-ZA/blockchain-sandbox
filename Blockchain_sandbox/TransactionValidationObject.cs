using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox.CustomEventArgs
{
    public class TransactionValidationObject
    {
        public Transaction transaction { get; set; }
        public int validVotes { get; set; }
        public int invalidVotes { get; set; }

        public TransactionValidationObject(Transaction _transaction)
        {
            this.transaction = _transaction;
        }
    }
}
