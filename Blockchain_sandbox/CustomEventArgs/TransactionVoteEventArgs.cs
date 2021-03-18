using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox.CustomEventArgs
{
    public class TransacionVoteEventArgs : EventArgs
    {
        public Guid transactionId { get; set; }
        public bool accepted { get; set; }

        public TransacionVoteEventArgs(Guid _transactionId, bool _accepted)
        {
            this.transactionId = _transactionId;
            this.accepted = _accepted;
        }
    }
}
