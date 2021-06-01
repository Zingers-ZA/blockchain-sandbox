using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox.CustomEventArgs
{
    public class TransactionSubmittedEventArgs : EventArgs
    {
        public Transaction transaction { get; set; }

        public TransactionSubmittedEventArgs(Transaction _transaction)
        {
            this.transaction = _transaction;
        }
    }
}
