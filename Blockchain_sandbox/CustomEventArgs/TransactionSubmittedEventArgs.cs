using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox.CustomEventArgs
{
    public class TransactionEventArgs : EventArgs
    {
        public Transaction transaction { get; set; }

        public TransactionEventArgs(Transaction _transaction)
        {
            this.transaction = _transaction;
        }
    }
}
