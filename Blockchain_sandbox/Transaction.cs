using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox
{
    public class Transaction
    {
        public Guid Id { get; }
        public Decimal Amount { get; }
        public Guid RecieverId { get; }
        public Guid SenderId { get; }

        public Transaction(Decimal _Amount, Guid _ReciverId, Guid _SenderId)
        {
            this.Id = Guid.NewGuid();
            this.Amount = _Amount;
            this.RecieverId = _ReciverId;
            this.SenderId = _SenderId;
        }
    }
}
